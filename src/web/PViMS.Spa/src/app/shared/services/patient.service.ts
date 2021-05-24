import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { PatientDetailWrapperModel, PatientDetailModel } from '../models/patient/patient.detail.model';
import { PatientExpandedModel } from '../models/patient/patient.expanded.model';
import { AdverseEventReportWrapperModel } from '../models/adverseevent.report.model';
import { AdverseEventFrequencyReportWrapperModel } from '../models/adverseeventfrequency.report.model';
import { PatientConditionDetailModel } from '../models/patient/patient-condition.detail.model';
import { PatientClinicalEventDetailModel } from '../models/patient/patient-clinical-event.detail.model';
import { PatientClinicalEventExpandedModel } from '../models/patient/patient-clinical-event.expanded.model';
import { PatientLabTestDetailModel } from '../models/patient/patient-lab-test.detail.model';
import { PatientMedicationReportWrapperModel } from '../models/patient/patient-medication.report.model';
import { PatientTreatmentReportWrapperModel } from '../models/patient/patient-treatment.report.model';

@Injectable({ providedIn: 'root' })
export class PatientService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "/patients";
    }

    searchPatient(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'facilityName', value: filterModel.facilityName });
      if (filterModel.patientId != null && filterModel.patientId != '') {
          parameters.push(<ParameterKeyValueModel> { key: 'patientId', value: filterModel.patientId });
      }
      if (filterModel.firstName != null) {
          parameters.push(<ParameterKeyValueModel> { key: 'firstName', value: filterModel.firstName });
      }
      if (filterModel.lastName != null) {
          parameters.push(<ParameterKeyValueModel> { key: 'lastName', value: filterModel.lastName });
      }
      if (filterModel.dateOfBirth != null) {
          parameters.push(<ParameterKeyValueModel> { key: 'dateOfBirth', value: filterModel.dateOfBirth.format("YYYY-MM-DD") });
      }
      if (filterModel.customAttributeId > 0) {
        parameters.push(<ParameterKeyValueModel> { key: 'customAttributeId', value: filterModel.customAttributeId });
        parameters.push(<ParameterKeyValueModel> { key: 'customAttributeValue', value: filterModel.customAttributeValue });
      }
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<PatientDetailWrapperModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    searchPatientByCondition(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];
      console.log(filterModel);
      parameters.push(<ParameterKeyValueModel> { key: 'customAttributeKey', value: filterModel.customAttributeKey });
      parameters.push(<ParameterKeyValueModel> { key: 'customAttributeValue', value: filterModel.customAttributeValue });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '10'});

      return this.Get<PatientExpandedModel>('', 'application/vnd.pvims.expanded.v1+json', parameters);
    }    

    getPatientExpanded(id: number): any {
        let parameters: ParameterKeyValueModel[] = [];
        parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

        return this.Get<PatientExpandedModel>('', 'application/vnd.pvims.expanded.v1+json', parameters);
    } 

    getPatientDetail(id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientDetailModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getPatientConditionDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientConditionDetailModel>(`/patients/${patientId}/conditions`, 'application/vnd.pvims.detail.v1+json', parameters);
    } 

    getPatientClinicalEventDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientClinicalEventDetailModel>(`/patients/${patientId}/clinicalevents`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getPatientClinicalEventExpanded(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientClinicalEventExpandedModel>(`/patients/${patientId}/clinicalevents`, 'application/vnd.pvims.expanded.v1+json', parameters);
    }

    getPatientMedicationDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientClinicalEventDetailModel>(`/patients/${patientId}/medications`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getPatientLabTestDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientLabTestDetailModel>(`/patients/${patientId}/labtests`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getAdverseEventReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'adverseEventCriteria', value: filterModel.criteriaId == null ? 1 : filterModel.criteriaId });
        parameters.push(<ParameterKeyValueModel> { key: 'adverseEventStratifyCriteria', value: filterModel.stratifyId == null ? 1 : filterModel.stratifyId });
        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

        return this.Get<AdverseEventReportWrapperModel>('/patients', 'application/vnd.pvims.adverseventreport.v1+json', parameters);
    }    

    getAdverseEventQuarterlyReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

        return this.Get<AdverseEventFrequencyReportWrapperModel>('/patients', 'application/vnd.pvims.quarterlyadverseventreport.v1+json', parameters);
    }    

    getAdverseEventAnnualReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

        return this.Get<AdverseEventFrequencyReportWrapperModel>('/patients', 'application/vnd.pvims.annualadverseventreport.v1+json', parameters);
    }    
    
    getPatientTreatmentReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'patientOnStudyCriteria', value: filterModel.criteriaId == null ? 1 : filterModel.criteriaId });
        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

        return this.Get<PatientTreatmentReportWrapperModel>('/patients', 'application/vnd.pvims.patienttreatmentreport.v1+json', parameters);
    }

    getPatientMedicationReport(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.conceptName});

      return this.Get<PatientMedicationReportWrapperModel>('/patients', 'application/vnd.pvims.patientmedicationreport.v1+json', parameters);
    }
  
    downloadAttachment(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });
  
      return this.Download(`/patients/${patientId}/attachments`, 'application/vnd.pvims.attachment.v1+xml', parameters);
    }

    downloadAllAttachment(patientId: number): any {
      let parameters: ParameterKeyValueModel[] = [];
  
      return this.Download(`/patients/${patientId}/attachments`, 'application/vnd.pvims.attachment.v1+xml', parameters);
    }

    savePatient(id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post('', shallowModel);
      }
      else {
        return this.Put(`${id}`, shallowModel);
      }
    }

    savePatientCondition(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post(`${patientId}/conditions`, shallowModel);
      }
      else {
        return this.Put(`${patientId}/conditions/${id}`, shallowModel);
      }
    }    

    savePatientClinicalEvent(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post(`${patientId}/clinicalevents`, shallowModel);
      }
      else {
        return this.Put(`${patientId}/clinicalevents/${id}`, shallowModel);
      }
    }    

    savePatientMedication(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      console.log(shallowModel);
      if(id == 0) {
        return this.Post(`${patientId}/medications`, shallowModel);
      }
      else {
        return this.Put(`${patientId}/medications/${id}`, shallowModel);
      }
    }

    savePatientLabTest(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post(`${patientId}/labtests`, shallowModel);
      }
      else {
        return this.Put(`${patientId}/labtests/${id}`, shallowModel);
      }
    }    

    saveAttachment(patientId: number, fileToUpload: File, model: any): any {
      const formData: FormData = new FormData();
      formData.append('description', model.description);
      formData.append('attachment', fileToUpload, fileToUpload.name);

      return this.PostFile(`${patientId}/attachments`, formData);
    }

    archivePatient(id: number, model: any): any {
      return this.Put(`${id}/archive`, model);
    }

    archiveAttachment(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/attachments/${id}/archive`, model);
    }

    archivePatientCondition(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/conditions/${id}/archive`, model);
    }

    archivePatientClinicalEvent(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/clinicalevents/${id}/archive`, model);
    }

    archivePatientMedication(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/medications/${id}/archive`, model);
    }

    archivePatientLabTest(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/labtests/${id}/archive`, model);
    }

}
