using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.Utilities;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace PVIMS.Services
{
    public class ArtefactService : IArtefactService
    {
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;

        private readonly IHostingEnvironment _environment;

        public ICustomAttributeService _attributeService { get; set; }
        public IPatientService _patientService { get; set; }

        public ArtefactService(IUnitOfWorkInt unitOfWork,
            ICustomAttributeService attributeService,
            IPatientService patientService,
            IHostingEnvironment environment,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public ArtefactInfoModel CreateActiveDatasetForDownload(long[] patientIds, long cohortGroupId)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            model.FileName = $"ActiveDataExtract_{generatedDate}.xlsx";

            using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            {
                // *************************************
                // Create sheet - Patient
                // *************************************
                var ws = pck.Workbook.Worksheets.Add("Patient");
                ws.View.ShowGridLines = true;

                var rowCount = 1;
                var colCount = 1;

                var patientquery = _unitOfWork.Repository<Patient>().Queryable().Where(p => p.Archived == false);
                if (patientIds.Length > 0)
                {
                    patientquery = patientquery.Where(p => patientIds.Contains(p.Id));
                }
                if (cohortGroupId > 0)
                {
                    patientquery = patientquery.Where(p => p.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }

                var patients = patientquery.OrderBy(p => p.Id).ToList();
                foreach (Patient patient in patients)
                {
                    ProcessEntity(patient, ref ws, ref rowCount, ref colCount, "Patient");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientMedication
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientMedication");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var medicationquery = _unitOfWork.Repository<PatientMedication>().Queryable().Where(pm => pm.Archived == false);
                if (patientIds.Length > 0)
                {
                    medicationquery = medicationquery.Where(pm => patientIds.Contains(pm.Patient.Id));
                }
                if (cohortGroupId > 0)
                {
                    medicationquery = medicationquery.Where(pm => pm.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var medications = medicationquery.OrderBy(pm => pm.Id).ToList();
                foreach (PatientMedication medication in medications)
                {
                    ProcessEntity(medication, ref ws, ref rowCount, ref colCount, "PatientMedication");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientClinicalEvent
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientClinicalEvent");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var eventquery = _unitOfWork.Repository<PatientClinicalEvent>().Queryable().Where(pc => pc.Archived == false);
                if (patientIds.Length > 0)
                {
                    eventquery = eventquery.Where(pc => patientIds.Contains(pc.Patient.Id));
                }
                if (cohortGroupId > 0)
                {
                    eventquery = eventquery.Where(pc => pc.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var events = eventquery.OrderBy(pc => pc.Id).ToList();
                foreach (PatientClinicalEvent clinicalEvent in events)
                {
                    ProcessEntity(clinicalEvent, ref ws, ref rowCount, ref colCount, "PatientClinicalEvent");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientCondition
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientCondition");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var conditionquery = _unitOfWork.Repository<PatientCondition>().Queryable().Where(pc => pc.Archived == false);
                if (patientIds.Length > 0)
                {
                    conditionquery = conditionquery.Where(pc => patientIds.Contains(pc.Patient.Id));
                }
                if (cohortGroupId > 0)
                {
                    conditionquery = conditionquery.Where(pc => pc.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var conditions = conditionquery.OrderBy(pc => pc.Id).ToList();
                foreach (PatientCondition condition in conditions)
                {
                    ProcessEntity(condition, ref ws, ref rowCount, ref colCount, "PatientCondition");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientLabTest
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientLabTest");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var labtestquery = _unitOfWork.Repository<PatientLabTest>().Queryable().Where(pl => pl.Archived == false);
                if (patientIds.Length > 0)
                {
                    labtestquery = labtestquery.Where(pl => patientIds.Contains(pl.Patient.Id));
                }
                if (cohortGroupId > 0)
                {
                    labtestquery = labtestquery.Where(pl => pl.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var labTests = labtestquery.OrderBy(pl => pl.Id).ToList();
                foreach (PatientLabTest labTest in labTests)
                {
                    ProcessEntity(labTest, ref ws, ref rowCount, ref colCount, "PatientLabTest");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - Encounter
                // *************************************
                ws = pck.Workbook.Worksheets.Add("Encounter");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var encounterquery = _unitOfWork.Repository<Encounter>().Queryable().Where(e => e.Archived == false);
                if (patientIds.Length > 0)
                {
                    encounterquery = encounterquery.Where(e => patientIds.Contains(e.Patient.Id));
                }
                if (cohortGroupId > 0)
                {
                    encounterquery = encounterquery.Where(e => e.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var encounters = encounterquery.OrderBy(e => e.Id).ToList();
                foreach (Encounter encounter in encounters)
                {
                    ProcessEntity(encounter, ref ws, ref rowCount, ref colCount, "Encounter");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - CohortGroupEnrolment
                // *************************************
                ws = pck.Workbook.Worksheets.Add("CohortGroupEnrolment");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var enrolmentquery = _unitOfWork.Repository<CohortGroupEnrolment>().Queryable().Where(e => e.Archived == false);
                if (patientIds.Length > 0)
                {
                    enrolmentquery = enrolmentquery.Where(e => patientIds.Contains(e.Patient.Id));
                }
                if (cohortGroupId > 0)
                {
                    enrolmentquery = enrolmentquery.Where(e => e.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var enrolments = enrolmentquery.OrderBy(e => e.Id).ToList();
                foreach (CohortGroupEnrolment enrolment in enrolments)
                {
                    ProcessEntity(enrolment, ref ws, ref rowCount, ref colCount, "CohortGroupEnrolment");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - PatientFacility
                // *************************************
                ws = pck.Workbook.Worksheets.Add("PatientFacility");
                ws.View.ShowGridLines = true;

                rowCount = 1;
                colCount = 1;

                var facilityquery = _unitOfWork.Repository<PatientFacility>().Queryable().Where(pf => pf.Archived == false);
                if (patientIds.Length > 0)
                {
                    facilityquery = facilityquery.Where(pf => patientIds.Contains(pf.Patient.Id));
                }
                if (cohortGroupId > 0)
                {
                    facilityquery = facilityquery.Where(pf => pf.Patient.CohortEnrolments.Any(cge => cge.CohortGroup.Id == cohortGroupId));
                }
                var facilities = facilityquery.OrderBy(pf => pf.Id).ToList();
                foreach (PatientFacility facility in facilities)
                {
                    ProcessEntity(facility, ref ws, ref rowCount, ref colCount, "PatientFacility");
                }
                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                pck.Save();
            }

            return model;
        }

        public async Task<ArtefactInfoModel> CreateDatasetInstanceForDownloadAsync(long datasetInstanceId)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            model.FileName = $"InstanceDataExtract_{generatedDate}.xlsx";

            using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            {
                // Create XLS
                var ws = pck.Workbook.Worksheets.Add("Spontaneous ID " + datasetInstanceId);
                ws.View.ShowGridLines = true;

                // Write headers
                ws.Cells["A1"].Value = "Dataset Name";
                ws.Cells["B1"].Value = "Dataset Category";
                ws.Cells["C1"].Value = "Element Name";
                ws.Cells["D1"].Value = "Field Type";
                ws.Cells["E1"].Value = "Value";

                //Set the first header and format it
                using (var r = ws.Cells["A1:E1"])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                }

                // Write content
                var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.Id == datasetInstanceId);
                if (datasetInstance == null)
                {
                    throw new KeyNotFoundException(nameof(datasetInstance));
                }

                var count = 1;
                // Loop through and render main table
                foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description != "Table").OrderBy(div2 => div2.Id))
                {
                    count += 1;
                    ws.Cells["A" + count].Value = datasetInstance.Dataset.DatasetName;
                    ws.Cells["B" + count].Value = value.DatasetElement.DatasetCategoryElements.Single(dce => dce.DatasetCategory.Dataset.Id == datasetInstance.Dataset.Id).DatasetCategory.DatasetCategoryName;
                    ws.Cells["C" + count].Value = value.DatasetElement.ElementName;
                    ws.Cells["D" + count].Value = value.DatasetElement.Field.FieldType.Description;
                    ws.Cells["E" + count].Value = value.InstanceValue;
                };

                // Loop through and render sub tables
                var maxcount = 5;
                var subcount = 1;
                foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description == "Table").OrderBy(div2 => div2.Id))
                {
                    count += 2;
                    ws.Cells["A" + count].Value = datasetInstance.Dataset.DatasetName;
                    ws.Cells["B" + count].Value = value.DatasetElement.DatasetCategoryElements.Single(dce => dce.DatasetCategory.Dataset.Id == datasetInstance.Dataset.Id).DatasetCategory.DatasetCategoryName;
                    ws.Cells["C" + count].Value = value.DatasetElement.ElementName;
                    ws.Cells["D" + count].Value = value.DatasetElement.Field.FieldType.Description;
                    ws.Cells["E" + count].Value = string.Empty;

                    if (value.DatasetInstanceSubValues.Count > 0)
                    {
                        // Write headers
                        count += 1;
                        foreach (var subElement in value.DatasetElement.DatasetElementSubs.Where(des1 => des1.System == false).OrderBy(des2 => des2.Id))
                        {
                            ws.Cells[GetExcelColumnName(subcount) + count].Value = subElement.ElementName;
                            subcount++;
                            maxcount = subcount > maxcount ? subcount : maxcount;
                        }

                        //Set the sub header and format it
                        using (var r = ws.Cells["A" + count + ":" + GetExcelColumnName(subcount) + count])
                        {
                            r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                            r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                        }

                        // Get unique contexts
                        var contexts = datasetInstance.GetInstanceSubValuesContext(value.DatasetElement.ElementName);
                        foreach (var context in contexts)
                        {
                            count += 1;
                            subcount = 1;
                            foreach (var subvalue in datasetInstance.GetInstanceSubValues(value.DatasetElement.ElementName, context))
                            {
                                subcount = value.DatasetElement.DatasetElementSubs.ToList().IndexOf(subvalue.DatasetElementSub) + 1;
                                ws.Cells[GetExcelColumnName(subcount) + count].Value = subvalue.InstanceValue;
                            }
                        }
                    }
                };

                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(maxcount) + count])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                pck.Save();
            }

            return model;
        }

        public async Task<ArtefactInfoModel> CreateE2BAsync(long datasetInstanceId = 0)
        {
            var sourceReport = await _datasetInstanceRepository.GetAsync(ds => ds.Id == datasetInstanceId);
            if (sourceReport == null)
            {
                throw new KeyNotFoundException(nameof(datasetInstanceId));
            }

            var xmlStructureForDataset = sourceReport.Dataset.DatasetXml;
            if (xmlStructureForDataset == null)
            {
                throw new ArgumentException(nameof(xmlStructureForDataset), "Unable to locate dataset XML structure");
            }

            var e2bXmlDocument = CreateNewE2bXmlDocument();

            e2bXmlDocument = ProcessE2bRootNode(sourceReport, xmlStructureForDataset, e2bXmlDocument);

            var artefactModel = PrepareArtefactModel(datasetInstanceId);
            SaveFormattedXML(e2bXmlDocument, artefactModel);

            return artefactModel;
        }

        private ArtefactInfoModel PrepareArtefactModel(long identifier = 0)
        {
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmsss");

            return new ArtefactInfoModel()
            {
                Path = Path.GetTempPath(),
                FileName = $"E2B{identifier}_{generatedDate}.xml"
            };
        }

        private XmlDocument CreateNewE2bXmlDocument()
        {
            XmlDocument e2bXmlDocument = new XmlDocument();

            XmlDeclaration xmlDeclaration = e2bXmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            e2bXmlDocument.AppendChild(xmlDeclaration);

            XmlDocumentType doctype;
            doctype = e2bXmlDocument.CreateDocumentType("ichicsr", "-//ICHM2//DTD ICH ICSR Vers. 2.1//EN", "ich-icsr-v2.1.dtd", null);
            e2bXmlDocument.AppendChild(doctype);

            return e2bXmlDocument;
        }

        private XmlDocument ProcessE2bRootNode(DatasetInstance sourceReport, DatasetXml xmlStructureForDataset, XmlDocument e2bXmlDocument)
        {
            if (sourceReport == null)
            {
                throw new ArgumentNullException(nameof(sourceReport));
            }
            if (xmlStructureForDataset == null)
            {
                throw new ArgumentNullException(nameof(xmlStructureForDataset));
            }
            if (e2bXmlDocument == null)
            {
                throw new ArgumentNullException(nameof(e2bXmlDocument));
            }

            DatasetXmlNode rootNode = xmlStructureForDataset.ChildrenNodes.SingleOrDefault(c => c.NodeType == NodeType.RootNode);
            if (rootNode == null)
            {
                throw new DomainException($"{nameof(xmlStructureForDataset)} Unable to locate rootnode");
            }

            e2bXmlDocument.AppendChild(ProcessE2bXmlNode(sourceReport, e2bXmlDocument, rootNode)[0]);

            return e2bXmlDocument;
        }

        private XmlNode[] ProcessE2bXmlNode(DatasetInstance sourceReport, XmlDocument e2bXmlDocument, DatasetXmlNode datasetXmlNode, DatasetInstanceSubValue[] subItemValues = null)
        {
            if (datasetXmlNode == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlNode));
            }

            List<XmlNode> xmlNodes = new List<XmlNode>();

            switch (datasetXmlNode.NodeType)
            {
                case NodeType.RootNode:
                case NodeType.StandardNode:

                    var xmlStandardNode = CreateXmlNodeWithAttributes(sourceReport, e2bXmlDocument, datasetXmlNode);

                    if (datasetXmlNode.DatasetElement != null)
                    {
                        xmlStandardNode = SetXmlNodeValueWithDatasetElement(sourceReport, datasetXmlNode, xmlStandardNode);
                    }

                    if (datasetXmlNode.DatasetElementSub != null)
                    {
                        xmlStandardNode = SetXmlNodeValueWithSubValues(sourceReport, datasetXmlNode, xmlStandardNode, subItemValues);
                    }

                    if (datasetXmlNode.ChildrenNodes.Count > 0)
                    {
                        xmlStandardNode = ProcessAllNodeChildren(sourceReport, e2bXmlDocument, datasetXmlNode, xmlStandardNode, subItemValues);
                    }
                    xmlNodes.Add(xmlStandardNode);
                    break;

                case NodeType.RepeatingNode:
                    if (datasetXmlNode.DatasetElement != null)
                    {
                        
                        var sourceContexts = sourceReport.GetInstanceSubValuesContext(datasetXmlNode.DatasetElement.ElementName);
                        foreach (Guid sourceContext in sourceContexts)
                        {
                            var xmlRepeatingNode = CreateXmlNodeWithAttributes(sourceReport, e2bXmlDocument, datasetXmlNode);
                            var values = sourceReport.GetInstanceSubValues(datasetXmlNode.DatasetElement.ElementName, sourceContext);

                            if (datasetXmlNode.ChildrenNodes.Count > 0)
                            {
                                xmlRepeatingNode = ProcessAllNodeChildren(sourceReport, e2bXmlDocument, datasetXmlNode, xmlRepeatingNode, values);
                            }

                            xmlNodes.Add(xmlRepeatingNode);
                        }
                    }
                    break;

                default:
                    break;
            }
            return xmlNodes.ToArray();
        }

        private XmlNode CreateXmlNodeWithAttributes(DatasetInstance sourceReport, XmlDocument e2bXmlDocument, DatasetXmlNode datasetXmlNode)
        {
            if (sourceReport == null)
            {
                throw new ArgumentNullException(nameof(sourceReport));
            }
            if (e2bXmlDocument == null)
            {
                throw new ArgumentNullException(nameof(e2bXmlDocument));
            }

            XmlNode xmlNode = e2bXmlDocument.CreateElement(datasetXmlNode.NodeName, "");
            if (datasetXmlNode.NodeAttributes.Count == 0) return xmlNode;

            foreach (DatasetXmlAttribute datasetXmlAttribute in datasetXmlNode.NodeAttributes)
            {
                if (datasetXmlAttribute.DatasetElement != null)
                {
                    xmlNode.Attributes.Append(CreateXmlAttributeUsingDatasetElement(sourceReport, e2bXmlDocument, datasetXmlAttribute));
                }
                else
                {
                    XmlAttribute xmlAttribute = e2bXmlDocument.CreateAttribute(datasetXmlAttribute.AttributeName);
                    xmlAttribute.InnerText = datasetXmlAttribute.AttributeValue;
                    xmlNode.Attributes.Append(xmlAttribute);
                }
            }

            return xmlNode;
        }

        private XmlNode SetXmlNodeValueWithDatasetElement(DatasetInstance sourceReport, DatasetXmlNode datasetXmlNode, XmlNode xmlNode)
        {
            if (sourceReport == null)
            {
                throw new ArgumentNullException(nameof(sourceReport));
            }
            if (datasetXmlNode == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlNode));
            }
            if (datasetXmlNode.DatasetElement == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlNode.DatasetElement));
            }

            var value = sourceReport.GetInstanceValue(datasetXmlNode.DatasetElement.ElementName);
            if (value?.IndexOf("=") > -1)
            {
                value = value.Substring(0, value.IndexOf("="));
            }
            xmlNode.InnerText = value;

            return xmlNode;
        }

        private XmlNode SetXmlNodeValueWithSubValues(DatasetInstance sourceReport, DatasetXmlNode datasetXmlNode, XmlNode xmlNode, DatasetInstanceSubValue[] subItemValues)
        {
            if (sourceReport == null)
            {
                throw new ArgumentNullException(nameof(sourceReport));
            }
            if (datasetXmlNode == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlNode));
            }
            if (datasetXmlNode.DatasetElementSub == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlNode.DatasetElementSub));
            }

            var subvalue = subItemValues?.SingleOrDefault(siv => siv.DatasetElementSub.Id == datasetXmlNode.DatasetElementSub.Id);
            if (subvalue != null)
            {
                var value = subvalue.InstanceValue;
                if (value.IndexOf("=") > -1)
                {
                    value = value.Substring(0, value.IndexOf("="));
                }
                xmlNode.InnerText = value;
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(datasetXmlNode.DatasetElementSub.DefaultValue))
                {
                    xmlNode.InnerText = datasetXmlNode.DatasetElementSub.DefaultValue;
                }
                else
                {
                    xmlNode.InnerText = string.Empty;
                }
            }

            return xmlNode;
        }

        private XmlNode ProcessAllNodeChildren(DatasetInstance sourceReport, XmlDocument e2bXmlDocument, DatasetXmlNode datasetXmlNode, XmlNode parentXmlNode, DatasetInstanceSubValue[] subItemValues = null)
        {
            if (sourceReport == null)
            {
                throw new ArgumentNullException(nameof(sourceReport));
            }
            if (e2bXmlDocument == null)
            {
                throw new ArgumentNullException(nameof(e2bXmlDocument));
            }
            if (datasetXmlNode == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlNode));
            }
            if (parentXmlNode == null)
            {
                throw new ArgumentNullException(nameof(parentXmlNode));
            }

            foreach (DatasetXmlNode datasetChildXmlNode in datasetXmlNode.ChildrenNodes)
            {
                var childNodes = ProcessE2bXmlNode(sourceReport, e2bXmlDocument, datasetChildXmlNode, subItemValues);
                foreach(var childNode in childNodes)
                {
                    parentXmlNode.AppendChild(childNode);
                }
            }

            return parentXmlNode;
        }

        private XmlAttribute CreateXmlAttributeUsingDatasetElement(DatasetInstance sourceReport, XmlDocument e2bXmlDocument, DatasetXmlAttribute datasetXmlAttribute)
        {
            if (sourceReport == null)
            {
                throw new ArgumentNullException(nameof(sourceReport));
            }
            if (e2bXmlDocument == null)
            {
                throw new ArgumentNullException(nameof(e2bXmlDocument));
            }
            if (datasetXmlAttribute == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlAttribute));
            }
            if (datasetXmlAttribute.DatasetElement == null)
            {
                throw new ArgumentNullException(nameof(datasetXmlAttribute.DatasetElement));
            }

            XmlAttribute xmlAttribute = e2bXmlDocument.CreateAttribute(datasetXmlAttribute.AttributeName);
            var value = sourceReport.GetInstanceValue(datasetXmlAttribute.DatasetElement.ElementName);
            if (value.IndexOf("=") > -1)
            {
                value = value.Substring(0, value.IndexOf("="));
            }
            xmlAttribute.InnerText = value;
            return xmlAttribute;
        }

        private void SaveFormattedXML(XmlDocument e2bXmlDocument, ArtefactInfoModel artefactModel)
        {
            if (e2bXmlDocument == null)
            {
                throw new ArgumentNullException(nameof(e2bXmlDocument));
            }
            if (artefactModel == null)
            {
                throw new ArgumentNullException(nameof(artefactModel));
            }

            WriteXMLContentToFile(artefactModel.FullPath, ConvertXMLToFormattedText(e2bXmlDocument), e2bXmlDocument);
        }

        private string ConvertXMLToFormattedText(XmlDocument sourceDocument)
        {
            if (sourceDocument == null)
            {
                throw new ArgumentNullException(nameof(sourceDocument));
            }

            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
                Encoding = Encoding.UTF8
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                sourceDocument.Save(writer);
            }

            return sb.ToString();
        }

        private void WriteXMLContentToFile(string fileName, string xmlText, XmlDocument sourceDocument)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            if (String.IsNullOrWhiteSpace(xmlText))
            {
                throw new ArgumentNullException(nameof(xmlText));
            }

            using (TextWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                sourceDocument.Save(streamWriter);
            }
        }

        public async Task<ArtefactInfoModel> CreatePatientSummaryForActiveReportAsync(Guid contextGuid)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            var patientClinicalEvent = await _patientClinicalEventRepository.GetAsync(pce => pce.PatientClinicalEventGuid == contextGuid);
            if (patientClinicalEvent == null)
            {
                throw new KeyNotFoundException(nameof(patientClinicalEvent));
            }

            var extendable = (IExtendable)patientClinicalEvent;
            var extendableValue = _attributeService.GetCustomAttributeValue("PatientClinicalEvent", "Is the adverse event serious?", extendable);
            var isSerious = extendableValue == "Yes";

            model.Path = Path.GetTempPath();
            var fileNamePrefix = isSerious ? "SAEReport_Active" : "PatientSummary_Active";
            model.FileName = $"{fileNamePrefix}{patientClinicalEvent.Patient.Id}_{generatedDate}.docx";

            using (var document = WordprocessingDocument.Create(Path.Combine(model.Path, model.FileName), WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = document.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = new Body();

                SectionProperties sectionProps = new SectionProperties();
                PageMargin pageMargin = new PageMargin() { Top = 404, Right = (UInt32Value)504U, Bottom = 404, Left = (UInt32Value)504U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
                sectionProps.Append(pageMargin);
                body.Append(sectionProps);

                mainPart.Document.AppendChild(body);
            };

            using (var document = WordprocessingDocument.Open(Path.Combine(model.Path, model.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                // Add page header
                body.Append(AddPatientSummaryPageHeader(isSerious ? "SERIOUS ADVERSE EVENT" : "PATIENT SUMMARY", document));

                var tableHeader = AddTableHeader("A. BASIC PATIENT INFORMATION");
                body.Append(tableHeader);
                var basicInformationtable = AddBasicInformationTable(patientClinicalEvent);
                body.Append(basicInformationtable);
                var notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("B. PRE-EXISITING CONDITIONS");
                body.Append(tableHeader);
                var conditiontable = AddConditionTable(patientClinicalEvent);
                body.Append(conditiontable);

                tableHeader = AddTableHeader("C. ADVERSE EVENT INFORMATION");
                body.Append(tableHeader);
                var adverseEventtable = AddAdverseEventTable(patientClinicalEvent, isSerious);
                body.Append(adverseEventtable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("D. MEDICATIONS");
                body.Append(tableHeader);
                var medicationtable = AddMedicationTable(patientClinicalEvent);
                body.Append(medicationtable);
                notesTable = AddNotesForMedicationTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("E. CLINICAL EVALUATIONS");
                body.Append(tableHeader);
                var evaluationtable = AddEvaluationTable(patientClinicalEvent);
                body.Append(evaluationtable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("F. WEIGHT HISTORY");
                body.Append(tableHeader);
                var weighttable = AddWeightTable(patientClinicalEvent);
                body.Append(weighttable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                document.Save();
            }

            return model;
        }

        public async Task<ArtefactInfoModel> CreatePatientSummaryForSpontaneousReportAsync(Guid contextGuid)
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.DatasetInstanceGuid == contextGuid);
            if (datasetInstance == null)
            {
                throw new KeyNotFoundException(nameof(datasetInstance));
            }

            var isSerious = !String.IsNullOrWhiteSpace(datasetInstance.GetInstanceValue("Reaction serious details"));

            model.Path = Path.GetTempPath();
            var fileNamePrefix = isSerious ? "SAEReport_Spontaneous" : "PatientSummary_Spontaneous";
            model.FileName = $"{fileNamePrefix}{datasetInstance.Id}_{generatedDate}.docx";

            using (var document = WordprocessingDocument.Create(Path.Combine(model.Path, model.FileName), WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = document.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = new Body();

                SectionProperties sectionProps = new SectionProperties();
                PageMargin pageMargin = new PageMargin() { Top = 404, Right = (UInt32Value)504U, Bottom = 404, Left = (UInt32Value)504U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
                sectionProps.Append(pageMargin);
                body.Append(sectionProps);

                mainPart.Document.AppendChild(body);
            };

            using (var document = WordprocessingDocument.Open(Path.Combine(model.Path, model.FileName), true))
            {
                var doc = document.MainDocumentPart.Document;
                var body = doc.Body;

                // Add page header
                body.Append(AddPatientSummaryPageHeader("PATIENT SUMMARY", document));

                var tableHeader = AddTableHeader("A. BASIC PATIENT INFORMATION");
                body.Append(tableHeader);
                var basicInformationtable = AddBasicInformationTable(datasetInstance);
                body.Append(basicInformationtable);
                var notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("B. ADVERSE EVENT INFORMATION");
                body.Append(tableHeader);
                var adverseEventtable = AddAdverseEventTable(datasetInstance, isSerious);
                body.Append(adverseEventtable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("C. MEDICATIONS");
                body.Append(tableHeader);
                var medicationtable = AddMedicationTable(datasetInstance);
                body.Append(medicationtable);
                notesTable = AddNotesForMedicationTable();
                body.Append(notesTable);

                tableHeader = AddTableHeader("D. CLINICAL EVALUATIONS");
                body.Append(tableHeader);
                var evaluationtable = AddEvaluationTable(datasetInstance);
                body.Append(evaluationtable);
                notesTable = AddNotesTable();
                body.Append(notesTable);

                document.Save();
            }

            return model;
        }

        public ArtefactInfoModel CreateSpontaneousDatasetForDownload()
        {
            var model = new ArtefactInfoModel();
            var generatedDate = DateTime.Now.ToString("yyyyMMddhhmmss");

            model.Path = Path.GetTempPath();
            model.FileName = $"SpontaneousDataExtract_{generatedDate}.xlsx";

            using (var pck = new ExcelPackage(new FileInfo(model.FullPath)))
            {
                // *************************************
                // Create sheet - Main Spontaneous
                // *************************************
                var ws = pck.Workbook.Worksheets.Add("Spontaneous");
                ws.View.ShowGridLines = true;

                var rowCount = 1;
                var colCount = 2;
                var maxColCount = 1;

                List<string> columns = new List<string>();

                // Header
                ws.Cells["A1"].Value = "Unique Identifier";

                var dataset = _unitOfWork.Repository<Dataset>().Queryable().Single(ds => ds.DatasetName == "Spontaneous Report");
                foreach (DatasetCategory category in dataset.DatasetCategories)
                {
                    foreach (DatasetCategoryElement element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description != "Table"))
                    {
                        ws.Cells[GetExcelColumnName(colCount) + "1"].Value = element.DatasetElement.ElementName;
                        columns.Add(element.DatasetElement.ElementName);
                        colCount += 1;
                    }
                }
                maxColCount = colCount - 1;

                //Set the header and format it
                using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + "1"])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                }

                // Data
                foreach (ReportInstance reportInstance in _unitOfWork.Repository<ReportInstance>()
                    .Queryable()
                    .Where(ri => ri.WorkFlow.WorkFlowGuid.ToString() == "4096D0A3-45F7-4702-BDA1-76AEDE41B986" && ri.Activities.Any(a => a.QualifiedName == "Confirm Report Data" && a.CurrentStatus.Description != "DELETED")))
                {
                    DatasetInstance datasetInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);

                    rowCount += 1;
                    ws.Cells["A" + rowCount].Value = datasetInstance.DatasetInstanceGuid.ToString();

                    foreach (var value in datasetInstance.DatasetInstanceValues.Where(div1 => div1.DatasetElement.System == false && div1.DatasetElement.Field.FieldType.Description != "Table").OrderBy(div2 => div2.Id))
                    {
                        colCount = columns.IndexOf(value.DatasetElement.ElementName) + 2;
                        ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = value.InstanceValue;
                    };
                }

                //format row
                using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + rowCount])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    r.AutoFitColumns();
                }

                // *************************************
                // Create sheet - Sub tables
                // *************************************
                foreach (DatasetCategory category in dataset.DatasetCategories)
                {
                    foreach (DatasetCategoryElement element in category.DatasetCategoryElements.Where(dce => dce.DatasetElement.System == false && dce.DatasetElement.Field.FieldType.Description == "Table"))
                    {
                        ws = pck.Workbook.Worksheets.Add(element.DatasetElement.ElementName);
                        ws.View.ShowGridLines = true;

                        // Write headers
                        colCount = 2;
                        rowCount = 1;

                        ws.Cells["A1"].Value = "Unique Identifier";

                        foreach (var subElement in element.DatasetElement.DatasetElementSubs.Where(des1 => des1.System == false).OrderBy(des2 => des2.Id))
                        {
                            ws.Cells[GetExcelColumnName(colCount) + "1"].Value = subElement.ElementName;
                            colCount += 1;
                        }
                        maxColCount = colCount - 1;

                        //Set the header and format it
                        using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + "1"])
                        {
                            r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                            r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                        }

                        // Data
                        foreach (var value in _unitOfWork.Repository<DatasetInstanceValue>()
                            .Queryable()
                            .Where(div1 => div1.DatasetElement.Id == element.DatasetElement.Id && div1.DatasetInstanceSubValues.Count > 0 && div1.DatasetInstance.Status == Core.ValueTypes.DatasetInstanceStatus.COMPLETE).OrderBy(div2 => div2.Id))
                        {
                            // Get report and ensure it is not deleted
                            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == value.DatasetInstance.DatasetInstanceGuid);

                            if (reportInstance != null)
                            {
                                if (reportInstance.Activities.Any(a => a.QualifiedName == "Confirm Report Data" && a.CurrentStatus.Description != "DELETED"))
                                {
                                    // Get unique contexts
                                    var contexts = value.DatasetInstance.GetInstanceSubValuesContext(value.DatasetElement.ElementName);
                                    foreach (var context in contexts)
                                    {
                                        rowCount += 1;
                                        ws.Cells["A" + rowCount].Value = value.DatasetInstance.DatasetInstanceGuid.ToString();

                                        foreach (var subvalue in value.DatasetInstance.GetInstanceSubValues(value.DatasetElement.ElementName, context))
                                        {
                                            if (subvalue.DatasetElementSub.System == false)
                                            {
                                                colCount = value.DatasetElement.DatasetElementSubs.ToList().IndexOf(subvalue.DatasetElementSub) + 2;
                                                ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = subvalue.InstanceValue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //format row
                        using (var r = ws.Cells["A1:" + GetExcelColumnName(maxColCount) + rowCount])
                        {
                            r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Regular));
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            r.AutoFitColumns();
                        }
                    }
                }

                pck.Save();
            }

            return model;
        }

        #region "Word Processing"

        private Paragraph AddPatientSummaryPageHeader(string header, WordprocessingDocument document)
        {
            PrepareLogo(document);

            Paragraph paragraph = new Paragraph();
            ParagraphProperties pprop = new ParagraphProperties();
            Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
            pprop.Append(CenterHeading);
            pprop.ParagraphStyleId = new ParagraphStyleId() { Val = "userheading" };
            paragraph.Append(pprop);

            RunProperties runProperties = new RunProperties();
            runProperties.AppendChild(new Bold());
            FontSize fs = new FontSize();
            fs.Val = "24";
            runProperties.AppendChild(fs);
            Run run = new Run();
            run.AppendChild(runProperties);
            run.AppendChild(new Text(header));
            paragraph.Append(run);

            return paragraph;
        }

        private Table AddTableHeader(string header)
        {
            UInt32Value rowHeight = 20;

            // Main header
            Table mainTable = new Table();

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "11352" })
                    );

            mainTable.AppendChild<TableProperties>(tprops);

            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell(header, 11352, false, true));
            mainTable.AppendChild<TableRow>(tr);

            return mainTable;
        }

        private Table AddBasicInformationTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            var headerWidth = 2500;
            var cellWidth = 3176;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() },
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() })
                    );

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Patient Name", headerWidth));
            tr.Append(PrepareCell(patientEvent.Patient.FullName, cellWidth));
            tr.Append(PrepareHeaderCell("Date of Birth", headerWidth));
            tr.Append(PrepareCell(patientEvent.Patient.DateOfBirth != null ? Convert.ToDateTime(patientEvent.Patient.DateOfBirth).ToString("yyyy-MM-dd") : "", cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Age Group", headerWidth));
            tr.Append(PrepareCell(patientEventExtended.GetAttributeValue("Age Group") != null ? patientEventExtended.GetAttributeValue("Age Group").ToString() : "", cellWidth));
            tr.Append(PrepareHeaderCell("Age at time of onset", headerWidth));
            if (patientEvent.OnsetDate != null && patientEvent.Patient.DateOfBirth != null)
            {
                tr.Append(PrepareCell(CalculateAge(Convert.ToDateTime(patientEvent.Patient.DateOfBirth), Convert.ToDateTime(patientEvent.OnsetDate)).ToString() + " years", cellWidth));
            }
            else
            {
                tr.Append(PrepareCell(string.Empty, cellWidth));
            }
            table.AppendChild<TableRow>(tr);

            // Row 3
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Gender", headerWidth));
            tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue("Patient", "Gender", patientExtended), cellWidth));
            tr.Append(PrepareHeaderCell("Facility", headerWidth));
            tr.Append(PrepareCell(patientEvent.Patient.GetCurrentFacility().Facility.FacilityName, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 4
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Medical Record Number", headerWidth));
            tr.Append(PrepareCell(patientExtended.GetAttributeValue("Medical Record Number") != null ? patientExtended.GetAttributeValue("Medical Record Number").ToString() : "", cellWidth));
            tr.Append(PrepareHeaderCell("Identity Number", headerWidth));
            tr.Append(PrepareCell(patientExtended.GetAttributeValue("Patient Identity Number") != null ? patientExtended.GetAttributeValue("Patient Identity Number").ToString() : "", cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 5
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Weight (kg)", headerWidth));
            tr.Append(PrepareCell("", cellWidth));
            tr.Append(PrepareHeaderCell("Height (cm)", headerWidth));
            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            return table;
        }

        private Table AddConditionTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            var headerWidth = 2500;
            var cellWidth = 8852;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Conditions
            var i = 0;
            foreach (PatientCondition pc in patientEvent.Patient.PatientConditions.Where(pc => pc.OnsetDate <= patientEvent.OnsetDate).OrderByDescending(c => c.OnsetDate))
            {
                i += 1;
                // Row 1
                var tr = new TableRow();
                TableRowProperties rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Condition " + i.ToString(), headerWidth));
                tr.Append(PrepareCell(pc.TerminologyMedDra.MedDraTerm, cellWidth, false));

                table.AppendChild<TableRow>(tr);

                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Start Date", headerWidth));
                tr.Append(PrepareCell(pc.OnsetDate.ToString("yyyy-MM-dd"), cellWidth, false));

                table.AppendChild<TableRow>(tr);

                var endDate = pc.OutcomeDate != null ? Convert.ToDateTime(pc.OutcomeDate).ToString("yyyy-MM-dd") : "";
                // Row 3
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("End Date", headerWidth));
                tr.Append(PrepareCell(endDate, cellWidth, false));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddAdverseEventTable(PatientClinicalEvent patientEvent, bool isSerious)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            var headerWidth = 2500;
            var cellWidth = 3176;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() },
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() })
                    );

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Source Description", headerWidth));
            tr.Append(PrepareCell(patientEvent.SourceDescription, cellWidth));
            tr.Append(PrepareHeaderCell("MedDRA Term", headerWidth));
            tr.Append(PrepareCell(patientEvent.SourceTerminologyMedDra.MedDraTerm, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            var onsetDate = patientEvent.OnsetDate != null ? Convert.ToDateTime(patientEvent.OnsetDate).ToString("yyyy-MM-dd") : "";
            var resDate = patientEvent.ResolutionDate != null ? Convert.ToDateTime(patientEvent.ResolutionDate).ToString("yyyy-MM-dd") : "";
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Onset Date", headerWidth));
            tr.Append(PrepareCell(onsetDate, cellWidth));
            tr.Append(PrepareHeaderCell("Resolution Date", headerWidth));
            tr.Append(PrepareCell(resDate, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 3
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Duration", headerWidth));
            if (!String.IsNullOrWhiteSpace(onsetDate) && !String.IsNullOrWhiteSpace(resDate))
            {
                var rduration = (Convert.ToDateTime(resDate) - Convert.ToDateTime(onsetDate)).Days;
                tr.Append(PrepareCell(rduration.ToString() + " days", cellWidth));
            }
            else
            {
                tr.Append(PrepareCell(string.Empty, cellWidth));
            }
            tr.Append(PrepareHeaderCell("Outcome", headerWidth));
            tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue("PatientClinicalEvent", "Outcome", patientEventExtended), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Cater for seriousness fields
            if (isSerious)
            {
                // Row 4
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Seriousness", headerWidth));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue("PatientClinicalEvent", "Seriousness", patientEventExtended), cellWidth));
                tr.Append(PrepareHeaderCell("Grading Scale", headerWidth));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue("PatientClinicalEvent", "Severity Grading Scale", patientEventExtended), cellWidth));

                table.AppendChild<TableRow>(tr);

                // Row 5
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Severity Grade", headerWidth));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue("PatientClinicalEvent", "Severity Grade", patientEventExtended), cellWidth));
                tr.Append(PrepareHeaderCell("SAE Number", headerWidth));
                tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue("PatientClinicalEvent", "FDA SAE Number", patientEventExtended), cellWidth));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddMedicationTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "3852" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            var reportInstance = _reportInstanceRepository.Get(ri => ri.ContextGuid == patientEvent.PatientClinicalEventGuid);
            if (reportInstance == null) { return table; };

            var i = 0;
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                i += 1;

                var patientMedication = _patientMedicationRepository.Get(pm => pm.PatientMedicationGuid == med.ReportInstanceMedicationGuid);
                if (patientMedication != null)
                {
                    IExtendable mcExtended = patientMedication;

                    // Row 1
                    var endDate = patientMedication.EndDate.HasValue ? patientMedication.EndDate.Value.ToString("yyyy-MM-dd") : "";
                    var tr = new TableRow();
                    TableRowProperties rprops = new TableRowProperties(
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareHeaderCell("Drug " + i.ToString(), 2500));
                    tr.Append(PrepareHeaderCell("Start Date ", 1250, true, false));
                    tr.Append(PrepareHeaderCell("End Date ", 1250, true, false));
                    tr.Append(PrepareHeaderCell("Dose ", 1250, true, false));
                    tr.Append(PrepareHeaderCell("Route ", 1250, true, false));
                    tr.Append(PrepareHeaderCell("Indication ", 3852));

                    table.AppendChild<TableRow>(tr);

                    // Row 2
                    tr = new TableRow();
                    rprops = new TableRowProperties(
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareCell(patientMedication.DisplayName, 2500, false));
                    tr.Append(PrepareCell(patientMedication.StartDate.ToString("yyyy-MM-dd"), 1250));
                    tr.Append(PrepareCell(endDate, 1250));
                    tr.Append(PrepareCell(patientMedication.Dose, 1250));
                    tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue("PatientMedication", "Route", mcExtended), 1250));
                    tr.Append(PrepareCell(_attributeService.GetCustomAttributeValue("PatientMedication", "Indication", mcExtended), 3852, false));

                    table.AppendChild<TableRow>(tr);

                    // Row 3
                    tr = new TableRow();
                    rprops = new TableRowProperties(
                        new HorizontalMerge { Val = MergedCellValues.Continue },
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareHeaderCell("C.1 CLINICIAN ACTION TAKEN WITH REGARD TO MEDICINE", 11352, false, false, 6));

                    table.AppendChild<TableRow>(tr);

                    // Row 4
                    tr = new TableRow();
                    rprops = new TableRowProperties(
                        new HorizontalMerge { Val = MergedCellValues.Continue },
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareCell("", 11352, false, 6));

                    table.AppendChild<TableRow>(tr);

                    // Row 5
                    tr = new TableRow();
                    rprops = new TableRowProperties(
                        new HorizontalMerge { Val = MergedCellValues.Continue },
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareHeaderCell("C.2 EFFECT OF DECHALLENGE/ RECHALLENGE", 11352, false, false, 6));

                    table.AppendChild<TableRow>(tr);

                    // Row 6
                    tr = new TableRow();
                    rprops = new TableRowProperties(
                        new HorizontalMerge { Val = MergedCellValues.Continue },
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareCell("", 11352, false, 6));

                    table.AppendChild<TableRow>(tr);
                }
            }

            return table;
        }

        private Table AddEvaluationTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "6352" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Test", 2500));
            tr.Append(PrepareHeaderCell("Test Date", 2500, true, false));
            tr.Append(PrepareHeaderCell("Test Result", 6352));

            table.AppendChild<TableRow>(tr);

            foreach (PatientLabTest labTest in patientEvent.Patient.PatientLabTests.Where(lt => lt.TestDate >= patientEvent.OnsetDate).OrderByDescending(lt => lt.TestDate))
            {
                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell(labTest.LabTest.Description, 2500, false));
                tr.Append(PrepareCell(labTest.TestDate.ToString("yyyy-MM-dd"), 2500));
                tr.Append(PrepareCell(labTest.TestResult, 6352, false));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddWeightTable(PatientClinicalEvent patientEvent)
        {
            IExtendable patientEventExtended = patientEvent;
            IExtendable patientExtended = patientEvent.Patient;

            Table table = new Table();

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "8852" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Weight Date", 2500));
            tr.Append(PrepareHeaderCell("Weight", 8852, true, false));

            table.AppendChild<TableRow>(tr);

            var weightSeries = _patientService.GetElementValues(patientEvent.Patient.Id, "Weight(kg)", 10);

            if (weightSeries.Length > 0)
            {
                foreach (var weight in weightSeries[0].Series)
                {
                    // Row 2
                    tr = new TableRow();
                    rprops = new TableRowProperties(
                        new TableRowHeight() { Val = rowHeight }
                        );
                    tr.AppendChild<TableRowProperties>(rprops);

                    tr.Append(PrepareCell(weight.Name, 2500, false));
                    tr.Append(PrepareCell(weight.Value.ToString(), 6352));

                    table.AppendChild<TableRow>(tr);
                }
            }

            return table;
        }

        private Table AddBasicInformationTable(DatasetInstance datasetInstance)
        {
            Table table = new Table();
            DateTime tempdt;

            var temp = datasetInstance.GetInstanceValue("Date of Birth");
            DateTime dob = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            temp = datasetInstance.GetInstanceValue("Age");
            string age = temp;
            temp = datasetInstance.GetInstanceValue("Age Unit");
            age += " " + temp; // Include age unit

            var headerWidth = 2500;
            var cellWidth = 3176;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() },
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() })
                    );

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Patient Name", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue("Initials"), cellWidth));
            tr.Append(PrepareHeaderCell("Date of Birth", headerWidth));
            tr.Append(PrepareCell(dob == DateTime.MinValue ? "" : dob.ToString("yyyy-MM-dd"), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Age Group", headerWidth));
            tr.Append(PrepareCell("", cellWidth));
            tr.Append(PrepareHeaderCell("Age at time of onset", headerWidth));
            tr.Append(PrepareCell(age, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 3
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Gender", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue("Sex"), cellWidth));
            tr.Append(PrepareHeaderCell("Facility", headerWidth));
            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 4
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Medical Record Number", headerWidth));
            tr.Append(PrepareCell("", cellWidth));
            tr.Append(PrepareHeaderCell("Identity Number", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue("Identification Number"), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 5
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Weight (kg)", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue("Weight  (kg)"), cellWidth));
            tr.Append(PrepareHeaderCell("Height (cm)", headerWidth));
            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            return table;
        }

        private Table AddAdverseEventTable(DatasetInstance datasetInstance, bool isSerious)
        {
            Table table = new Table();
            DateTime tempdt;

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == datasetInstance.DatasetInstanceGuid);

            var headerWidth = 2500;
            var cellWidth = 3176;
            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() },
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() })
                    );

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Source Description", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue("Description of reaction"), cellWidth));
            tr.Append(PrepareHeaderCell("MedDRA Term", headerWidth));
            tr.Append(PrepareCell(reportInstance.TerminologyMedDra != null ? reportInstance.TerminologyMedDra.MedDraTerm : string.Empty, cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            var temp = datasetInstance.GetInstanceValue("Reaction known start date");
            DateTime onsetDate = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            temp = datasetInstance.GetInstanceValue("Reaction date of recovery");
            DateTime recoveryDate = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Onset Date", headerWidth));
            tr.Append(PrepareCell(onsetDate == DateTime.MinValue ? "" : onsetDate.ToString("yyyy-MM-dd"), cellWidth));
            tr.Append(PrepareHeaderCell("Resolution Date", headerWidth));
            tr.Append(PrepareCell(recoveryDate == DateTime.MinValue ? "" : recoveryDate.ToString("yyyy-MM-dd"), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 3
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Duration", headerWidth));
            if (onsetDate != DateTime.MinValue && recoveryDate != DateTime.MinValue)
            {
                var rduration = (recoveryDate - onsetDate).Days;
                tr.Append(PrepareCell(rduration.ToString() + " days", cellWidth));
            }
            else
            {
                tr.Append(PrepareCell(string.Empty, cellWidth));
            }
            tr.Append(PrepareHeaderCell("Outcome", headerWidth));
            tr.Append(PrepareCell(datasetInstance.GetInstanceValue("Outcome of reaction"), cellWidth));

            table.AppendChild<TableRow>(tr);

            // Cater for seriousness fields
            if (isSerious)
            {
                // Row 4
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Seriousness", headerWidth));
                tr.Append(PrepareCell(datasetInstance.GetInstanceValue("Reaction serious details"), cellWidth));
                tr.Append(PrepareHeaderCell("Grading Scale", headerWidth));
                tr.Append(PrepareCell("", cellWidth));

                table.AppendChild<TableRow>(tr);

                // Row 5
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Severity Grade", headerWidth));
                tr.Append(PrepareCell("", cellWidth));
                tr.Append(PrepareHeaderCell("SAE Number", headerWidth));
                tr.Append(PrepareCell("", cellWidth));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddMedicationTable(DatasetInstance datasetInstance)
        {
            Table table = new Table();

            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == datasetInstance.DatasetInstanceGuid);

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "1250" },
                    new GridColumn() { Width = "3852" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            var sourceProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Product Information");
            var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceProductElement.ElementName);

            var i = 0;
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                i += 1;

                var drugItemValues = datasetInstance.GetInstanceSubValues(sourceProductElement.ElementName, med.ReportInstanceMedicationGuid);

                var startValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Start Date");
                var endValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug End Date");
                var dose = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose number");
                var route = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug route of administration");
                var indication = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Indication");

                // Row 1
                var tr = new TableRow();
                TableRowProperties rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("Drug " + i.ToString(), 2500));
                tr.Append(PrepareHeaderCell("Start Date ", 1250, true, false));
                tr.Append(PrepareHeaderCell("End Date ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Dose ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Route ", 1250, true, false));
                tr.Append(PrepareHeaderCell("Indication ", 3852));

                table.AppendChild<TableRow>(tr);

                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell(drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue, 2500, false));
                tr.Append(PrepareCell(startValue != null ? Convert.ToDateTime(startValue.InstanceValue).ToString("yyyy-MM-dd") : string.Empty, 1250));
                tr.Append(PrepareCell(endValue != null ? Convert.ToDateTime(endValue.InstanceValue).ToString("yyyy-MM-dd") : string.Empty, 1250));
                tr.Append(PrepareCell(dose != null ? dose.InstanceValue : string.Empty, 1250));
                tr.Append(PrepareCell(route != null ? route.InstanceValue : string.Empty, 1250));
                tr.Append(PrepareCell(indication != null ? indication.InstanceValue : string.Empty, 3852, false));

                table.AppendChild<TableRow>(tr);

                // Row 3
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("C.1 CLINICIAN ACTION TAKEN WITH REGARD TO MEDICINE", 11352, false, false, 6));

                table.AppendChild<TableRow>(tr);

                // Row 4
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell("", 11352, false, 5));

                table.AppendChild<TableRow>(tr);

                // Row 5
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareHeaderCell("C.2 EFFECT OF DECHALLENGE/ RECHALLENGE", 11352, false, false, 6));

                table.AppendChild<TableRow>(tr);

                // Row 6
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new HorizontalMerge { Val = MergedCellValues.Continue },
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell("", 11352, false, 5));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddEvaluationTable(DatasetInstance datasetInstance)
        {

            Table table = new Table();

            UInt32Value rowHeight = 12;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "2500" },
                    new GridColumn() { Width = "6352" }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = rowHeight }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Test", 2500));
            tr.Append(PrepareHeaderCell("Test Date", 2500, true, false));
            tr.Append(PrepareHeaderCell("Test Result", 6352));

            table.AppendChild<TableRow>(tr);

            var sourceLabElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Test Results");
            var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceLabElement.ElementName);

            foreach (Guid sourceContext in sourceContexts)
            {
                var labItemValues = datasetInstance.GetInstanceSubValues(sourceLabElement.ElementName, sourceContext);

                var testDate = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Date");
                var testResult = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Result");

                // Row 2
                tr = new TableRow();
                rprops = new TableRowProperties(
                    new TableRowHeight() { Val = rowHeight }
                    );
                tr.AppendChild<TableRowProperties>(rprops);

                tr.Append(PrepareCell(labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Name").InstanceValue, 2500, false));
                tr.Append(PrepareCell(testDate != null ? Convert.ToDateTime(testDate.InstanceValue).ToString("yyyy-MM-dd") : string.Empty, 2500));
                tr.Append(PrepareCell(testResult != null ? testResult.InstanceValue : string.Empty, 6352, false));

                table.AppendChild<TableRow>(tr);
            }

            return table;
        }

        private Table AddNotesTable()
        {
            Table table = new Table();

            var headerWidth = 2500;
            var cellWidth = 8852;
            UInt32Value rowHeight = 24;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = headerWidth.ToString() },
                    new GridColumn() { Width = cellWidth.ToString() }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = (UInt32Value)36, HeightType = HeightRuleValues.AtLeast }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("Notes", headerWidth));
            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            return table;
        }

        private Table AddNotesForMedicationTable()
        {
            Table table = new Table();

            var cellWidth = 11352;
            UInt32Value rowHeight = 24;

            TableProperties tprops = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                    ),
                new TableGrid(
                    new GridColumn() { Width = cellWidth.ToString() }
                    ));

            table.AppendChild<TableProperties>(tprops);

            // Row 1
            var tr = new TableRow();
            TableRowProperties rprops = new TableRowProperties(
                new TableRowHeight() { Val = (UInt32Value)36, HeightType = HeightRuleValues.AtLeast }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareHeaderCell("C.3 Notes", cellWidth));

            table.AppendChild<TableRow>(tr);

            // Row 2
            tr = new TableRow();
            rprops = new TableRowProperties(
                new TableRowHeight() { Val = (UInt32Value)36, HeightType = HeightRuleValues.AtLeast }
                );
            tr.AppendChild<TableRowProperties>(rprops);

            tr.Append(PrepareCell("", cellWidth));

            table.AppendChild<TableRow>(tr);

            return table;
        }

        private TableCell PrepareHeaderCell(string text, int width, bool centre = false, bool bold = false, int mergeCount = 0)
        {
            var tc = new TableCell();

            TableCellProperties props = new TableCellProperties(
                new TableCellWidth { Width = width.ToString() },
                new Shading { Color = "auto", ThemeFillShade = "D9", Fill = "D9D9D9" },
                new TableCellMargin(
                    new BottomMargin { Width = "30" },
                    new TopMargin { Width = "30" },
                    new LeftMargin { Width = "30" },
                    new RightMargin { Width = "30" }
                    ),
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                );
            if (mergeCount > 0) { props.Append(new GridSpan { Val = mergeCount }); };
            tc.AppendChild<TableCellProperties>(props);

            if (centre)
            {
                ParagraphProperties pprop = new ParagraphProperties();
                Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
                pprop.Append(CenterHeading);
                tc.AppendChild<ParagraphProperties>(pprop);
            };

            RunProperties runProperties = new RunProperties();
            if (bold) { runProperties.AppendChild(new Bold()); };
            FontSize fs = new FontSize();
            fs.Val = "20";
            runProperties.AppendChild(fs);
            Run run = new Run();
            run.AppendChild(runProperties);
            run.AppendChild(new Text(text));

            tc.Append(new Paragraph(run));

            return tc;
        }

        private TableCell PrepareCell(string text, int width, bool centre = true, int mergeCount = 0)
        {
            var tc = new TableCell();

            TableCellProperties props = new TableCellProperties(
                new TableCellWidth { Width = width.ToString() },
                new TableCellMargin(
                    new BottomMargin { Width = "30" },
                    new TopMargin { Width = "30" },
                    new LeftMargin { Width = "30" },
                    new RightMargin { Width = "30" }
                    ),
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
                );
            if (mergeCount > 0) { props.Append(new GridSpan { Val = mergeCount }); };
            tc.AppendChild<TableCellProperties>(props);

            if (centre)
            {
                ParagraphProperties pprop = new ParagraphProperties();
                Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
                pprop.Append(CenterHeading);
                tc.AppendChild<ParagraphProperties>(pprop);
            };

            RunProperties runProperties = new RunProperties();
            FontSize fs = new FontSize();
            fs.Val = "20";
            runProperties.AppendChild(fs);
            Run run = new Run();
            run.AppendChild(runProperties);
            run.AppendChild(new Text(text));

            tc.Append(new Paragraph(run));

            return tc;
        }

        private void PrepareLogo(WordprocessingDocument document)
        {
            ImagePart imagePart = document.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);

            var fileName = Path.Combine(_environment.ContentRootPath, "StaticFiles", "SIAPS_USAID_Small.jpg");
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            AddImageToBody(document, document.MainDocumentPart.GetIdOfPart(imagePart));
        }

        private static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 1757548L, Cy = 253064L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 1757548L, Cy = 253064L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            Paragraph paragraph = new Paragraph();
            ParagraphProperties pprop = new ParagraphProperties();
            Justification CenterHeading = new Justification() { Val = JustificationValues.Center };
            pprop.Append(CenterHeading);
            pprop.ParagraphStyleId = new ParagraphStyleId() { Val = "logoheading" };
            paragraph.Append(pprop);

            Run run = new Run();
            run.AppendChild(element);
            paragraph.Append(run);

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(paragraph);
        }

        #endregion

        private int CalculateAge(DateTime birthDate, DateTime onsetDate)
        {
            var age = onsetDate.Year - birthDate.Year;
            if (onsetDate > birthDate.AddYears(-age)) age--;
            return age;
        }

        //private IEnumerable<MergeElement> GetPatientSummaryMergeElementsForActiveReport(PatientClinicalEvent patientEvent, bool isSerious)
        //{
        //    IExtendable patientEventExtended = patientEvent;
        //    IExtendable patientExtended = patientEvent.Patient;

        //    var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == patientEvent.PatientClinicalEventGuid);

        //    var genderConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "Patient" && c.AttributeKey == "Gender");
        //    var outcomeConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Outcome");
        //    var seriousnessConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Seriousness");
        //    var scaleConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Severity Grading Scale");
        //    var severityConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientClinicalEvent" && c.AttributeKey == "Severity Grade");
        //    var routeConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Route");
        //    var indicationConfig = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().SingleOrDefault(c => c.ExtendableTypeName == "PatientMedication" && c.AttributeKey == "Indication");

        //    var mergeElements = new List<MergeElement>();

        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "PatientName",
        //        MergeValue = patientEvent.Patient.FullName
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "BirthDate",
        //        MergeValue = patientEvent.Patient.DateOfBirth != null ? Convert.ToDateTime(patientEvent.Patient.DateOfBirth).ToString("yyyy-MM-dd") : ""
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "AgeGroup",
        //        MergeValue = patientEventExtended.GetAttributeValue("Age Group") != null ? patientEventExtended.GetAttributeValue("Age Group").ToString() : ""
        //    });
        //    if (patientEvent.OnsetDate != null && patientEvent.Patient.DateOfBirth != null)
        //    {
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "OnsetAge",
        //            MergeValue = CalculateAge(Convert.ToDateTime(patientEvent.Patient.DateOfBirth), Convert.ToDateTime(patientEvent.OnsetDate)).ToString() + " years"
        //        });
        //    }
        //    else
        //    {
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "OnsetAge",
        //            MergeValue = string.Empty
        //        });
        //    }
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Gender",
        //        MergeValue = _attributeService.GetCustomAttributeValue(genderConfig, patientExtended)
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Facility",
        //        MergeValue = patientEvent.Patient.GetCurrentFacility().Facility.FacilityName
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "MedicalNumber",
        //        MergeValue = patientExtended.GetAttributeValue("Medical Record Number") != null ? patientExtended.GetAttributeValue("Medical Record Number").ToString() : ""
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "IdentityNumber",
        //        MergeValue = patientExtended.GetAttributeValue("Patient Identity Number") != null ? patientExtended.GetAttributeValue("Patient Identity Number").ToString() : ""
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "SourceDescription",
        //        MergeValue = patientEvent.SourceDescription
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "MeddraTerm",
        //        MergeValue = patientEvent.SourceTerminologyMedDra.MedDraTerm
        //    });
        //    var onsetDate = patientEvent.OnsetDate != null ? Convert.ToDateTime(patientEvent.OnsetDate).ToString("yyyy-MM-dd") : "";
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "OnsetDate",
        //        MergeValue = onsetDate
        //    });
        //    var resDate = patientEvent.ResolutionDate != null ? Convert.ToDateTime(patientEvent.ResolutionDate).ToString("yyyy-MM-dd") : "";
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "ResolutionDate",
        //        MergeValue = resDate
        //    });
        //    if (!String.IsNullOrWhiteSpace(onsetDate) && !String.IsNullOrWhiteSpace(resDate))
        //    {
        //        var rduration = (Convert.ToDateTime(onsetDate) - Convert.ToDateTime(resDate)).Days;
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Duration",
        //            MergeValue = rduration.ToString() + " days"
        //        });
        //    }
        //    else
        //    {
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Duration",
        //            MergeValue = string.Empty
        //        });
        //    }
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Outcome",
        //        MergeValue = _attributeService.GetCustomAttributeValue(outcomeConfig, patientExtended)
        //    });

        //    // Cater for seriousness fields
        //    if (isSerious)
        //    {
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Seriousness",
        //            MergeValue = _attributeService.GetCustomAttributeValue(seriousnessConfig, patientExtended)
        //        });
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "GradingScale",
        //            MergeValue = _attributeService.GetCustomAttributeValue(scaleConfig, patientExtended)

        //        });
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "SeverityGrade",
        //            MergeValue = _attributeService.GetCustomAttributeValue(severityConfig, patientExtended)
        //        });
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "SAENumber",
        //            MergeValue = patientEventExtended.GetAttributeValue("FDA SAE Number") != null ? patientEventExtended.GetAttributeValue("FDA SAE Number").ToString() : ""
        //        });
        //    }

        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Weight",
        //        MergeValue = string.Empty
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Height",
        //        MergeValue = string.Empty
        //    });

        //    // lab tests
        //    var ct = 0;
        //    foreach (PatientLabTest labTest in patientEvent.Patient.PatientLabTests.Where(lt => lt.TestDate >= patientEvent.OnsetDate).OrderByDescending(lt => lt.TestDate).Take(4))
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Test" + cts,
        //            MergeValue = labTest.LabTest.Description
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "TestDate" + cts,
        //            MergeValue = labTest.TestDate.ToString("yyyy-MM-dd")
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "TestResult" + cts,
        //            MergeValue = labTest.TestResult
        //        });
        //    }
        //    while (ct < 5)
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Test" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "TestDate" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "TestResult" + cts,
        //            MergeValue = string.Empty
        //        });
        //    }

        //    // Medications
        //    ct = 0;
        //    foreach (ReportInstanceMedication med in reportInstance.Medications.Take(2))
        //    {
        //        ct += 1;

        //        var cts = string.Format("_{0}", ct.ToString());
        //        var patientMedication = _unitOfWork.Repository<PatientMedication>().Queryable().Single(pm => pm.PatientMedicationGuid == med.ReportInstanceMedicationGuid);
        //        IExtendable mcExtended = patientMedication;

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Drug" + cts,
        //            MergeValue = patientMedication.Medication.DrugName
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "DStart" + cts,
        //            MergeValue = patientMedication.DateStart.ToString("yyyy-MM-dd")
        //        });

        //        var endDate = patientMedication.DateEnd != null ? Convert.ToDateTime(patientMedication.DateEnd).ToString("yyyy-MM-dd") : "";
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "DEnd" + cts,
        //            MergeValue = endDate
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Dose" + cts,
        //            MergeValue = patientMedication.Dose
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Route" + cts,
        //            MergeValue = _attributeService.GetCustomAttributeValue(routeConfig, mcExtended)
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Indication" + cts,
        //            MergeValue = _attributeService.GetCustomAttributeValue(indicationConfig, mcExtended)
        //        });
        //    }
        //    while (ct < 3)
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Drug" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "DStart" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "DEnd" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Dose" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Route" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Indication" + cts,
        //            MergeValue = string.Empty
        //        });
        //    }

        //    // Conditions
        //    ct = 0;
        //    foreach (PatientCondition pc in patientEvent.Patient.PatientConditions.Where(pc => pc.DateStart <= patientEvent.OnsetDate).OrderByDescending(c => c.DateStart).Take(2))
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Condition" + cts,
        //            MergeValue = pc.TerminologyMedDra.MedDraTerm
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "StartDate" + cts,
        //            MergeValue = pc.DateStart.ToString("yyyy-MM-dd")
        //        });

        //        var endDate = pc.OutcomeDate != null ? Convert.ToDateTime(pc.OutcomeDate).ToString("yyyy-MM-dd") : "";
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "EndDate" + cts,
        //            MergeValue = endDate
        //        });
        //    }
        //    while (ct < 3)
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Condition" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "StartDate" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "EndDate" + cts,
        //            MergeValue = string.Empty
        //        });
        //    }

        //    return mergeElements;
        //}

        //private IEnumerable<MergeElement> GetPatientSummaryMergeElementsForSpontaneousReport(DatasetInstance datasetInstance, bool isSerious)
        //{
        //    DateTime tempdt;

        //    var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == datasetInstance.DatasetInstanceGuid);

        //    var mergeElements = new List<MergeElement>();

        //    var temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "0D704069-5C50-4085-8FE1-355BB64EF196"));
        //    DateTime dob = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

        //    temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "D314C438-5ABA-4ED2-855D-1A5B22B5A301"));
        //    string age = temp;
        //    temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "80C219DC-238C-487E-A3D5-8919ABA674B1"));
        //    age += " " + temp; // Include age unit

        //    temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F5EEB382-D4A5-41A1-A447-37D5ECA50B99"));
        //    DateTime onsetDate = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

        //    temp = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F977C2F8-C7DD-4AFE-BCAA-1C06BD54D155"));
        //    DateTime recoveryDate = DateTime.TryParse(temp, out tempdt) ? tempdt : DateTime.MinValue;

        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "PatientName",
        //        MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "29CD2157-8FB6-4883-A4E6-A4B9EDE6B36B"))
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "BirthDate",
        //        MergeValue = dob == DateTime.MinValue ? "" : dob.ToString("yyyy-MM-dd")
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "AgeGroup",
        //        MergeValue = ""
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "OnsetAge",
        //        MergeValue = age
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Gender",
        //        MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "E061D363-534E-4EA4-B6E5-F1C531931B12"))

        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Facility",
        //        MergeValue = string.Empty
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "MedicalNumber",
        //        MergeValue = string.Empty
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "IdentityNumber",
        //        MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "5A2E89A9-8240-4665-967D-0C655CF281B7"))
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "SourceDescription",
        //        MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "ACD938A4-76D1-44CE-A070-2B8DF0FE9E0F"))
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "MeddraTerm",
        //        MergeValue = reportInstance.TerminologyMedDra != null ? reportInstance.TerminologyMedDra.MedDraTerm : string.Empty
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "OnsetDate",
        //        MergeValue = onsetDate == DateTime.MinValue ? "" : onsetDate.ToString("yyyy-MM-dd")
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "ResolutionDate",
        //        MergeValue = recoveryDate == DateTime.MinValue ? "" : recoveryDate.ToString("yyyy-MM-dd")
        //    });
        //    if (onsetDate != DateTime.MinValue && recoveryDate != DateTime.MinValue)
        //    {
        //        var rduration = (onsetDate - recoveryDate).Days;
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Duration",
        //            MergeValue = rduration.ToString() + " days"
        //        });
        //    }
        //    else
        //    {
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Duration",
        //            MergeValue = string.Empty
        //        });
        //    }
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Outcome",
        //        MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "976F6C53-78F2-4007-8F39-54057E554EEB"))
        //    });

        //    // Cater for seriousness fields
        //    if (isSerious)
        //    {
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Seriousness",
        //            MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "302C07C9-B0E0-46AB-9EF8-5D5C2F756BF1"))
        //        });
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "GradingScale",
        //            MergeValue = string.Empty

        //        });
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "SeverityGrade",
        //            MergeValue = string.Empty
        //        });
        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "SAENumber",
        //            MergeValue = string.Empty
        //        });
        //    }

        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Weight",
        //        MergeValue = datasetInstance.GetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "985BD25D-54E7-4A24-8636-6DBC0F9C7B96"))
        //    });
        //    mergeElements.Add(new TextMergeElement
        //    {
        //        MergeField = "Height",
        //        MergeValue = string.Empty
        //    });

        //    // lab tests
        //    var sourceLabElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "12D7089D-1603-4309-99DE-60F20F9A005E");
        //    var sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceLabElement);

        //    var ct = 0;
        //    foreach (Guid sourceContext in sourceContexts)
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        var labItemValues = datasetInstance.GetInstanceSubValues(sourceLabElement, sourceContext);

        //        var testDate = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Date");
        //        var testResult = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Result");

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Test" + cts,
        //            MergeValue = labItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Test Name").InstanceValue
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "TestDate" + cts,
        //            MergeValue = testDate != null ? testDate.InstanceValue : string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "TestResult" + cts,
        //            MergeValue = testResult != null ? testResult.InstanceValue : string.Empty
        //        });
        //    }
        //    while (ct < 5)
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Test" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "TestDate" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "TestResult" + cts,
        //            MergeValue = string.Empty
        //        });
        //    }

        //    // Medications
        //    var sourceProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "712CA632-0CD0-4418-9176-FB0B95AEE8A1");
        //    sourceContexts = datasetInstance.GetInstanceSubValuesContext(sourceProductElement);

        //    ct = 0;
        //    foreach (ReportInstanceMedication med in reportInstance.Medications.Take(2))
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        var drugItemValues = datasetInstance.GetInstanceSubValues(sourceProductElement, med.ReportInstanceMedicationGuid);

        //        var startValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product start date");
        //        var endValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product end date");
        //        var dose = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose number");
        //        var route = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product route of administration");
        //        var indication = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product indication");

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Drug" + cts,
        //            MergeValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "DStart" + cts,
        //            MergeValue = startValue != null ? startValue.InstanceValue : string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "DEnd" + cts,
        //            MergeValue = endValue != null ? endValue.InstanceValue : string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Dose" + cts,
        //            MergeValue = dose != null ? dose.InstanceValue : string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Route" + cts,
        //            MergeValue = route != null ? route.InstanceValue : string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Indication" + cts,
        //            MergeValue = indication != null ? indication.InstanceValue : string.Empty
        //        });
        //    }
        //    while (ct < 3)
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Drug" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "DStart" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "DEnd" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Dose" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Route" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Indication" + cts,
        //            MergeValue = string.Empty
        //        });
        //    }

        //    // Conditions
        //    ct = 0;
        //    while (ct < 3)
        //    {
        //        ct += 1;
        //        var cts = string.Format("_{0}", ct.ToString());

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "Condition" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "StartDate" + cts,
        //            MergeValue = string.Empty
        //        });

        //        mergeElements.Add(new TextMergeElement
        //        {
        //            MergeField = "EndDate" + cts,
        //            MergeValue = string.Empty
        //        });
        //    }

        //    return mergeElements;
        //}

        #region "Excel Processing"

        private void ProcessEntity(Object obj, ref ExcelWorksheet ws, ref int rowCount, ref int colCount, string entityName)
        {
            DateTime tempdt;

            // Write headers
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            var attributes = _unitOfWork.Repository<CustomAttributeConfiguration>().Queryable().Where(c => c.ExtendableTypeName == entityName).OrderBy(c => c.Id);

            IQueryable elements = null;
            if (entityName == "Encounter")
            {
                elements = _unitOfWork.Repository<DatasetCategoryElement>()
                    .Queryable()
                    .Where(dce => dce.DatasetCategory.Dataset.DatasetName == "Chronic Treatment")
                    .OrderBy(dce => dce.DatasetCategory.CategoryOrder)
                    .ThenBy(dce => dce.FieldOrder);
            }

            if (rowCount == 1)
            {
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name != "CustomAttributesXmlSerialised")
                    {
                        if (property.PropertyType == typeof(DateTime?)
                            || property.PropertyType == typeof(DateTime)
                            || property.PropertyType == typeof(string)
                            || property.PropertyType == typeof(int)
                            || property.PropertyType == typeof(decimal)
                            || property.PropertyType == typeof(User)
                            || property.PropertyType == typeof(Concept)
                            || property.PropertyType == typeof(Product)
                            || property.PropertyType == typeof(Patient)
                            || property.PropertyType == typeof(Encounter)
                            || property.PropertyType == typeof(TerminologyMedDra)
                            || property.PropertyType == typeof(Outcome)
                            || property.PropertyType == typeof(LabTest)
                            || property.PropertyType == typeof(LabTestUnit)
                            || property.PropertyType == typeof(CohortGroup)
                            || property.PropertyType == typeof(Facility)
                            || property.PropertyType == typeof(Priority)
                            || property.PropertyType == typeof(EncounterType)
                            || property.PropertyType == typeof(TreatmentOutcome)
                            || property.PropertyType == typeof(Guid)
                            || property.PropertyType == typeof(bool)
                            )
                        {
                            ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = property.Name;
                            colCount += 1;
                        }
                    }
                }

                // Now process attributes
                foreach (CustomAttributeConfiguration attribute in attributes)
                {
                    ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = attribute.AttributeKey;
                    colCount += 1;
                }

                // Process instance headers
                if (elements != null)
                {
                    foreach (DatasetCategoryElement dce in elements)
                    {
                        ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = dce.DatasetElement.ElementName;
                        colCount += 1;
                    }
                }

                //Set the first header and format it
                using (var r = ws.Cells["A1:" + GetExcelColumnName(colCount - 1) + "1"])
                {
                    r.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10, FontStyle.Bold));
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    r.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                }
            }

            rowCount += 1;
            colCount = 1;

            var subOutput = "**IGNORE**";

            // Write values
            foreach (PropertyInfo property in properties)
            {
                if (property.Name != "CustomAttributesXmlSerialised")
                {
                    subOutput = "**IGNORE**";
                    if (property.PropertyType == typeof(DateTime?)
                        || property.PropertyType == typeof(DateTime))
                    {
                        var dt = property.GetValue(obj, null) != null ? Convert.ToDateTime(property.GetValue(obj, null)).ToString("yyyy-MM-dd") : "";
                        subOutput = dt;
                    }
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(int)
                        || property.PropertyType == typeof(decimal)
                        || property.PropertyType == typeof(Guid)
                        || property.PropertyType == typeof(bool)
                        )
                    {
                        subOutput = property.GetValue(obj, null) != null ? property.GetValue(obj, null).ToString() : "";
                    }
                    if (property.PropertyType == typeof(User))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var user = (User)property.GetValue(obj, null);
                            subOutput = user.UserName;
                        }
                    }
                    if (property.PropertyType == typeof(Patient))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var patient = (Patient)property.GetValue(obj, null);
                            subOutput = patient.PatientGuid.ToString();
                        }
                    }
                    if (property.PropertyType == typeof(Concept))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var medication = (Concept)property.GetValue(obj, null);
                            subOutput = medication.ConceptName;
                        }
                    }
                    if (property.PropertyType == typeof(Product))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var medication = (Product)property.GetValue(obj, null);
                            subOutput = medication.ProductName;
                        }
                    }
                    if (property.PropertyType == typeof(Encounter))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var encounter = (Encounter)property.GetValue(obj, null);
                            subOutput = encounter.EncounterGuid.ToString();
                        }
                    }
                    if (property.PropertyType == typeof(TerminologyMedDra))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var meddra = (TerminologyMedDra)property.GetValue(obj, null);
                            subOutput = meddra.DisplayName;
                        }
                    }
                    if (property.PropertyType == typeof(Outcome))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var outcome = (Outcome)property.GetValue(obj, null);
                            subOutput = outcome.Description;
                        }
                    }
                    if (property.PropertyType == typeof(TreatmentOutcome))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var txOutcome = (TreatmentOutcome)property.GetValue(obj, null);
                            subOutput = txOutcome.Description;
                        }
                    }
                    if (property.PropertyType == typeof(LabTest))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var labTest = (LabTest)property.GetValue(obj, null);
                            subOutput = labTest.Description;
                        }
                    }
                    if (property.PropertyType == typeof(LabTestUnit))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var unit = (LabTestUnit)property.GetValue(obj, null);
                            subOutput = unit.Description;
                        }
                    }
                    if (property.PropertyType == typeof(CohortGroup))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var group = (CohortGroup)property.GetValue(obj, null);
                            subOutput = group.DisplayName;
                        }
                    }
                    if (property.PropertyType == typeof(Facility))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var facility = (Facility)property.GetValue(obj, null);
                            subOutput = facility.FacilityName;
                        }
                    }
                    if (property.PropertyType == typeof(Priority))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var priority = (Priority)property.GetValue(obj, null);
                            subOutput = priority.Description;
                        }
                    }
                    if (property.PropertyType == typeof(EncounterType))
                    {
                        subOutput = "";
                        if (property.GetValue(obj, null) != null)
                        {
                            var et = (EncounterType)property.GetValue(obj, null);
                            subOutput = et.Description;
                        }
                    }
                    if (subOutput != "**IGNORE**")
                    {
                        ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = subOutput;
                        colCount += 1;
                    }
                }
            }

            IExtendable extended = null;
            switch (entityName)
            {
                case "Patient":
                    extended = (Patient)obj;
                    break;

                case "PatientMedication":
                    extended = (PatientMedication)obj;
                    break;

                case "PatientClinicalEvent":
                    extended = (PatientClinicalEvent)obj;
                    break;

                case "PatientCondition":
                    extended = (PatientCondition)obj;
                    break;

                case "PatientLabTest":
                    extended = (PatientLabTest)obj;
                    break;

                default:
                    break;
            }

            if (extended != null)
            {
                foreach (CustomAttributeConfiguration attribute in attributes)
                {
                    var output = "";
                    var val = extended.GetAttributeValue(attribute.AttributeKey);
                    if (val != null)
                    {
                        if (attribute.CustomAttributeType == CustomAttributeType.Selection)
                        {
                            var tempSDI = _unitOfWork.Repository<SelectionDataItem>().Queryable().SingleOrDefault(u => u.AttributeKey == attribute.AttributeKey && u.SelectionKey == val.ToString());
                            if (tempSDI != null)
                                output = tempSDI.Value;
                        }
                        else if (attribute.CustomAttributeType == CustomAttributeType.DateTime)
                        {
                            if (attribute != null && DateTime.TryParse(val.ToString(), out tempdt))
                            {
                                output = tempdt.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                output = val.ToString();
                            }
                        }
                        else
                        {
                            output = val.ToString();
                        }
                    }

                    ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = output;
                    colCount += 1;
                }
            }

            if (elements != null)
            {
                var id = Convert.ToInt32(((Encounter)obj).Id);
                var instance = _unitOfWork.Repository<DatasetInstance>()
                    .Queryable()
                    .SingleOrDefault(di => di.Dataset.DatasetName == "Chronic Treatment" && di.ContextId == id);
                foreach (DatasetCategoryElement dce in elements)
                {
                    var eleOutput = "";
                    if (instance != null)
                    {
                        eleOutput = instance.GetInstanceValue(dce.DatasetElement.ElementName);
                    }

                    ws.Cells[GetExcelColumnName(colCount) + rowCount].Value = eleOutput;
                    colCount += 1;
                }
            }
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        #endregion
    }
}
