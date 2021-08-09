﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.ReportInstanceAggregate
{
    public class CreateE2BForActiveCommandCommandHandler
        : IRequestHandler<CreateE2BForActiveCommand, bool>
    {
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workFlowService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateE2BForActiveCommandCommandHandler> _logger;

        public CreateE2BForActiveCommandCommandHandler(
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            IWorkFlowService workFlowService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ILogger<CreateE2BForActiveCommandCommandHandler> logger)
        {
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateE2BForActiveCommand message, CancellationToken cancellationToken)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.WorkFlow.WorkFlowGuid == message.WorkFlowGuid
                    && ri.Id == message.ReportInstanceId, new string[] { "CreatedBy", "WorkFlow" });

            if (reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate report instance");
            }

            var activeReport = await _patientClinicalEventRepository.GetAsync(p => p.PatientClinicalEventGuid == reportInstanceFromRepo.ContextGuid, new string[] { "Patient" });
            if (activeReport == null)
            {
                throw new KeyNotFoundException("Unable to locate active report");
            }

            var dataset = await GetDatasetForInstance();
            if (dataset == null)
            {
                throw new KeyNotFoundException("Unable to locate E2B dataset");
            }

            var newActivityExecutionStatusEvent = await _workFlowService.ExecuteActivityAsync(reportInstanceFromRepo.ContextGuid, "E2BINITIATED", "AUTOMATION: E2B dataset created", null, "");

            var e2bInstance = dataset.CreateInstance(newActivityExecutionStatusEvent.Id, "Active", null, null, activeReport);
            await _datasetInstanceRepository.SaveAsync(e2bInstance);

            UpdateValuesUsingSource(e2bInstance, activeReport, dataset);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- E2B instance created for active report {message.ReportInstanceId} created");

            return true;
        }

        private void UpdateValuesUsingSource(DatasetInstance e2bInstance, PatientClinicalEvent activeReport, Dataset dataset)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);

            if (dataset.DatasetName.Contains("(R2)"))
            {
                SetInstanceValuesForActiveRelease2(e2bInstance, activeReport);
            }
            if (dataset.DatasetName.Contains("(R3)"))
            {
                var term = _workFlowService.GetTerminologyMedDraForReportInstance(activeReport.PatientClinicalEventGuid);

                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.2 Batch Number"), "MSH.PViMS-B01000" + activeReport.Id.ToString());
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.5 Date of Batch Transmission"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.1 Message Identifier"), "MSH.PViMS-B01000" + activeReport.Id.ToString() + "-" + DateTime.Now.ToString("mmsss"));
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.4 Date of Message Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.1 Sender’s (case) Safety Report Unique Identifier"), String.Format("ZA-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), activeReport.Id.ToString()));
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.2 Date of Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.8.1 Worldwide Unique Case Identification Number"), String.Format("ZA-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), activeReport.Id.ToString()));
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.2.1b MedDRA Code for Reaction / Event"), term.DisplayName);
            }
        }

        private async Task<Dataset> GetDatasetForInstance()
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.E2BVersion);
            if (config == null)
            {
                throw new KeyNotFoundException("Unable to locate E2BVersion configuration");
            }
            var datasetName = config.ConfigValue;
            return await _datasetRepository.GetAsync(d => d.DatasetName == datasetName, 
                new string[] { "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType", "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType" });
        }

        private void SetInstanceValuesForActiveRelease2(DatasetInstance e2bInstance, PatientClinicalEvent activeReport)
        {
            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == activeReport.PatientClinicalEventGuid);

            // ************************************* ichicsrmessageheader
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "7FF710CB-C08C-4C35-925E-484B983F2135"), e2bInstance.Id.ToString("D8")); // Message Number
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "693614B6-D5D5-457E-A03B-EAAFA66E6FBD"), DateTime.Today.ToString("yyyyMMddhhmmss")); // Message Date

            // ************************************* safetyreport
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "6799CAD0-2A65-48A5-8734-0090D7C2D85E"), string.Format("PH.FDA.{0}", reportInstance.Id.ToString("D6"))); //Safety Report ID
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "9C92D382-03AF-4A52-9A2F-04A46ADA0F7E"), DateTime.Today.ToString("yyyyMMdd")); //Transmission Date 
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "AE53FEB2-FF27-4CD5-AD54-C3FFED1490B5"), "2"); //Report Type

            IExtendable activeReportExtended = activeReport;
            var objectValue = activeReportExtended.GetAttributeValue("Is the adverse event serious?");
            var serious = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(serious))
            {
                var selectionValue = _unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Is the adverse event serious?" && sdi.SelectionKey == serious).Value;
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), selectionValue == "Yes" ? "1=Yes" : "2=No"); //Serious
            }

            objectValue = activeReportExtended.GetAttributeValue("Seriousness");
            var seriousReason = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(seriousReason) && serious == "1")
            {
                var selectionValue = _unitOfWork.Repository<SelectionDataItem>().Queryable().Single(si => si.AttributeKey == "Seriousness" && si.SelectionKey == seriousReason).Value;

                var sd = "2=No";
                var slt = "2=No";
                var sh = "2=No";
                var sdi = "2=No";
                var sca = "2=No";
                var so = "2=No";

                switch (selectionValue)
                {
                    case "Death":
                        sd = "1=Yes";
                        break;

                    case "Life threatening":
                        slt = "1=Yes";
                        break;

                    case "A congenital anomaly or birth defect":
                        sca = "1=Yes";
                        break;

                    case "Initial or prolonged hospitalization":
                        sh = "1=Yes";
                        break;

                    case "Persistent or significant disability or incapacity":
                        sdi = "1=Yes";
                        break;

                    case "A medically important event":
                        so = "1=Yes";
                        break;

                    default:
                        break;
                }

                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B4EA6CBF-2D9C-482D-918A-36ABB0C96EFA"), sd); //Seriousness Death
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "26C6F08E-B80B-411E-BFDC-0506FE102253"), slt); //Seriousness Life Threatening
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "837154A9-D088-41C6-A9E2-8A0231128496"), sh); //Seriousness Hospitalization
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DDEBDEC0-2A90-49C7-970E-B7855CFDF19D"), sdi); //Seriousness Disabling
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DF89C98B-1D2A-4C8E-A753-02E265841F4F"), sca); //Seriousness Congenital Anomaly
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "33A75547-EF1B-42FB-8768-CD6EC52B24F8"), so); //Seriousness Other
            }

            objectValue = activeReportExtended.GetAttributeValue("Date of Report");
            var reportDate = objectValue != null ? objectValue.ToString() : "";
            DateTime tempdt;
            if (DateTime.TryParse(reportDate, out tempdt))
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "65ADEF15-961A-4558-B864-7832D276E0E3"), Convert.ToDateTime(reportDate).ToString("yyyyMMdd")); //Date report was first received
            }
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "A10C704D-BC1D-445E-B084-9426A91DB63B"), DateTime.Today.ToString("yyyyMMdd")); //Date of most recent info

            // ************************************* primarysource
            objectValue = activeReportExtended.GetAttributeValue("Name of reporter");
            var fullName = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(fullName))
            {
                if (fullName.Contains(" "))
                {
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C35D5F5A-D539-4EEE-B080-FF384D5FBE08"), fullName.Substring(0, fullName.IndexOf(" "))); //Reporter Given Name
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F214C619-EE0E-433E-8F52-83469778E418"), fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1))); //Reporter Family Name
                }
                else
                {
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C35D5F5A-D539-4EEE-B080-FF384D5FBE08"), fullName); //Reporter Given Name
                }
            }

            objectValue = activeReportExtended.GetAttributeValue("Profession");
            var profession = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(profession))
            {
                var selectionValue = _unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Profession" && sdi.SelectionKey == profession).Value;

                switch (selectionValue)
                {
                    case "Other health professional":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "3=Other Health Professional"); //Qualification
                        break;

                    case "Physician":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "1=Physician");
                        break;

                    case "Consumer or other non health professional":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "5=Consumer or other non health professional");
                        break;

                    case "Pharmacist":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "2=Pharmacist");
                        break;

                    case "Lawyer":
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "4=Lawyer");
                        break;

                    default:
                        break;
                }
            }

            // ************************************* sender
            var regAuth = _unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.RegulatoryAuthority);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Type"), "2=Regulatory Authority");
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Organization"), regAuth.OrganisationName);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Given Name"), regAuth.ContactFirstName);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Family Name"), regAuth.ContactSurname);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Street Address"), regAuth.StreetAddress);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender City"), regAuth.City);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender State"), regAuth.State);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Postcode"), regAuth.PostCode);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Number"), regAuth.ContactNumber);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Country Code"), regAuth.CountryCode);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Email Address"), regAuth.ContactEmail);

            // ************************************* receiver
            var repAuth = _unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.ReportingAuthority);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Type"), "5=WHO Collaborating Center for International Drug Monitoring");
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Organization"), repAuth.OrganisationName);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Given Name"), repAuth.ContactFirstName);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Family Name"), repAuth.ContactSurname);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Street Address"), repAuth.StreetAddress);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver City"), repAuth.City);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver State"), repAuth.State);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Postcode"), repAuth.PostCode);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel"), repAuth.ContactNumber);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel Country Code"), repAuth.CountryCode);
            e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Email Address"), repAuth.ContactEmail);

            // ************************************* patient
            var init = String.Format("{0}{1}", activeReport.Patient.FirstName.Substring(0, 1), activeReport.Patient.Surname.Substring(0, 1));
            if (!String.IsNullOrWhiteSpace(init)) { e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "A0BEAB3A-0B0A-457E-B190-1B66FE60CA73"), init); }; //Patient Initial

            var dob = activeReport.Patient.DateOfBirth;
            var onset = activeReport.OnsetDate;
            var recovery = activeReport.ResolutionDate;
            if (dob != null)
            {
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "4F71B7F4-4317-4680-B3A3-9C1C1F72AD6A"), Convert.ToDateTime(dob).ToString("yyyyMMdd")); //Patient Birthdate

                if (onset != null)
                {
                    var age = (Convert.ToDateTime(onset) - Convert.ToDateTime(dob)).Days;
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "E10C259B-DD2C-4F19-9D41-16FDDF9C5807"), age.ToString()); //Patient Onset Age
                    e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "CA9B94C2-E1EF-407B-87C3-181224AF637A"), "804=Day"); //Patient Onset Age Unit
                }
            }

            var encounter = _unitOfWork.Repository<Encounter>().Queryable().FirstOrDefault(e => e.Patient.Id == activeReport.Patient.Id && e.Archived == false & e.EncounterDate <= activeReport.OnsetDate);
            if (encounter != null)
            {
                var encounterInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().SingleOrDefault(ds => ds.Dataset.DatasetName == "Chronic Treatment" && ds.ContextId == encounter.Id);
                if (encounterInstance != null)
                {
                    var weight = encounterInstance.GetInstanceValue("Weight (kg)");
                    if (!String.IsNullOrWhiteSpace(weight)) { e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "89A6E687-A220-4319-AAC1-AFBB55C81873"), weight); }; //Patient Weight

                    var height = encounterInstance.GetInstanceValue("Height (cm)");
                    if (!String.IsNullOrWhiteSpace(height)) { e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "40DAD435-8282-4B3E-B65E-3478FF55D028"), height); }; //Patient Height

                    var lmp = encounterInstance.GetInstanceValue("Date of last menstrual period");
                    if (!String.IsNullOrWhiteSpace(lmp)) { e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "93253F91-60D1-4161-AF1A-F3ABDD140CB9"), Convert.ToDateTime(lmp).ToString("yyyyMMdd")); }; //Patient Last Menstrual Date

                    var gest = encounterInstance.GetInstanceValue("Estimated gestation (weeks)");
                    if (!String.IsNullOrWhiteSpace(gest))
                    {
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B6BE9689-B6B2-4FCF-8918-664AFC91A4E0"), gest); //Gestation Period
                        e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1F174413-2A1E-45BD-B5C4-0C8F5DFFBFF4"), "803=Week");  //Gestation Period Unit
                    };
                }
            }

            // ************************************* reaction
            var terminologyMedDra = _workFlowService.GetTerminologyMedDraForReportInstance(activeReport.PatientClinicalEventGuid);
            var term = terminologyMedDra != null ? terminologyMedDra.DisplayName : "";
            if (!String.IsNullOrWhiteSpace(term)) { e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C8DD9A5E-BD9A-488D-8ABF-171271F5D370"), term); }; //Reaction MedDRA LLT
            if (onset != null) { e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1EAD9E11-60E6-4B27-9A4D-4B296B169E90"), Convert.ToDateTime(onset).ToString("yyyyMMdd")); }; //Reaction Start Date
            if (recovery != null) { e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "3A0F240E-8B36-48F6-9527-77E55F6E7CF1"), Convert.ToDateTime(recovery).ToString("yyyyMMdd")); }; // Reaction End Date
            if (onset != null && recovery != null)
            {
                var rduration = (Convert.ToDateTime(recovery) - Convert.ToDateTime(onset)).Days;
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "0712C664-2ADD-44C0-B8D5-B6E83FB01F42"), rduration.ToString()); //Reaction Duration
                e2bInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F96E702D-DCC5-455A-AB45-CAEFF25BF82A"), "804=Day"); //Reaction Duration Unit
            }

            // ************************************* test
            var destinationTestElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.DatasetElementGuid.ToString() == "693A2E8C-B041-46E7-8687-0A42E6B3C82E"); // Test History
            foreach (PatientLabTest labTest in activeReport.Patient.PatientLabTests.Where(lt => lt.TestDate >= activeReport.OnsetDate).OrderByDescending(lt => lt.TestDate))
            {
                var newContext = Guid.NewGuid();

                e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Date"), labTest.TestDate.ToString("yyyyMMdd"), (Guid)newContext);
                e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Name"), labTest.LabTest.Description, (Guid)newContext);

                var testResult = !String.IsNullOrWhiteSpace(labTest.LabValue) ? labTest.LabValue : !String.IsNullOrWhiteSpace(labTest.TestResult) ? labTest.TestResult : "";
                e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Result"), testResult, (Guid)newContext);

                var testUnit = labTest.TestUnit != null ? labTest.TestUnit.Description : "";
                if (!String.IsNullOrWhiteSpace(testUnit)) { e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Unit"), testUnit, (Guid)newContext); };

                var lowRange = labTest.ReferenceLower;
                if (!String.IsNullOrWhiteSpace(lowRange)) { e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Low Test Range"), lowRange, (Guid)newContext); };

                var highRange = labTest.ReferenceUpper;
                if (!String.IsNullOrWhiteSpace(highRange)) { e2bInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "High Test Range"), highRange, (Guid)newContext); };
            }

            // ************************************* drug
            string[] validNaranjoCriteria = { "Possible", "Probable", "Definite" };
            string[] validWHOCriteria = { "Possible", "Probable", "Certain" };

            var destinationProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.DatasetElementGuid.ToString() == "E033BDE8-EDC8-43FF-A6B0-DEA6D6FA581C"); // Medicinal Products
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                var newContext = Guid.NewGuid();

                var patientMedication = _unitOfWork.Repository<PatientMedication>().Queryable().Single(pm => pm.PatientMedicationGuid == med.ReportInstanceMedicationGuid);
                IExtendable mcExtended = patientMedication;

                var character = "";
                character = (validNaranjoCriteria.Contains(med.NaranjoCausality) || validWHOCriteria.Contains(med.WhoCausality)) ? "1=Suspect" : "2=Concomitant";
                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Characterization"), character, (Guid)newContext);

                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Medicinal Product"), patientMedication.Concept.ConceptName, (Guid)newContext);

                objectValue = mcExtended.GetAttributeValue("Batch Number");
                var batchNumber = objectValue != null ? objectValue.ToString() : "";
                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Batch Number"), batchNumber, (Guid)newContext);

                objectValue = mcExtended.GetAttributeValue("Comments");
                var comments = objectValue != null ? objectValue.ToString() : "";
                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Additional Information"), comments, (Guid)newContext);

                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Dosage Text"), patientMedication.Dose, (Guid)newContext);

                var form = patientMedication?.Concept?.MedicationForm.Description;
                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Dosage Form"), form, (Guid)newContext);

                var startdate = patientMedication.StartDate;
                e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Start Date"), startdate.ToString("yyyyMMdd"), (Guid)newContext);

                var enddate = patientMedication.EndDate;
                if (enddate.HasValue)
                {
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug End Date"), Convert.ToDateTime(enddate).ToString("yyyyMMdd"), (Guid)newContext);

                    var rduration = (Convert.ToDateTime(enddate) - Convert.ToDateTime(startdate)).Days;
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration"), rduration.ToString(), (Guid)newContext);
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration Unit"), "804=Day", (Guid)newContext);
                }

                var doseUnit = MapDoseUnitForActive(patientMedication.DoseUnit);
                if (!string.IsNullOrWhiteSpace(doseUnit)) { e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Structured Dosage Unit"), doseUnit, (Guid)newContext); };

                objectValue = mcExtended.GetAttributeValue("Clinician action taken with regard to medicine if related to AE");
                var drugAction = objectValue != null ? objectValue.ToString() : "";
                if (!string.IsNullOrWhiteSpace(drugAction)) { drugAction = MapDrugActionForActive(drugAction); };
                if (!string.IsNullOrWhiteSpace(drugAction)) { e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Action"), doseUnit, (Guid)newContext); };

                // Causality
                if (med.WhoCausality != null)
                {
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Reaction Assessment"), activeReport.SourceDescription, (Guid)newContext);
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Method of Assessment"), "WHO Causality Scale", (Guid)newContext);
                    e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), med.WhoCausality.ToLowerInvariant() == "ignored" ? "" : med.WhoCausality, (Guid)newContext);
                }
                else
                {
                    if (med.NaranjoCausality != null)
                    {
                        e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Reaction Assessment"), activeReport.SourceDescription, (Guid)newContext);
                        e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Method of Assessment"), "Naranjo Causality Scale", (Guid)newContext);
                        e2bInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), med.NaranjoCausality.ToLowerInvariant() == "ignored" ? "" : med.NaranjoCausality, (Guid)newContext);
                    }
                }
            } // foreach (ReportInstanceMedication med in reportInstance.Medications)

        } // end of sub

        private string MapDoseUnitForActive(string doseUnit)
        {
            switch (doseUnit)
            {
                case "Bq":
                    return "014=Bq becquerel(s)";

                case "Ci":
                    return "018=Ci curie(s)";

                case "{DF}":
                    return "032=DF dosage form";

                case "[drp]":
                    return "031=Gtt drop(s)";

                case "GBq":
                    return "015=GBq gigabecquerel(s)";

                case "g":
                    return "002=G gram(s)";

                case "[iU]":
                    return "025=Iu international unit(s)";

                case "[iU]/kg":
                    return "028=iu/kg iu/kilogram";

                case "kBq":
                    return "017=Kbq kilobecquerel(s)";

                case "kg":
                    return "001=kg kilogram(s)";

                case "k[iU]":
                    return "026=Kiu iu(1000s)";

                case "L":
                    return "011=l litre(s)";

                case "MBq":
                    return "016=MBq megabecquerel(s)";

                case "M[iU]":
                    return "027=Miu iu(1,000,000s)";

                case "uCi":
                    return "020=uCi microcurie(s)";

                case "ug":
                    return "004=ug microgram(s)";

                case "ug/kg":
                    return "007=mg/kg milligram(s)/kilogram";

                case "uL":
                    return "013=ul microlitre(s)";

                case "mCi":
                    return "019=MCi millicurie(s)";

                case "meq":
                    return "029=Meq milliequivalent(s)";

                case "mg":
                    return "003=Mg milligram(s)";

                case "mg/kg":
                    return "007=mg/kg milligram(s)/kilogram";

                case "mg/m2":
                    return "009=mg/m 2 milligram(s)/sq. meter";

                case "ug/m2":
                    return "010=ug/ m 2 microgram(s)/ sq. Meter";

                case "mL":
                    return "012=ml millilitre(s)";

                case "mmol":
                    return "023=Mmol millimole(s)";

                case "mol":
                    return "022=Mol mole(s)";

                case "nCi":
                    return "021=NCi nanocurie(s)";

                case "ng":
                    return "005=ng nanogram(s)";

                case "%":
                    return "030=% percent";

                case "pg":
                    return "006=pg picogram(s)";

                default:
                    break;
            }

            return "";
        }

        private string MapDrugActionForActive(string drugAction)
        {
            if (!String.IsNullOrWhiteSpace(drugAction))
            {
                var selectionValue = _unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Clinician action taken with regard to medicine if related to AE" && sdi.SelectionKey == drugAction).Value;

                switch (selectionValue)
                {
                    case "Dose not changed":
                        return "4=Dose not changed";

                    case "Dose reduced":
                        return "2=Dose reduced";

                    case "Drug interrupted":
                        return "5=Unknown";

                    case "Drug withdrawn":
                        return "1=Drug withdrawn";

                    case "Not applicable":
                        return "6=Not applicable";

                    default:
                        break;
                }
            } // if (!String.IsNullOrWhiteSpace(drugAction))

            return "";
        }
    }
}