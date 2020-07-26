import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { MetaTableDetailWrapperModel } from '../models/meta/meta-table.detail.model';
import { MetaColumnDetailWrapperModel } from '../models/meta/meta-column.detail.model';
import { MetaDependencyDetailWrapperModel } from '../models/meta/meta-dependency.detail.model';
import { MetaSummaryModel } from '../models/meta/meta-summary.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { MetaPageDetailWrapperModel } from '../models/meta/meta-page.detail.model';
import { EMPTY } from 'rxjs';
import { MetaPageExpandedModel } from '../models/meta/meta-page.expanded.model';

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

  getMetaPageExpanded(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<MetaPageExpandedModel>('/metapages', 'application/vnd.pvims.expanded.v1+json', parameters);
  } 

  getAllMetaPages(): any {
    // Return all facilities from the API
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getMetaPages(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as MetaPageDetailWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<MetaPageDetailWrapperModel>(next.href, 'application/vnd.pvims.detail.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as MetaPageDetailWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getMetaPages(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaPageDetailWrapperModel>('/metapages', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  refresh(): any {
    return this.Post('meta', null);
  }  

}
