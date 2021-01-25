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
import { DatasetCategoryListComponent } from './work/dataset-list/dataset-category-list/dataset-category-list.component';
import { DatasetCategoryElementListComponent } from './work/dataset-list/dataset-category-element-list/dataset-category-element-list.component';
import { DatasetCategoryElementDeletePopupComponent } from './work/dataset-list/dataset-category-element-list/dataset-category-element-delete-popup/dataset-category-element-delete.popup.component';
import { DatasetCategoryDeletePopupComponent } from './work/dataset-list/dataset-category-list/dataset-category-delete-popup/dataset-category-delete.popup.component';
import { DatasetElementSelectPopupComponent } from './shared/dataset-element-select-popup/dataset-element-select.popup.component';
import { DatasetCategoryElementPopupComponent } from './work/dataset-list/dataset-category-element-list/dataset-category-element-popup/dataset-category-element.popup.component';
import { ConceptListComponent } from './reference/concept-list/concept-list.component';
import { MedicationPopupComponent } from './reference/medicine-list/medication-popup/medication.popup.component';
import { MedicationDeletePopupComponent } from './reference/medicine-list/medication-delete-popup/medication-delete.popup.component';
import { ConceptPopupComponent } from './reference/concept-list/concept-popup/concept.popup.component';
import { GenericDeletePopupComponent } from './shared/generic-delete-popup/generic-delete.popup.component';

@NgModule({
  declarations: [
    AuditLogListComponent,
    CareEventListComponent,
    ConceptListComponent,
    ConceptPopupComponent,
    ConfigListComponent,
    ConditionListComponent,
    ConditionDeletePopupComponent,
    ConditionPopupComponent,
    ContactDetailListComponent,
    DatasetCategoryListComponent,
    DatasetCategoryDeletePopupComponent,
    DatasetCategoryElementPopupComponent,
    DatasetCategoryElementListComponent,
    DatasetCategoryElementDeletePopupComponent,
    DatasetListComponent,
    DatasetElementListComponent,
    DatasetElementSelectPopupComponent,
    FacilityListComponent, 
    FormListComponent,
    FormDeletePopupComponent,
    FormPopupComponent,
    HolidayListComponent,
    LabResultListComponent,
    LabTestListComponent,
    LabTestSelectPopupComponent,
    MeddraListComponent,
    ImportMeddraPopupComponent,
    MedicineListComponent,
    MedicationPopupComponent,
    MedicationDeletePopupComponent,
    RoleListComponent,
    UserListComponent, 
    WorkPlanListComponent,
    ReportMetaViewComponent,
    EncounterTypeListComponent,
    LandingComponent,
    GenericDeletePopupComponent
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
    ConceptPopupComponent,
    ConditionPopupComponent,
    ConditionDeletePopupComponent,
    DatasetCategoryDeletePopupComponent,
    DatasetCategoryElementPopupComponent,
    DatasetCategoryElementDeletePopupComponent,
    DatasetElementSelectPopupComponent,
    GenericDeletePopupComponent,
    ImportMeddraPopupComponent,
    LabTestSelectPopupComponent,
    MedicationPopupComponent,
    MedicationDeletePopupComponent
  ]  
})
export class AdministrationModule { }
