import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RouterModule } from '@angular/router';
import { AnalyticalRoutes } from './analytical-routing.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedModule } from 'app/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';

import { SpontaneousAnalyserComponent } from './analyser/spontaneous-analyser/spontaneous-analyser.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ActivityhistoryComponent } from './report/activity-history/activity-history.component';
import { ActivityStatusChangePopupComponent } from './report/activity-status-change-popup/activity-status-change.popup.component';
import { ReportSearchComponent } from './report/report-search/report-search.component';
import { NaranjoPopupComponent } from './report/naranjo-popup/naranjo.popup.component';
import { SetMeddraPopupComponent } from './report/set-meddra-popup/set-meddra.popup.component';
import { DatasetInstancePopupComponent } from './report/dataset-instance-popup/dataset-instance.popup.component';
import { LandingComponent } from './landing/landing.component';
import { WhoPopupComponent } from './report/who-popup/who.popup.component';
import { ActiveAnalyserComponent } from './analyser/active-analyser/active-analyser.component';

@NgModule({
  declarations: [
    SpontaneousAnalyserComponent, 
    ActiveAnalyserComponent, 
    ReportSearchComponent, 
    ActivityhistoryComponent,
    ActivityStatusChangePopupComponent,
    SetMeddraPopupComponent, 
    NaranjoPopupComponent,
    WhoPopupComponent,
    DatasetInstancePopupComponent,
    LandingComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedComponentsModule,
    SharedMaterialModule,
    SharedModule,
    TranslateModule,
    PerfectScrollbarModule,
    RouterModule.forChild(AnalyticalRoutes)
  ],
  entryComponents:
  [
    ActivityStatusChangePopupComponent,
    SetMeddraPopupComponent, 
    NaranjoPopupComponent,
    WhoPopupComponent,
    DatasetInstancePopupComponent
  ]  
})
export class AnalyticalModule { }
