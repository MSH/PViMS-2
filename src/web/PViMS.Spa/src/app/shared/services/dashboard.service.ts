import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';

@Injectable({ providedIn: 'root' })
export class DashboardService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);    
        this.apiController = "";
    }
    generateDashboard(id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<any[]>('/dashboards', 'application/vnd.pvims.detail.v1+json', parameters);
    }     
}
