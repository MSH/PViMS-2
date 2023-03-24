import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { FilterModel } from '../models/grid.model';
import { DashboardDetailWrapperModel } from '../models/dashboard/dashboard.detail.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DashboardService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);    
        this.apiController = "";
    }

    getAllDashboards(): any {
      let filter = new FilterModel();
      filter.recordsPerPage = 50;
      filter.currentPage = 1;
  
      return this.getDashboards(filter)
        .pipe( 
          expand(response => {
            let typedResponse = response as DashboardDetailWrapperModel;
            let next = typedResponse.links.find(l => l.rel == 'nextPage');
            return next ? this.GetByAddress<DashboardDetailWrapperModel>(next.href, 'application/vnd.pvims.detail.v1+json') : EMPTY;
          }),
          map(response => {
            let typedResponse = response as DashboardDetailWrapperModel;
            return typedResponse.value;
          }),
          reduce((accData, data) => accData.concat(data), [])
        );
    }
  
    getDashboards(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];
  
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
  
      return this.Get<DashboardDetailWrapperModel>('/dashboards', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    generateDashboard(id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<any[]>('/dashboards', 'application/vnd.pvims.detail.v1+json', parameters);
    }     
}
