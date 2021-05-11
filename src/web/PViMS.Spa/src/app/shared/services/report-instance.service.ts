import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { CausalityReportWrapperModel } from '../models/causality.report.model';
import { ActivityChangeModel } from '../models/activity/activity-change.model';
import { ReportInstanceDetailWrapperModel } from '../models/report-instance/report-instance.detail.model';
import { ReportInstanceExpandedWrapperModel } from '../models/report-instance/report-instance.expanded.model';

@Injectable({ providedIn: 'root' })
export class ReportInstanceService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "";
    }

    searchReportInstanceByActivity(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'qualifiedName', value: filterModel.qualifiedName });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      if(filterModel.activeReportsOnly != undefined) {
        parameters.push(<ParameterKeyValueModel> { key: 'activeReportsOnly', value: filterModel.activeReportsOnly});
      }

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    searchReportInstanceByDate(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
      parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    searchReportInstanceByTerm(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getFeedbackReportInstancesByDetail(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.feedback.v1+json', parameters);
    }

    searchFeedbackInstanceByTerm(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.feedback.v1+json', parameters);
    }

    getNewReportInstancesByDetail(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.newreports.v1+json', parameters);
    }

    getReportInstanceDetail(workFlowGuid: string, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.detail.v1+json', parameters);
    } 

    getReportInstanceExpanded(workFlowGuid: string, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<ReportInstanceExpandedWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.expanded.v1+json', parameters);
    } 

    getReportInstanceTaskDetail(workFlowGuid: string, reportInstanceId: number, reportInstanceTaskId: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: reportInstanceTaskId.toString() });

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks`, 'application/vnd.pvims.detail.v1+json', parameters);
    } 

    getActivityChangeStatus(workFlowGuid: string, reportInstanceId: number, id: number, activityExecutionStatusId: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      let header = '';
      if(activityExecutionStatusId == 85)
      {
        header = 'application/vnd.pvims.activitystatusconfirm.v1+json';
      }
      return this.Get<ActivityChangeModel>(`/workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/activity`, header, parameters);
    }

    getCausalityReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'causalityCriteria', value: filterModel.criteriaId == null ? 1 : filterModel.criteriaId });
        parameters.push(<ParameterKeyValueModel> { key: 'facilityId', value: filterModel.facilityId == null ? 0 : filterModel.facilityId });
        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

        return this.Get<CausalityReportWrapperModel>(`/workflow/892F3305-7819-4F18-8A87-11CBA3AEE219/reportinstances`, 'application/vnd.pvims.causalityreport.v1+json', parameters);
    }

    downloadAttachment(workFlowGuid: string, reportInstanceId: number, activityExecutionStatusEventId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });
  
      return this.Download(`/workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/activity/${activityExecutionStatusEventId}/attachments`, 'application/vnd.pvims.attachment.v1+xml', parameters);
    }

    downloadSummary(workFlowGuid: string, reportInstanceId: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: reportInstanceId.toString() });
  
      return this.Download(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.pvims.patientsummary.v1+json', parameters);
    }

    addTaskToReportInstanceCommand(workFlowGuid: string, reportInstanceId: number, model: any): any {
      return this.Post(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks`, model);
    }    

    changeTaskDetailsCommand(workFlowGuid: string, reportInstanceId: number, reportInstanceTaskId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks/${reportInstanceTaskId}/details`, model);
    }

    changeTaskStatusCommand(workFlowGuid: string, reportInstanceId: number, reportInstanceTaskId: number, model: any): any {
      return this.Post(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks/${reportInstanceTaskId}/status`, model);
    }

    updateStatus(workFlowGuid: string, reportInstanceId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/status`, model);
    }

    updateTerminology(workFlowGuid: string, reportInstanceId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/terminology`, model);
    }

    updateCausality(workFlowGuid: string, reportInstanceId: number, medicationId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/medications/${medicationId}/causality`, model);
    }

    createE2B(workFlowGuid: string, reportInstanceId: number): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/createe2b`, null);
    }

}
