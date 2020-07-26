import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { DatasetDetailWrapperModel } from '../models/work/dataset.detail.model';
import { DatasetCategoryDetailWrapperModel } from '../models/work/dataset-category.detail.model';
import { DatasetInstanceDetailWrapperModel } from '../models/dataset/dataset-instance.detail.model';

@Injectable({ providedIn: 'root' })
export class DatasetService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getDatasets(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<DatasetDetailWrapperModel>('/datasets', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getDatasetCategories(datasetid: number, filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<DatasetCategoryDetailWrapperModel>(`/datasets/${datasetid}/categories`, 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getDatasetInstanceDetail(datasetid: number, id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<DatasetInstanceDetailWrapperModel>(`/datasets/${datasetid}/instances`, 'application/vnd.pvims.detail.v1+json', parameters);
  } 

  saveDataset(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`datasets`, model);
    }
    else {
      return this.Put(`datasets/${id}`, model);
    }
  }

  saveDatasetCategory(datasetid: number, id: number, model: any): any {
    if(id == 0) {
      return this.Post(`datasets/${datasetid}/categories`, model);
    }
    else {
      return this.Put(`datasets/${datasetid}/categories/${id}`, model);
    }
  }

  deleteDataset(id: number): any {
    return this.Delete(`datasets/${id}`);
  } 
  
  saveSpontaneousInstance(id: number, allModels: any[]): any {
    console.log(allModels);
    return this.Put(`datasets/${id}/instances`, allModels);    
  }
}
