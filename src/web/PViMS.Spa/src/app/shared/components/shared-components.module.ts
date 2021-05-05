import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedMaterialModule } from '../shared-material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { SharedPipesModule } from '../pipes/shared-pipes.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedDirectivesModule } from '../directives/shared-directives.module';

// ONLY REQUIRED FOR **SIDE** NAVIGATION LAYOUT
import { HeaderSideComponent } from './header-side/header-side.component';
import { SidebarSideComponent } from './sidebar-side/sidebar-side.component';

// ONLY REQUIRED FOR **TOP** NAVIGATION LAYOUT
import { HeaderTopComponent } from './header-top/header-top.component';
import { SidebarTopComponent } from './sidebar-top/sidebar-top.component';

// ONLY FOR DEMO
import { CustomizerComponent } from './customizer/customizer.component';

// ALWAYS REQUIRED 
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { SidenavComponent } from './sidenav/sidenav.component';
import { FooterComponent } from './footer/footer.component';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { AppComfirmComponent } from '../services/app-confirm/app-confirm.component';
import { AppLoaderComponent } from '../services/app-loader/app-loader.component';
import { ButtonLoadingComponent } from './button-loading/button-loading.component';
import { EgretSidebarComponent, EgretSidebarTogglerDirective } from './egret-sidebar/egret-sidebar.component';
import { BottomSheetShareComponent } from './bottom-sheet-share/bottom-sheet-share.component';
import { EgretExampleViewerComponent } from './example-viewer/example-viewer.component';
import { EgretExampleViewerTemplateComponent } from './example-viewer-template/example-viewer-template.component';
import { ViewErrorPopupComponent } from 'app/views/clinical/form/synchronise/viewerror-popup/viewerror.popup.component';
import { LabPopupComponent } from 'app/views/administration/reference/lab-test-list/lab-popup/lab.popup.component';
import { LabDeletePopupComponent } from 'app/views/administration/reference/lab-test-list/lab-delete-popup/lab-delete.popup.component';
import { LabResultPopupComponent } from 'app/views/administration/reference/lab-result-list/lab-result-popup/lab-result.popup.component';
import { LabResultDeletePopupComponent } from 'app/views/administration/reference/lab-result-list/lab-result-delete-popup/lab-result-delete.popup.component';
import { ConfigPopupComponent } from 'app/views/administration/system/config-list/config-popup/config.popup.component';
import { FacilityPopupComponent } from 'app/views/administration/system/facility-list/facility-popup/facility.popup.component';
import { ContactDetailPopupComponent } from 'app/views/administration/system/contact-detail-list/contact-detail-popup/contact-detail.popup.component';
import { HolidayPopupComponent } from 'app/views/administration/system/holiday-list/holiday-popup/holiday.popup.component';
import { HolidayDeletePopupComponent } from 'app/views/administration/system/holiday-list/holiday-delete-popup/holiday-delete.popup.component';
import { FacilityDeletePopupComponent } from 'app/views/administration/system/facility-list/facility-delete-popup/facility-delete.popup.component';
import { CareEventPopupComponent } from 'app/views/administration/work/care-event-list/care-event-popup/care-event.popup.component';
import { CareEventDeletePopupComponent } from 'app/views/administration/work/care-event-list/care-event-delete-popup/care-event-delete.popup.component';
import { DatasetElementPopupComponent } from 'app/views/administration/work/dataset-element-list/dataset-element-popup/dataset-element.popup.component';
import { DatasetElementDeletePopupComponent } from 'app/views/administration/work/dataset-element-list/dataset-element-delete-popup/dataset-element-delete.popup.component';
import { DatasetPopupComponent } from 'app/views/administration/work/dataset-list/dataset-popup/dataset.popup.component';
import { EncounterTypePopupComponent } from 'app/views/administration/work/encounter-type-list/encounter-type-popup/encounter-type.popup.component';
import { EncounterTypeDeletePopupComponent } from 'app/views/administration/work/encounter-type-list/encounter-type-delete-popup/encounter-type-delete.popup.component';
import { UserDeletePopupComponent } from 'app/views/administration/user/user-list/user-delete-popup/user-delete.popup.component';
import { UserAddPopupComponent } from 'app/views/administration/user/user-list/user-add-popup/user-add.popup.component';
import { UserUpdatePopupComponent } from 'app/views/administration/user/user-list/user-update-popup/user-update.popup.component';
import { DatasetDeletePopupComponent } from 'app/views/administration/work/dataset-list/dataset-delete-popup/dataset-delete.popup.component';
import { PasswordResetPopupComponent } from 'app/views/administration/user/user-list/password-reset-popup/password-reset.popup.component';
import { FormAConditionsPopupComponent } from 'app/views/clinical/form/form-a/form-a-conditions-popup/form-a-conditions.popup.component';
import { FormALabsPopupComponent } from 'app/views/clinical/form/form-a/form-a-labs-popup/form-a-labs.popup.component';
import { FormAMedicationsPopupComponent } from 'app/views/clinical/form/form-a/form-a-medications-popup/form-a-medications.popup.component';
import { FormBConditionsPopupComponent } from 'app/views/clinical/form/form-b/form-b-conditions-popup/form-b-conditions.popup.component';
import { FormBLabsPopupComponent } from 'app/views/clinical/form/form-b/form-b-labs-popup/form-b-labs.popup.component';
import { FormBMedicationsPopupComponent } from 'app/views/clinical/form/form-b/form-b-medications-popup/form-b-medications.popup.component';
import { FormBAdversePopupComponent } from 'app/views/clinical/form/form-b/form-b-adverse-popup/form-b-adverse.popup.component';
import { FormCompletePopupComponent } from 'app/views/clinical/form/form-complete-popup/form-complete.popup.component';
import { PingComponent } from './ping/ping.component';
import { UserProfilePopupComponent } from 'app/views/security/user-profile/user-profile.popup.component';
import { AppointmentPopupComponent } from 'app/views/clinical/patient/patient-view/appointment-popup/appointment.popup.component';
import { EnrolmentPopupComponent } from 'app/views/clinical/patient/patient-view/enrolment-popup/enrolment.popup.component';
import { DeenrolmentPopupComponent } from 'app/views/clinical/patient/patient-view/deenrolment-popup/deenrolment.popup.component';
import { MedicationListPopupComponent } from 'app/views/analytical/report/report-search/medications-popup/medicationlist.popup.component';
import { AttachmentPopupComponent } from 'app/views/clinical/patient/patient-view/attachment-popup/attachment.popup.component';
import { ConfirmPopupComponent } from './popup/confirm.popup.component';
import { ErrorPopupComponent } from './popup/error.popup.component';
import { InfoPopupComponent } from './popup/info.popup.component';
import { ConceptSelectPopupComponent } from './popup/concept-select-popup/concept-select.popup.component';
import { AboutPopupComponent } from './about/about.popup.component';
import { TimeoutComponent } from './timeout/timeout.component';
import { MeddraSelectPopupComponent } from './popup/meddra-select-popup/meddra-select.popup.component';
import { DatasetCategoryPopupComponent } from 'app/views/administration/work/dataset-list/dataset-category-list/dataset-category-popup/dataset-category.popup.component';

const components = [
  HeaderTopComponent,
  HeaderSideComponent,
  BreadcrumbComponent,

  SidebarTopComponent,
  SidebarSideComponent,
  SidenavComponent,
  
  NotificationsComponent,

  AdminLayoutComponent,
  AuthLayoutComponent,
  
  AppComfirmComponent,
  ConfirmPopupComponent,
  ErrorPopupComponent,
  InfoPopupComponent,
  AppLoaderComponent,
  UserProfilePopupComponent,

  // Form popups
  FormAConditionsPopupComponent,
  FormALabsPopupComponent,
  FormAMedicationsPopupComponent,
  FormBConditionsPopupComponent,
  FormBLabsPopupComponent,
  FormBMedicationsPopupComponent,
  FormBAdversePopupComponent,
  FormCompletePopupComponent,
  
  // Patient view popups
  AppointmentPopupComponent,
  AttachmentPopupComponent,
  EnrolmentPopupComponent,
  DeenrolmentPopupComponent,
  MedicationListPopupComponent,

  // Admin popups
  CareEventPopupComponent,
  CareEventDeletePopupComponent,   
  ConfigPopupComponent,
  ContactDetailPopupComponent,
  DatasetPopupComponent,
  DatasetDeletePopupComponent,
  DatasetCategoryPopupComponent,
  DatasetElementPopupComponent,
  DatasetElementDeletePopupComponent,
  EncounterTypePopupComponent,
  EncounterTypeDeletePopupComponent,
  FacilityPopupComponent,
  FacilityDeletePopupComponent,
  HolidayPopupComponent,
  HolidayDeletePopupComponent,
  LabPopupComponent,
  LabDeletePopupComponent,
  LabResultPopupComponent,
  LabResultDeletePopupComponent,
  PasswordResetPopupComponent,
  UserAddPopupComponent,
  UserDeletePopupComponent,
  UserUpdatePopupComponent,

  ConceptSelectPopupComponent,
  MeddraSelectPopupComponent,

  ViewErrorPopupComponent,
  AboutPopupComponent,

  CustomizerComponent,
  ButtonLoadingComponent,
  EgretSidebarComponent,
  FooterComponent,
  EgretSidebarTogglerDirective,
  BottomSheetShareComponent,
  EgretExampleViewerComponent,
  EgretExampleViewerTemplateComponent,
  PingComponent,
  TimeoutComponent
]

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslateModule,
    FlexLayoutModule,
    ReactiveFormsModule,
    PerfectScrollbarModule,
    SharedPipesModule,
    SharedDirectivesModule,
    SharedMaterialModule
  ],
  declarations: components,
  entryComponents: [
    AppComfirmComponent, 
    AppLoaderComponent, 
    BottomSheetShareComponent, 
    ConfirmPopupComponent, 
    ErrorPopupComponent, 
    InfoPopupComponent,
    UserProfilePopupComponent,
    
    FormAConditionsPopupComponent,
    FormALabsPopupComponent,
    FormAMedicationsPopupComponent,
    FormBConditionsPopupComponent,
    FormBLabsPopupComponent,
    FormBMedicationsPopupComponent,
    FormBAdversePopupComponent,
    FormCompletePopupComponent,

    AppointmentPopupComponent,
    AttachmentPopupComponent,
    EnrolmentPopupComponent,
    DeenrolmentPopupComponent,
    MedicationListPopupComponent,
    
    CareEventPopupComponent,
    CareEventDeletePopupComponent,   
    ContactDetailPopupComponent,
    ConfigPopupComponent,
    DatasetPopupComponent,
    DatasetDeletePopupComponent,
    DatasetElementPopupComponent,
    DatasetCategoryPopupComponent,
    DatasetElementDeletePopupComponent,
    EncounterTypePopupComponent,
    EncounterTypeDeletePopupComponent,
    FacilityPopupComponent,
    FacilityDeletePopupComponent,
    HolidayPopupComponent,
    HolidayDeletePopupComponent,
    LabPopupComponent,
    LabDeletePopupComponent,
    LabResultPopupComponent,
    LabResultDeletePopupComponent,
    PasswordResetPopupComponent,
    UserAddPopupComponent,
    UserDeletePopupComponent,
    UserUpdatePopupComponent,

    ConceptSelectPopupComponent,
    MeddraSelectPopupComponent,
    
    ViewErrorPopupComponent,
    AboutPopupComponent
    
  ],
  exports: components
})
export class SharedComponentsModule {}