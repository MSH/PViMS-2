import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RouterModule } from '@angular/router';
import { ClinicalRoutes } from './clinical-routing.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedModule } from 'app/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { QuillModule } from 'ngx-quill';

import { EncounterSearchComponent } from './encounter/encounter-search/encounter-search.component';
import { AppointmentSearchComponent } from './appointment/appointment-search/appointment-search.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { EncounterViewComponent } from './encounter/encounter-view/encounter-view.component';
import { FormListComponent } from './form/formlist/formlist.component';
import { FormAComponent } from './form/form-a/form-a.component';
import { FormBComponent } from './form/form-b/form-b.component';
import { FormCComponent } from './form/form-c/form-c.component';
import { SynchroniseComponent } from './form/synchronise/synchronise.component';
import { WebcamModule } from 'ngx-webcam';
import { AttachmentCapturePopupComponent } from './form/attachment-capture-popup/attachment-capture.popup.component';
import { AttachmentViewPopupComponent } from './form/attachment-view-popup/attachment-view.popup.component';
import { PatientSearchComponent } from './patient/patient-search/patient-search.component';
import { PatientViewComponent } from './patient/patient-view/patient-view.component';
import { PatientUpdatePopupComponent } from './patient/patient-view/patient-update-popup/patient-update.popup.component';
import { PatientAddPopupComponent } from './patient/patient-search/patient-add-popup/patient-add.popup.component';
import { EncounterPopupComponent } from './patient/patient-view/encounter-popup/encounter.popup.component';
import { DnaPopupComponent } from './shared/dna-popup/dna.popup.component';
import { EncounterUpdatePopupComponent } from './encounter/encounter-view/encounter-update-popup/encounter-update.popup.component';
import { EncounterDeletePopupComponent } from './encounter/encounter-view/encounter-delete-popup/encounter-delete.popup.component';
import { ClinicalEventPopupComponent } from './shared/clinical-event-popup/clinical-event.popup.component';
import { ConditionPopupComponent } from './shared/condition-popup/condition.popup.component';
import { GenericDeletePopupComponent } from './shared/generic-delete-popup/generic-delete.popup.component';
import { ConditionViewPopupComponent } from './shared/condition-view-popup/condition-view.popup.component';
import { GenericArchivePopupComponent } from './shared/generic-archive-popup/generic-archive.popup.component';
import { ClinicalEventViewPopupComponent } from './shared/clinical-event-view-popup/clinical-event-view.popup.component';
import { FormDeletePopupComponent } from './form/form-delete-popup/form-delete.popup.component';
import { MedicationPopupComponent } from './shared/medication-popup/medication.popup.component';
import { LabTestPopupComponent } from './shared/lab-test-popup/lab-test.popup.component';
import { FeedbackSearchComponent } from './feedback/feedback-search/feedback-search.component';
import { CohortSearchComponent } from './cohort/cohort-search/cohort-search.component';
import { CohortPopupComponent } from './cohort/cohort-search/cohort-popup/cohort.popup.component';
import { CohortDeletePopupComponent } from './cohort/cohort-search/cohort-delete-popup/cohort-delete.popup.component';
import { CohortEnrolmentListComponent } from './cohort/cohort-enrolment-list/cohort-enrolment-list.component';
import { FormADRComponent } from './form/form-adr/form-adr.component';
import { LandingComponent } from './form/landing/landing.component';
import { AttachmentAddPopupComponent } from './shared/attachment-add-popup/attachment-add.popup.component';
import { ClinicalEventTaskPopupComponent } from './feedback/clinical-event-task-popup/clinical-event-task.popup.component';
import { ChangeTaskStatusPopupComponent } from './feedback/change-task-status-popup/change-task-status.popup.component';
import { FormADRMedicationPopupComponent } from './shared/form-adr-medication-popup/form-adr-medication.popup.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedComponentsModule,
    SharedMaterialModule,
    SharedModule,
    TranslateModule,
    PerfectScrollbarModule,
    QuillModule,
    WebcamModule,
    RouterModule.forChild(ClinicalRoutes)
  ],
  declarations: 
  [
    AppointmentSearchComponent,
    AttachmentAddPopupComponent,
    AttachmentCapturePopupComponent,
    AttachmentViewPopupComponent,
    ChangeTaskStatusPopupComponent,
    ClinicalEventPopupComponent,
    ClinicalEventTaskPopupComponent,
    ClinicalEventViewPopupComponent,
    CohortEnrolmentListComponent,
    CohortSearchComponent,
    CohortPopupComponent,
    CohortDeletePopupComponent,
    ConditionPopupComponent,
    ConditionViewPopupComponent,
    DnaPopupComponent,
    EncounterDeletePopupComponent,
    EncounterPopupComponent,
    EncounterSearchComponent,
    EncounterUpdatePopupComponent,
    EncounterViewComponent,
    FeedbackSearchComponent,
    FormAComponent,
    FormADRComponent,
    FormADRMedicationPopupComponent,
    FormBComponent,
    FormCComponent,
    FormDeletePopupComponent,
    FormListComponent,
    GenericArchivePopupComponent,
    GenericDeletePopupComponent,
    LabTestPopupComponent,
    LandingComponent,
    MedicationPopupComponent,
    PatientAddPopupComponent,
    PatientSearchComponent,
    PatientUpdatePopupComponent,
    PatientViewComponent,
    SynchroniseComponent
  ],
  entryComponents:
  [
    AttachmentAddPopupComponent,
    AttachmentCapturePopupComponent,
    AttachmentViewPopupComponent,
    ChangeTaskStatusPopupComponent,
    ClinicalEventPopupComponent,
    ClinicalEventTaskPopupComponent,
    ClinicalEventViewPopupComponent,
    ConditionPopupComponent,
    ConditionViewPopupComponent,
    FormADRMedicationPopupComponent,
    FormDeletePopupComponent,
    PatientAddPopupComponent,
    PatientUpdatePopupComponent,
    GenericDeletePopupComponent,
    GenericArchivePopupComponent,
    MedicationPopupComponent,
    LabTestPopupComponent,
    EncounterPopupComponent,
    EncounterUpdatePopupComponent,
    EncounterDeletePopupComponent,
    DnaPopupComponent,
    CohortPopupComponent,
    CohortDeletePopupComponent
  ]
})
export class ClinicalModule { }
