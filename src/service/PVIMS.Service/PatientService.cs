using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using VPS.Common.Repositories;

namespace PVIMS.Services
{
    public class PatientService : IPatientService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientStatus> _patientStatusRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<EncounterType> _encounterTypeRepository;
        private readonly IRepositoryInt<EncounterTypeWorkPlan> _encounterTypeWorkPlanRepository;
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<Priority> _priorityRepository;
        private readonly ITypeExtensionHandler _typeExtensionHandler;

        public PatientService(IUnitOfWorkInt unitOfWork,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientStatus> patientStatusRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<EncounterType> encounterTypeRepository,
            IRepositoryInt<EncounterTypeWorkPlan> encounterTypeWorkPlanRepository,
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<Priority> priorityRepository,
            ITypeExtensionHandler modelExtensionBuilder)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientStatusRepository = patientStatusRepository ?? throw new ArgumentNullException(nameof(patientStatusRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _encounterTypeRepository = encounterTypeRepository ?? throw new ArgumentNullException(nameof(encounterTypeRepository));
            _encounterTypeWorkPlanRepository = encounterTypeWorkPlanRepository ?? throw new ArgumentNullException(nameof(encounterTypeWorkPlanRepository));
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _priorityRepository = priorityRepository ?? throw new ArgumentNullException(nameof(priorityRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _typeExtensionHandler = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
        }

        public SeriesValueList[] GetElementValues(long patientId, string elementName, int records)
        {
            var patientFromRepo = _patientRepository.Get(p => p.Id == patientId);
            if (patientFromRepo == null)
            {
                throw new ArgumentException(nameof(patientId));
            }
            if (string.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentException(nameof(elementName));
            }

            var encounters = _encounterRepository.List(e => e.Patient.Id == patientId)
                .OrderBy(e => e.EncounterDate);

            var seriesValueArray = new List<SeriesValueList>();
            var seriesValueList = new SeriesValueList()
            {
                Name = elementName
            };

            var values = new List<SeriesValueListItem>();
            foreach (Encounter encounter in encounters)
            {
                var datasetInstance = _datasetInstanceRepository.Get(di => di.ContextID == encounter.Id && di.Dataset.ContextType.Id == (int)ContextTypes.Encounter);
                if(datasetInstance != null)
                {
                    var value = datasetInstance.GetInstanceValue(elementName);
                    var decimalValue = 0M;
                    Decimal.TryParse(value, out decimalValue);
                    if(!String.IsNullOrWhiteSpace(value))
                    {
                        var modelItem = new SeriesValueListItem()
                        {
                            Value = decimalValue,
                            //Min = intValue - ((intValue * 20) / 100),
                            //Max = intValue + ((intValue * 20) / 100),
                            Name = encounter.EncounterDate.ToString("yyyy-MM-dd")
                        };
                        values.Add(modelItem);
                        if(values.Count >= records) { break; }
                    }
                }
            }
            seriesValueList.Series = values;
            seriesValueArray.Add(seriesValueList);
            return seriesValueArray.ToArray();
        }

        public SeriesValueListItem GetCurrentElementValueForPatient(long patientId, string elementName)
        {
            var patientFromRepo = _patientRepository.Get(p => p.Id == patientId);
            if (patientFromRepo == null)
            {
                throw new ArgumentException(nameof(patientId));
            }
            if(string.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentException(nameof(elementName));
            }

            var encounter = patientFromRepo.GetCurrentEncounter();
            if (encounter == null) return null;

            var datasetInstance = _datasetInstanceRepository.Get(di => di.ContextID == encounter.Id && di.Dataset.ContextType.Id == (int)ContextTypes.Encounter);
            if (datasetInstance == null) return null;

            var value = datasetInstance.GetInstanceValue(elementName);
            var tempDecimal = 0.00M;
            if (decimal.TryParse(value, out tempDecimal))
            {
                return new SeriesValueListItem()
                {
                    Value = tempDecimal,
                    Name = encounter.EncounterDate.ToString("yyyy-MM-dd")
                };
            }

            return null;
        }

        /// <summary>
        /// Add a new patient to the repository
        /// </summary>
        /// <param name="patientDetail">The details of the patient to add</param>
        public int AddPatient(PatientDetailForCreation patientDetail)
        {
            var facility = _facilityRepository.Get(f => f.FacilityName == patientDetail.CurrentFacilityName);
            if(facility == null)
            {
                throw new ArgumentException(nameof(patientDetail.CurrentFacilityName));
            }
            var patientStatus = _patientStatusRepository.Get(f => f.Description == "Active");

            var newPatient = new Patient
            {
                FirstName = patientDetail.FirstName,
                MiddleName = patientDetail.MiddleName,
                Surname = patientDetail.Surname,
                DateOfBirth = patientDetail.DateOfBirth,
            };
            newPatient.SetPatientFacility(facility);
            newPatient.SetPatientStatus(patientStatus);

            // Custom Property handling
            _typeExtensionHandler.UpdateExtendable(newPatient, patientDetail.CustomAttributes, "Admin");

            // Clinical data
            AddConditions(newPatient, patientDetail.Conditions);
            AddLabTests(newPatient, patientDetail.LabTests);
            AddMedications(newPatient, patientDetail.Medications);
            AddClinicalEvents(newPatient, patientDetail.ClinicalEvents);

            // Other data
            AddAttachments(newPatient, patientDetail.Attachments);

            if(patientDetail.CohortGroupId > 0)
            {
                AddCohortEnrollment(newPatient, patientDetail);
            }

            _patientRepository.Save(newPatient);

            // Register encounter
            if(patientDetail.EncounterTypeId > 0)
            {
                AddEncounter(newPatient, new EncounterDetail() { 
                    EncounterDate = patientDetail.EncounterDate, 
                    EncounterTypeId = patientDetail.EncounterTypeId, 
                    PatientId = newPatient.Id, 
                    PriorityId = patientDetail.PriorityId });
            }

            return newPatient.Id;
        }

        /// <summary>
        /// Update an existing patient in the repository
        /// </summary>
        /// <param name="patientDetail">The details of the patient to add</param>
        public void UpdatePatient(PatientDetailForUpdate patientDetail)
        {
            var identifier_asm = patientDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == "Medical Record Number")?.Value.ToString();

            if(String.IsNullOrWhiteSpace(identifier_asm))
            {
                throw new Exception("Unable to locate patient in repo for update");
            }

            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();
            parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_asm });

            var patientFromRepo = GetPatientUsingAttributes(parameters);
            if (patientFromRepo == null)
            {
                throw new ArgumentException(nameof(patientDetail));
            }

            if(!String.IsNullOrWhiteSpace(patientDetail.FirstName))
            {
                patientFromRepo.FirstName = patientDetail.FirstName;
            }

            if (!String.IsNullOrWhiteSpace(patientDetail.Surname))
            {
                patientFromRepo.Surname = patientDetail.Surname;
            }

            if (!String.IsNullOrWhiteSpace(patientDetail.MiddleName))
            {
                patientFromRepo.MiddleName = patientDetail.MiddleName;
            }

            if(patientDetail.DateOfBirth.HasValue)
            {
                patientFromRepo.DateOfBirth = patientDetail.DateOfBirth;
            }

            // Custom Property handling
            _typeExtensionHandler.UpdateExtendable(patientFromRepo, patientDetail.CustomAttributes, "Admin");

            // Clinical data
            AddConditions(patientFromRepo, patientDetail.Conditions);
            AddLabTests(patientFromRepo, patientDetail.LabTests);
            AddMedications(patientFromRepo, patientDetail.Medications);
            AddClinicalEvents(patientFromRepo, patientDetail.ClinicalEvents);

            // Other data
            AddAttachments(patientFromRepo, patientDetail.Attachments);

            _patientRepository.Update(patientFromRepo);

            // Register encounter
            if (patientDetail.EncounterTypeId > 0)
            {
                AddEncounter(patientFromRepo, new EncounterDetail() { 
                    EncounterDate = patientDetail.EncounterDate, 
                    EncounterTypeId = patientDetail.EncounterTypeId, 
                    PatientId = patientFromRepo.Id, 
                    PriorityId = patientDetail.PriorityId 
                });
            }
        }

        /// <summary>
        /// Register patient encounter
        /// </summary>
        public int AddEncounter(Patient patient, EncounterDetail encounterDetail)
        {
            var encounterType = _encounterTypeRepository.Get(et => et.Id == encounterDetail.EncounterTypeId);
            if (encounterType == null)
            {
                throw new ArgumentException(nameof(encounterDetail.EncounterTypeId));
            }

            var priority = _priorityRepository.Get(p => p.Id == encounterDetail.PriorityId);
            if (priority == null)
            {
                throw new ArgumentException(nameof(encounterDetail.PriorityId));
            }

            var newEncounter = new Encounter(patient)
            {
                EncounterType = encounterType,
                Priority = priority,
                EncounterDate = encounterDetail.EncounterDate,
                Notes = encounterDetail.Notes
            };

            //newEncounter.AuditStamp(user);
            _encounterRepository.Save(newEncounter);

            var encounterTypeWorkPlan = _encounterTypeWorkPlanRepository.Get(et => et.EncounterType.Id == encounterType.Id);
            if (encounterTypeWorkPlan != null)
            {
                // Create a new instance
                var dataset = _datasetRepository.Get(d => d.Id == encounterTypeWorkPlan.WorkPlan.Dataset.Id);
                if (dataset != null)
                {
                    var datasetInstance = dataset.CreateInstance(newEncounter.Id, encounterTypeWorkPlan);
                    _datasetInstanceRepository.Save(datasetInstance);
                }
            }

            return newEncounter.Id;
        }

        /// <summary>
        /// Check patient's custom attributes to ensure they are unique within the repository
        /// </summary>
        /// <param name="parameters">The list of custom attributes that should be checked</param>
        public bool isUnique(List<CustomAttributeParameter> parameters, int patientId = 0)
        {
            if(parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if(parameters.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameters));
            }

            string sql = "SELECT Id FROM Patient p CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y) WHERE ";
            sql += $"p.Id != {patientId} AND (";

            foreach(var parameter in parameters)
            {
                sql += $"X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeKey.Trim()}' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeValue.Trim()}' ";
                sql += "OR ";
            }
            sql = sql.Substring(0, sql.Length - 3);
            sql += ")";

            return _unitOfWork.Repository<ScalarInt>().ExecuteSqlScalar(sql, new SqlParameter[0]) == 0;
        }

        /// <summary>
        /// Check patient's custom attributes to ensure they exist within the repository
        /// </summary>
        /// <param name="parameters">The list of custom attributes that should be checked</param>
        public bool Exists(List<CustomAttributeParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameters));
            }

            string sql = "SELECT Id FROM Patient p CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y) WHERE ";

            foreach (var parameter in parameters)
            {
                sql += $"X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeKey.Trim()}' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeValue.Trim()}' ";
                sql += "OR ";
            }
            sql = sql.Substring(0, sql.Length - 3);

            return _unitOfWork.Repository<ScalarInt>().ExecuteSqlScalar(sql, new SqlParameter[0]) > 0;
        }

        /// <summary>
        /// Fetch a patient record using the supplied attributes
        /// </summary>
        /// <param name="parameters">The list of custom attributes that should be checked</param>
        public Patient GetPatientUsingAttributes(List<CustomAttributeParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameters));
            }

            string sql = "SELECT Id FROM Patient p CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y) WHERE ";

            foreach (var parameter in parameters)
            {
                sql += $"X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeKey.Trim()}' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)') = '{parameter.AttributeValue.Trim()}' ";
                sql += "OR ";
            }
            sql = sql.Substring(0, sql.Length - 3);

            var patientId = _unitOfWork.Repository<ScalarInt>().ExecuteSqlScalar(sql, new SqlParameter[0]);
            if(patientId == 0)
            {
                return null;
            }
            return _patientRepository.Get(p => p.Id == patientId);
        }

        /// <summary>
        /// Prepare patient record with associated conditions
        /// </summary>
        private void AddConditions(Patient patient, List<ConditionDetail> conditions)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (conditions == null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }
            if (conditions.Count == 0)
            {
                return;
            }

            foreach (var condition in conditions)
            {
                var treatmentOutcome = !String.IsNullOrWhiteSpace(condition.TreatmentOutcome) ? _unitOfWork.Repository<TreatmentOutcome>().Get(to => to.Description == condition.TreatmentOutcome) : null;
                var terminologyMedDra = condition.MeddraTermId != null ? _unitOfWork.Repository<TerminologyMedDra>().Get(tm => tm.Id == condition.MeddraTermId) : null;

                var patientCondition = new PatientCondition
                {
                    Patient = patient,
                    ConditionSource = condition.ConditionSource,
                    TerminologyMedDra = terminologyMedDra,
                    DateStart = Convert.ToDateTime(condition.DateStart),
                    TreatmentOutcome = treatmentOutcome
                };

                // Custom Property handling
                _typeExtensionHandler.UpdateExtendable(patientCondition, condition.CustomAttributes, "Admin");

                patient.PatientConditions.Add(patientCondition);
            }
        }

        /// <summary>
        /// Prepare patient record with associated lab tests
        /// </summary>
        private void AddLabTests(Patient patient, List<LabTestDetail> labTests) 
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (labTests == null)
            {
                throw new ArgumentNullException(nameof(labTests));
            }
            if (labTests.Count == 0)
            {
                return;
            }

            foreach (var labTest in labTests)
            {
                var test = _unitOfWork.Repository<LabTest>().Get(to => to.Description == labTest.LabTestSource);

                var patientLabTest = new PatientLabTest
                {
                    Patient = patient,
                    LabTestSource = labTest.LabTestSource,
                    TestDate = labTest.TestDate,
                    TestResult = labTest.TestResult,
                    LabTest = test
                };

                // Custom Property handling
                _typeExtensionHandler.UpdateExtendable(patientLabTest, labTest.CustomAttributes, "Admin");

                patient.PatientLabTests.Add(patientLabTest);
            }
        }

        /// <summary>
        /// Prepare patient record with associated medications
        /// </summary>
        private void AddMedications(Patient patient, List<MedicationDetail> medications)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (medications == null)
            {
                throw new ArgumentNullException(nameof(medications));
            }
            if (medications.Count == 0)
            {
                return;
            }

            foreach (var medication in medications)
            {
                var patientMedication = new PatientMedication
                {
                    Patient = patient,
                    MedicationSource = medication.MedicationSource,
                    DateStart = medication.DateStart,
                    DateEnd = medication.DateEnd,
                    Dose = medication.Dose,
                    DoseFrequency = medication.DoseFrequency,
                };

                // Custom Property handling
                _typeExtensionHandler.UpdateExtendable(patientMedication, medication.CustomAttributes, "Admin");

                patient.PatientMedications.Add(patientMedication);
            }
        }

        /// <summary>
        /// Prepare patient record with associated clinical events
        /// </summary>
        private void AddClinicalEvents(Patient patient, List<ClinicalEventDetail> clinicalEvents)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (clinicalEvents == null)
            {
                throw new ArgumentNullException(nameof(clinicalEvents));
            }
            if (clinicalEvents.Count == 0)
            {
                return;
            }

            foreach (var clinicalEvent in clinicalEvents)
            {
                var patientClinicalEvent = new PatientClinicalEvent
                {
                    Patient = patient,
                    SourceDescription = clinicalEvent.SourceDescription,
                    OnsetDate = clinicalEvent.OnsetDate,
                    ResolutionDate = clinicalEvent.ResolutionDate
                };

                // Custom Property handling
                _typeExtensionHandler.UpdateExtendable(patientClinicalEvent, clinicalEvent.CustomAttributes, "Admin");

                patient.PatientClinicalEvents.Add(patientClinicalEvent);
            }
        }

        /// <summary>
        /// Prepare patient record with associated attachments
        /// </summary>
        private void AddAttachments(Patient patient, List<AttachmentDetail> attachments)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }
            if (attachments == null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }
            if (attachments.Count == 0)
            {
                return;
            }

            // Handle attachments
            foreach (var sourceAttachment in attachments)
            {
                var attachmentType = _unitOfWork.Repository<AttachmentType>()
                    .Queryable()
                    .SingleOrDefault(u => u.Key == "jpg");

                //var byt = Encoding.ASCII.GetBytes(sourceAttachment.ImageSource);
                //string hash;
                //using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                //{
                //    hash = Convert.ToBase64String(sha1.ComputeHash(byt));
                //}

                // Create the attachment
                var attachment = new Attachment
                {
                    Patient = patient,
                    Description = sourceAttachment.Description,
                    FileName = $"{Guid.NewGuid()}.jpg",
                    AttachmentType = attachmentType,
                    Size = 0,
                    Content = Convert.FromBase64String(sourceAttachment.ImageSource.Replace("data:image/jpeg;base64,", ""))
                };

                patient.Attachments.Add(attachment);
            }
        }

        /// <summary>
        /// Enrol patient into cohort
        /// </summary>
        private void AddCohortEnrollment(Patient patient, PatientDetailForCreation patientDetail)
        {
            var cohortGroup = _cohortGroupRepository.Get(cg => cg.Id == patientDetail.CohortGroupId);
            var enrolment = new CohortGroupEnrolment
            {
                Patient = patient,
                EnroledDate = Convert.ToDateTime(patientDetail.EnroledDate),
                CohortGroup = cohortGroup
            };

            patient.CohortEnrolments.Add(enrolment);
        }
    }
}
