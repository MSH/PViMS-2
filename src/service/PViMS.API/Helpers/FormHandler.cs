using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using VPS.Common.Repositories;

namespace PVIMS.API.Helpers
{
    public class FormHandler
    {
        private readonly IPatientService _patientService;
        private readonly IWorkFlowService _workflowService;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<DatasetElement> _datasetElementRepository;
        private readonly IUnitOfWorkInt _unitOfWork;

        private List<Dictionary<string, string>> _formValues = new List<Dictionary<string, string>>();
        private List<Dictionary<string, string>[]> _formArrayValues = new List<Dictionary<string, string>[]>();

        private List<string> _validationErrors { get; set; } = new List<string>();

        FormForCreationDto _formForCreation;
        private PatientDetailForCreation _patientDetailForCreation;
        private PatientDetailForUpdate _patientDetailForUpdate;

        public FormHandler(IPatientService patientService,
            IWorkFlowService workflowService,
            IRepositoryInt<DatasetElement> datasetElementRepository,
            ITypeExtensionHandler modelExtensionBuilder,
            IUnitOfWorkInt unitOfWork)
        {
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _workflowService = workflowService ?? throw new ArgumentNullException(nameof(workflowService));
            _datasetElementRepository = datasetElementRepository ?? throw new ArgumentNullException(nameof(datasetElementRepository));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void SetForm(FormForCreationDto formForCreation)
        {
            _formForCreation = formForCreation;
            var formControlValues = formForCreation.FormValues.Select(fv => fv.FormControlValue).ToList();
            foreach (var formValue in formControlValues)
            {
                if (formValue.StartsWith("["))
                {
                    // Handle array
                    _formArrayValues.Add(JsonConvert.DeserializeObject<Dictionary<string, string>[]>(formValue));
                }
                else
                {
                    // Handle object
                    _formValues.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(formValue));
                }
            }

            _patientDetailForCreation = null;
            _patientDetailForUpdate = null;
        }

        public void SetSpontaneousForm(Object[] formControlValues)
        {
            foreach (Object formValue in formControlValues)
            {
                if (formValue is JObject)
                {
                    _formValues.Add(((JObject)formValue).ToObject<Dictionary<string, string>>());
                }
                if (formValue is JArray)
                {
                    var tempArray = (JArray)formValue;
                    var outputArray = new List<Dictionary<string, string>>();

                    foreach (JObject content in tempArray.Children<JObject>())
                    {
                        outputArray.Add(content.ToObject<Dictionary<string, string>>());
                    }

                    _formArrayValues.Add(outputArray.ToArray());
                }
            }
        }

        public void ValidateSourceIdentifier()
        {
            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();

            var identifier_asm = GetAttributeValueFromObject(1, "asmNumber");

            // Refactor
            switch (_formForCreation.FormType)
            {
                case "FormA":
                    var identifier_id = GetAttributeValueFromObject(1, "patientIdentityNumber");

                    // Ensure patient record does not exist
                    parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_asm });
                    parameters.Add(new CustomAttributeParameter() { AttributeKey = "Patient Identity Number", AttributeValue = identifier_id });

                    if (!_patientService.isUnique(parameters))
                    {
                        _validationErrors.Add("Unable to synchronise as patient already exists");
                    }

                    break;

                case "FormB":
                    // Ensure patient record exists
                    parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_asm });

                    if (!_patientService.Exists(parameters))
                    {
                        _validationErrors.Add("Unable to synchronise as patient cannot be found");
                    }

                    break;

                case "FormC":
                    // Ensure patient record exists
                    parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_asm });

                    if (!_patientService.Exists(parameters))
                    {
                        _validationErrors.Add("Unable to synchronise as patient cannot be found");
                    }

                    break;

                default:
                    break;
            }
        }

        public void PreparePatientAndClinicalDetail()
        {
            switch (_formForCreation.FormType)
            {
                case "FormA":
                    PreparePatientDetailsFromFormA();

                    break;

                case "FormB":
                    PreparePatientDetailsFromFormB();

                    break;

                case "FormC":
                    PreparePatientDetailsFromFormC();

                    break;

                default:
                    break;
            }
        }

        public void ProcessFormForCreationOrUpdate()
        {
            if(_validationErrors.Count > 0)
            {
                throw new Exception("Unable to process form as there are validation errors");
            }
            if(_patientDetailForCreation == null && _patientDetailForUpdate == null)
            {
                throw new Exception("Unable to process form as patient detail is not prepared");
            }

            if (_patientDetailForCreation != null)
            {
                _patientService.AddPatient(_patientDetailForCreation);
            }
            if (_patientDetailForUpdate != null)
            {
                _patientService.UpdatePatient(_patientDetailForUpdate);
            }
        }

        public void ProcessSpontaneousFormForCreation(Dataset datasetFromRepo)
        {
            var datasetInstance = datasetFromRepo.CreateInstance(1, null);

            datasetInstance.Status = DatasetInstanceStatus.COMPLETE;

            var datasetElementIds = datasetFromRepo.DatasetCategories.SelectMany(dc => dc.DatasetCategoryElements.Select(dce => dce.DatasetElement.Id)).ToArray();

            var datasetElements = _unitOfWork.Repository<DatasetElement>()
                .Queryable()
                .Where(de => datasetElementIds.Contains(de.Id))
                .ToDictionary(e => e.ElementName);

            var datasetElementSubs = datasetElements
                .SelectMany(de => de.Value.DatasetElementSubs)
                .ToDictionary(des => des.ElementName);

            // Save patient info
            datasetInstance.SetInstanceValue(datasetElements["Initials"], GetAttributeValueFromObject(0, "initials"));
            datasetInstance.SetInstanceValue(datasetElements["Identification Number"], GetAttributeValueFromObject(0, "identification"));
            datasetInstance.SetInstanceValue(datasetElements["Identification Type"], GetAttributeValueFromObject(0, "identificationType"));
            datasetInstance.SetInstanceValue(datasetElements["Date of Birth"], GetAttributeValueFromObject(0, "birthDate"));
            datasetInstance.SetInstanceValue(datasetElements["Age"], GetAttributeValueFromObject(0, "age"));
            datasetInstance.SetInstanceValue(datasetElements["Age Unit"], GetAttributeValueFromObject(0, "ageUnitOfMeasure"));
            datasetInstance.SetInstanceValue(datasetElements["Weight  (kg)"], GetAttributeValueFromObject(0, "weight"));
            datasetInstance.SetInstanceValue(datasetElements["Sex"], GetAttributeValueFromObject(0, "sex"));
            datasetInstance.SetInstanceValue(datasetElements["Ethnic Group"], GetAttributeValueFromObject(0, "ethnic"));

            // Test results
            var rowCount = GetRowCountFromArray(0);
            if (rowCount > 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    // Create context for lab test
                    var context = Guid.NewGuid();

                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Test Date"], GetAttributeValueFromArrayRow(0, i, "testResultDate"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Test Name"], GetAttributeValueFromArrayRow(0, i, "labTest"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Test Result"], GetAttributeValueFromArrayRow(0, i, "testResultValue"), context);
                }
            }

            // Product information
            rowCount = GetRowCountFromArray(1);
            if (rowCount > 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    // Create context for medication
                    var context = Guid.NewGuid();

                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Product"], GetAttributeValueFromArrayRow(1, i, "product"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Product Suspected"], GetAttributeValueFromArrayRow(1, i, "suspected"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Drug Strength"], GetAttributeValueFromArrayRow(1, i, "strength"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Drug Strength Unit"], GetAttributeValueFromArrayRow(1, i, "strengthUnit"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Dose Number"], GetAttributeValueFromArrayRow(1, i, "dose"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Dose Unit"], GetAttributeValueFromArrayRow(1, i, "doseUnit"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Drug route of administration"], GetAttributeValueFromArrayRow(1, i, "route"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Drug Start Date"], GetAttributeValueFromArrayRow(1, i, "startDate"), context);
                    datasetInstance.SetInstanceSubValue(datasetElementSubs["Drug End Date"], GetAttributeValueFromArrayRow(1, i, "endDate"), context);
                }
            }

            // Reaction and treatment
            datasetInstance.SetInstanceValue(datasetElements["Description of reaction"], GetAttributeValueFromObject(1, "reaction"));
            datasetInstance.SetInstanceValue(datasetElements["Reaction start date"], GetAttributeValueFromObject(1, "startDate"));
            datasetInstance.SetInstanceValue(datasetElements["Reaction estimated start date"], GetAttributeValueFromObject(1, "estimatedStartDate"));
            datasetInstance.SetInstanceValue(datasetElements["Reaction serious details"], GetAttributeValueFromObject(1, "reactionHappen"));
            datasetInstance.SetInstanceValue(datasetElements["Treatment given for reaction"], GetAttributeValueFromObject(1, "treatmentGiven"));
            datasetInstance.SetInstanceValue(datasetElements["Treatment given for reaction details"], GetAttributeValueFromObject(1, "whatTreatment"));
            datasetInstance.SetInstanceValue(datasetElements["Outcome of reaction"], GetAttributeValueFromObject(1, "treatmentOutcome"));
            datasetInstance.SetInstanceValue(datasetElements["Reaction date of recovery"], GetAttributeValueFromObject(1, "recoveryDate"));
            datasetInstance.SetInstanceValue(datasetElements["Reaction date of death"], GetAttributeValueFromObject(1, "deceasedDate"));
            datasetInstance.SetInstanceValue(datasetElements["Reaction other relevant info"], GetAttributeValueFromObject(1, "otherInfo"));

            // Reporter information
            datasetInstance.SetInstanceValue(datasetElements["Reporter Name"], GetAttributeValueFromObject(2, "reporter"));
            datasetInstance.SetInstanceValue(datasetElements["Reporter Telephone Number"], GetAttributeValueFromObject(2, "telephoneNumber"));
            datasetInstance.SetInstanceValue(datasetElements["Reporter Profession"], GetAttributeValueFromObject(2, "profession"));
            datasetInstance.SetInstanceValue(datasetElements["Report Reference Number"], GetAttributeValueFromObject(2, "reference"));
            datasetInstance.SetInstanceValue(datasetElements["Reporter Place of Practice"], GetAttributeValueFromObject(2, "place"));
            datasetInstance.SetInstanceValue(datasetElements["Keep Reporter Confidential"], GetAttributeValueFromObject(2, "confidential"));
            datasetInstance.SetInstanceValue(datasetElements["Reporter E-mail Address"], GetAttributeValueFromObject(2, "email"));

            _unitOfWork.Repository<DatasetInstance>().Save(datasetInstance);

            // Instantiate new instance of work flow
            var patientIdentifier = datasetInstance.GetInstanceValue("Identification Number");
            if (String.IsNullOrWhiteSpace(patientIdentifier))
            {
                patientIdentifier = datasetInstance.GetInstanceValue("Initials");
            }
            var sourceIdentifier = datasetInstance.GetInstanceValue("Description of reaction");
            _workflowService.CreateWorkFlowInstance("New Spontaneous Surveilliance Report", datasetInstance.DatasetInstanceGuid, patientIdentifier, sourceIdentifier);

            // Prepare medications
            List<ReportInstanceMedicationListItem> medications = new List<ReportInstanceMedicationListItem>();
            var sourceProductElement = _datasetElementRepository.Get(u => u.ElementName == "Product Information");
            var destinationProductElement = _datasetElementRepository.Get(u => u.ElementName == "Medicinal Products");
            var sourceContexts = datasetInstance.GetInstanceSubValuesContext("Product Information");
            foreach (Guid sourceContext in sourceContexts)
            {
                var drugItemValues = datasetInstance.GetInstanceSubValues("Product Information", sourceContext);
                var drugName = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue;

                if (drugName != string.Empty)
                {
                    var item = new ReportInstanceMedicationListItem()
                    {
                        MedicationIdentifier = drugName,
                        ReportInstanceMedicationGuid = sourceContext
                    };
                    medications.Add(item);
                }
            }
            _workflowService.AddOrUpdateMedicationsForWorkFlowInstance(datasetInstance.DatasetInstanceGuid, medications);

            _unitOfWork.Complete();
        }

        public List<string> GetValidationErrors()
        {
            return _validationErrors;
        }

        private string GetAttributeValueFromObject(int attributeArray, string attributeKey)
        {
            //throw new Exception($"value length: { JsonConvert.SerializeObject(_formValues.First())}");

            if (attributeArray > _formValues.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(attributeArray));
            }

            var formValues = _formValues[attributeArray];

            if (formValues.ContainsKey(attributeKey))
            {
                return formValues[attributeKey] ?? "";
            }
            else
            {
                throw new ArgumentNullException(nameof(attributeKey));
            }
        }

        private int GetRowCountFromArray(int attributeArray)
        {
            if (attributeArray > _formArrayValues.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(attributeArray));
            }

            var formArray = _formArrayValues[attributeArray];

            return formArray.Length;
        }

        private string GetAttributeValueFromArrayRow(int attributeArray, int row, string attributeKey)
        {
            if (attributeArray > _formArrayValues.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(attributeArray));
            }

            var formArray = _formArrayValues[attributeArray];
            if (row > formArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            var formRow = formArray[row];

            if (formRow.ContainsKey(attributeKey))
            {
                return formRow[attributeKey] ?? "";
            }
            else
            {
                throw new ArgumentNullException(attributeKey);
                //throw new ArgumentNullException(nameof(attributeKey));
            }
        }

        private void PreparePatientDetailsFromFormA()
        {
            _patientDetailForCreation = new PatientDetailForCreation();
            _patientDetailForCreation.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<Patient>();

            // Prepare patient first class
            _patientDetailForCreation.CurrentFacilityName = GetAttributeValueFromObject(1, "treatmentSiteId");
            _patientDetailForCreation.FirstName = GetAttributeValueFromObject(1, "patientFirstName");
            _patientDetailForCreation.Surname = GetAttributeValueFromObject(1, "patientLastName");
            _patientDetailForCreation.DateOfBirth = String.IsNullOrWhiteSpace(GetAttributeValueFromObject(1, "birthDate")) ? (DateTime?)null : Convert.ToDateTime(GetAttributeValueFromObject(1, "birthDate"));

            // Prepare patient attributes
            _patientDetailForCreation.SetAttributeValue("Medical Record Number", GetAttributeValueFromObject(1, "asmNumber"));
            _patientDetailForCreation.SetAttributeValue("Gender", TransformToGender(GetAttributeValueFromObject(1, "gender")));
            _patientDetailForCreation.SetAttributeValue("Address", GetAttributeValueFromObject(1, "address"));
            _patientDetailForCreation.SetAttributeValue("Patient Contact Number", GetAttributeValueFromObject(1, "contactNumber"));
            _patientDetailForCreation.SetAttributeValue("Patient Identity Number", GetAttributeValueFromObject(1, "patientIdentityNumber"));

            // Clinical
            _patientDetailForCreation.Conditions.AddRange(PrepareConditionDetail(0));
            _patientDetailForCreation.LabTests.AddRange(PrepareLabTestDetail(1));
            _patientDetailForCreation.Medications.AddRange(PrepareMedicationDetail());

            // Attachments
            _patientDetailForCreation.Attachments.Add(new AttachmentDetail()
            {
                Description = _formForCreation.FormIdentifier,
                ImageSource = _formForCreation.Attachment
            });

            if (_formForCreation.HasSecondAttachment)
            {
                _patientDetailForCreation.Attachments.Add(new AttachmentDetail()
                {
                    Description = _formForCreation.FormIdentifier,
                    ImageSource = _formForCreation.Attachment_2
                });
            }

            // Encounter
            _patientDetailForCreation.EncounterTypeId = 1;
            _patientDetailForCreation.EncounterDate = String.IsNullOrWhiteSpace(GetAttributeValueFromObject(2, "currentDate")) ? DateTime.Today : Convert.ToDateTime(GetAttributeValueFromObject(2, "currentDate"));
            _patientDetailForCreation.PriorityId = 1;

            if (!_patientDetailForCreation.IsValid())
            {
                _patientDetailForCreation.InvalidAttributes.ForEach(element => _validationErrors.Add(element));
            }
        }

        private void PreparePatientDetailsFromFormB()
        {
            var identifier_asm = GetAttributeValueFromObject(1, "asmNumber");

            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();
            parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_asm });

            var patientFromRepo = _patientService.GetPatientUsingAttributes(parameters);
            if (patientFromRepo == null)
            {
                throw new Exception($"Unable to locate patient record for {identifier_asm}");
            }
            VPS.CustomAttributes.IExtendable patientExtended = patientFromRepo;

            _patientDetailForUpdate = new PatientDetailForUpdate();
            _patientDetailForUpdate.CustomAttributes = _modelExtensionBuilder.BuildModelExtension(patientExtended);

            // Prepare patient first class
            _patientDetailForUpdate.FirstName = GetAttributeValueFromObject(1, "patientFirstName");
            _patientDetailForUpdate.Surname = GetAttributeValueFromObject(1, "patientLastName");
            _patientDetailForUpdate.DateOfBirth = String.IsNullOrWhiteSpace(GetAttributeValueFromObject(1, "birthDate")) ? (DateTime?)null : Convert.ToDateTime(GetAttributeValueFromObject(1, "birthDate"));

            // Prepare patient attributes
            _patientDetailForUpdate.SetAttributeValue("Medical Record Number", GetAttributeValueFromObject(1, "asmNumber"));
            if (!String.IsNullOrWhiteSpace(GetAttributeValueFromObject(1, "patientIdentityNumber")))
            {
                _patientDetailForUpdate.SetAttributeValue("Patient Identity Number", GetAttributeValueFromObject(1, "patientIdentityNumber"));
            }
            if (!String.IsNullOrWhiteSpace(GetAttributeValueFromObject(1, "patientIdentityNumber")))
            {
                _patientDetailForUpdate.SetAttributeValue("Patient Identity Number", GetAttributeValueFromObject(1, "patientIdentityNumber"));
            }
            if (TransformToGender(GetAttributeValueFromObject(1, "gender")) != "0")
            {
                _patientDetailForUpdate.SetAttributeValue("Gender", TransformToGender(GetAttributeValueFromObject(1, "gender")));
            }

            // Clinical
            _patientDetailForUpdate.LabTests.AddRange(PrepareLabTestDetail(3));
            _patientDetailForUpdate.ClinicalEvents.AddRange(PrepareClinicalEventDetail(1));

            // Attachments
            _patientDetailForUpdate.Attachments.Add(new AttachmentDetail()
            {
                Description = _formForCreation.FormIdentifier,
                ImageSource = _formForCreation.Attachment
            });

            if (_formForCreation.HasSecondAttachment)
            {
                _patientDetailForUpdate.Attachments.Add(new AttachmentDetail()
                {
                    Description = _formForCreation.FormIdentifier,
                    ImageSource = _formForCreation.Attachment_2
                });
            }

            // Encounter
            _patientDetailForUpdate.EncounterTypeId = 2;
            _patientDetailForUpdate.EncounterDate = String.IsNullOrWhiteSpace(GetAttributeValueFromObject(6, "currentDate")) ? DateTime.Today : Convert.ToDateTime(GetAttributeValueFromObject(6, "currentDate"));
            _patientDetailForUpdate.PriorityId = 1;

            if (!_patientDetailForUpdate.IsValid())
            {
                _patientDetailForUpdate.InvalidAttributes.ForEach(element => _validationErrors.Add(element));
            }
        }

        private void PreparePatientDetailsFromFormC()
        {
            var identifier_asm = GetAttributeValueFromObject(1, "asmNumber");

            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();
            parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_asm });

            var patientFromRepo = _patientService.GetPatientUsingAttributes(parameters);
            if (patientFromRepo == null)
            {
                throw new Exception($"Unable to locate patient record for {identifier_asm}");
            }
            VPS.CustomAttributes.IExtendable patientExtended = patientFromRepo;

            _patientDetailForUpdate = new PatientDetailForUpdate();
            _patientDetailForUpdate.CustomAttributes = _modelExtensionBuilder.BuildModelExtension(patientExtended);

            // Attachments
            _patientDetailForUpdate.Attachments.Add(new AttachmentDetail()
            {
                Description = _formForCreation.FormIdentifier,
                ImageSource = _formForCreation.Attachment
            });

            if (_formForCreation.HasSecondAttachment)
            {
                _patientDetailForUpdate.Attachments.Add(new AttachmentDetail()
                {
                    Description = _formForCreation.FormIdentifier,
                    ImageSource = _formForCreation.Attachment_2
                });
            }

            // Encounter
            _patientDetailForUpdate.EncounterTypeId = 3;
            _patientDetailForUpdate.EncounterDate = String.IsNullOrWhiteSpace(GetAttributeValueFromObject(4, "currentDate")) ? DateTime.Today : Convert.ToDateTime(GetAttributeValueFromObject(4, "currentDate"));
            _patientDetailForUpdate.PriorityId = 1;

            if (!_patientDetailForUpdate.IsValid())
            {
                _patientDetailForUpdate.InvalidAttributes.ForEach(element => _validationErrors.Add(element));
            }
        }

        private List<ConditionDetail> PrepareConditionDetail(int attributeArray)
        {
            List<ConditionDetail> conditions = new List<ConditionDetail>();
            var rowCount = GetRowCountFromArray(attributeArray);
            if (rowCount > 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    var conditionDetail = new ConditionDetail();
                    conditionDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientCondition>();

                    // Prepare first class
                    conditionDetail.DateStart = DateTime.Today;
                    conditionDetail.ConditionSource = GetAttributeValueFromArrayRow(attributeArray, i, "condition");
                    conditionDetail.TreatmentOutcome = GetAttributeValueFromArrayRow(attributeArray, i, "conditionStatus");

                    // Prepare attributes
                    conditionDetail.SetAttributeValue("Condition Ongoing", GetAttributeValueFromArrayRow(attributeArray, i, "conditionStatus") == "Continues" ? "1" : "2");

                    conditions.Add(conditionDetail);
                }
            }
            return conditions;
        }

        private List<LabTestDetail> PrepareLabTestDetail(int attributeArray)
        {
            List<LabTestDetail> labTests = new List<LabTestDetail>();
            var rowCount = GetRowCountFromArray(attributeArray);
            if (rowCount > 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    var labTestDetail = new LabTestDetail();
                    labTestDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientLabTest>();

                    // Prepare first class
                    labTestDetail.TestDate = Convert.ToDateTime(GetAttributeValueFromArrayRow(attributeArray, i, "testResultDate"));
                    labTestDetail.LabTestSource = GetAttributeValueFromArrayRow(attributeArray, i, "labTest");
                    labTestDetail.TestResult = GetAttributeValueFromArrayRow(attributeArray, i, "testResultValue");

                    labTests.Add(labTestDetail);
                }
            }
            return labTests;
        }

        private List<MedicationDetail> PrepareMedicationDetail()
        {
            List<MedicationDetail> medications = new List<MedicationDetail>();
            var rowCount = GetRowCountFromArray(2);
            if (rowCount > 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    var medicationDetail = new MedicationDetail();
                    medicationDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientMedication>();

                    // Prepare first class
                    medicationDetail.DateStart = Convert.ToDateTime(GetAttributeValueFromArrayRow(2, i, "startDate"));
                    medicationDetail.MedicationSource = GetAttributeValueFromArrayRow(2, i, "medication");
                    medicationDetail.DateEnd = String.IsNullOrWhiteSpace(GetAttributeValueFromArrayRow(2, i, "endDate")) ? (DateTime?)null : Convert.ToDateTime(GetAttributeValueFromArrayRow(2, i, "endDate"));
                    medicationDetail.Dose = GetAttributeValueFromArrayRow(2, i, "dose");
                    medicationDetail.DoseFrequency = GetAttributeValueFromArrayRow(2, i, "frequency");

                    medications.Add(medicationDetail);
                }
            }
            return medications;
        }

        private List<ClinicalEventDetail> PrepareClinicalEventDetail(int attributeArray)
        {
            List<ClinicalEventDetail> clinicalEvents = new List<ClinicalEventDetail>();
            var rowCount = GetRowCountFromArray(attributeArray);
            if (rowCount > 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    var clinicalEventDetail = new ClinicalEventDetail();
                    clinicalEventDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientClinicalEvent>();

                    // Prepare first class
                    clinicalEventDetail.SourceDescription = GetAttributeValueFromArrayRow(attributeArray, i, "adverseEvent");
                    clinicalEventDetail.OnsetDate = Convert.ToDateTime(GetAttributeValueFromArrayRow(attributeArray, i, "startDate"));
                    clinicalEventDetail.ResolutionDate = String.IsNullOrWhiteSpace(GetAttributeValueFromArrayRow(attributeArray, i, "endDate")) ? (DateTime?)null : Convert.ToDateTime(GetAttributeValueFromArrayRow(attributeArray, i, "endDate"));

                    // Prepare attributes
                    clinicalEventDetail.SetAttributeValue("Intensity (Severity)", TransformToSeverity(GetAttributeValueFromArrayRow(attributeArray, i, "gravity")));
                    clinicalEventDetail.SetAttributeValue("Is the adverse event serious?", TransformToYesNo(GetAttributeValueFromArrayRow(attributeArray, i, "serious")));
                    clinicalEventDetail.SetAttributeValue("Seriousness", TransformToSeriousness(GetAttributeValueFromArrayRow(attributeArray, i, "severity")));

                    clinicalEvents.Add(clinicalEventDetail);
                }
            }
            return clinicalEvents;
        }

        private string TransformToGender(string source)
        {
            return (source == "M") ? "1" : (source == "F") ? "2" : "0";
        }

        private string TransformToYesNo(string source)
        {
            return (source == "Yes") ? "1" : (source == "No") ? "2" : "0";
        }

        private string TransformToSeverity(string source)
        {
            switch (source)
            {
                case "Mild":
                    return "1";

                case "Moderate":
                    return "2";

                case "Severe":
                    return "3";

                default:
                    return "0";
            }
        }

        private string TransformToSeriousness(string source)
        {
            switch (source)
            {
                case "Hospitalisation":
                    return "4";

                case "Prolonged hospital stay":
                    return "5";

                case "Permanent disability":
                    return "2";

                case "Congenital malformations":
                    return "1";

                case "Life risk":
                    return "6";

                case "Death":
                    return "3";

                default:
                    return "0";
            }
        }

    }
}
