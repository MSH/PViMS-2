import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { MetaTableDetailWrapperModel } from '../models/meta/meta-table.detail.model';
import { MetaColumnDetailWrapperModel } from '../models/meta/meta-column.detail.model';
import { MetaDependencyDetailWrapperModel } from '../models/meta/meta-dependency.detail.model';
import { MetaSummaryModel } from '../models/meta/meta-summary.model';

@Injectable({ providedIn: 'root' })
export class MetaService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getMetaTables(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaTableDetailWrapperModel>('/metatables', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getMetaColumns(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaColumnDetailWrapperModel>('/metacolumns', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getMetaDependencies(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaDependencyDetailWrapperModel>('/metadependencies', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getMetaSummary(): any {
    let parameters: ParameterKeyValueModel[] = [];

    return this.Get<MetaSummaryModel>('/meta', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  refresh(): any {
    return this.Post('meta', null);
  }  
}
