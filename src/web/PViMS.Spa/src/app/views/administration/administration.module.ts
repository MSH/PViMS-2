import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportRoutes } from './administration-routing.module';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { SharedModule } from 'app/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { RouterModule } from '@angular/router';
import { ConfigListComponent } from './system/config-list/config-list.component';
import { FacilityListComponent } from './system/facility-list/facility-list.component';
import { MedicineListComponent } from './reference/medicine-list/medicine-list.component';
import { UserListComponent } from './user/user-list/user-list.component';
import { LabTestListComponent } from './reference/lab-test-list/lab-test-list.component';
import { LabResultListComponent } from './reference/lab-result-list/lab-result-list.component';
import { ContactDetailListComponent } from './system/contact-detail-list/contact-detail-list.component';
import { HolidayListComponent } from './system/holiday-list/holiday-list.component';
import { ReportMetaViewComponent } from './system/report-meta-view/report-meta-view.component';
import { RoleListComponent } from './user/role-list/role-list.component';
import { CareEventListComponent } from './work/care-event-list/care-event-list.component';
import { DatasetElementListComponent } from './work/dataset-element-list/dataset-element-list.component';
import { DatasetListComponent } from './work/dataset-list/dataset-list.component';
import { EncounterTypeListComponent } from './work/encounter-type-list/encounter-type-list.component';
import { WorkPlanListComponent } from './work/work-plan-list/work-plan-list.component';
import { DatasetCategoryListComponent } from './work/dataset-category-list/dataset-category-list.component';
import { ConditionListComponent } from './reference/condition-list/condition-list.component';
import { AuditLogListComponent } from './audit-log-list/audit-log-list.component';
import { LandingComponent } from './landing/landing.component';
import { MeddraListComponent } from './reference/meddra-list/meddra-list.component';
import { ImportMeddraPopupComponent } from './reference/meddra-list/import-meddra-popup/import-meddra.popup.component';
import { FormListComponent } from './work/form-list/form-list.component';
import { FormPopupComponent } from './work/form-list/form-result-popup/form.popup.component';
import { FormDeletePopupComponent } from './work/form-list/form-delete-popup/form-delete.popup.component';
import { ConditionDeletePopupComponent } from './reference/condition-list/condition-delete-popup/condition-delete.popup.component';
import { ConditionPopupComponent } from './reference/condition-list/condition-popup/condition.popup.component';
import { LabTestSelectPopupComponent } from './shared/lab-test-select-popup/lab-test-select.popup.component';

@NgModule({
  declarations: [
    ConfigListComponent,
    ContactDetailListComponent,
    FacilityListComponent, 
    MedicineListComponent, 
    UserListComponent, 
    LabTestListComponent,
    LabResultListComponent,
    HolidayListComponent,
    ReportMetaViewComponent,
    RoleListComponent,
    CareEventListComponent,
    DatasetElementListComponent,
    DatasetListComponent,
    EncounterTypeListComponent,
    WorkPlanListComponent,
    DatasetCategoryListComponent,
    ConditionListComponent,
    ConditionDeletePopupComponent,
    ConditionPopupComponent,
    AuditLogListComponent,
    LandingComponent,
    MeddraListComponent,
    ImportMeddraPopupComponent,
    FormListComponent,
    FormDeletePopupComponent,
    FormPopupComponent,
    LabTestSelectPopupComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedComponentsModule,
    SharedMaterialModule,
    SharedModule,    
    TranslateModule,
    PerfectScrollbarModule,
    RouterModule.forChild(ReportRoutes)
  ],
  entryComponents:
  [
    ImportMeddraPopupComponent,
    ConditionPopupComponent,
    ConditionDeletePopupComponent,
    LabTestSelectPopupComponent
  ]  
})
export class AdministrationModule { }
