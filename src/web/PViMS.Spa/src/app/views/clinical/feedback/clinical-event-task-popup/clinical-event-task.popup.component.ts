import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { switchMap } from 'rxjs/operators';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { GridModel } from 'app/shared/models/grid.model';
import { TaskModel } from 'app/shared/models/report-instance/task.model';
import { TaskCommentsPopupComponent } from 'app/shared/components/popup/task-comments-popup/task-comments.popup.component';
import { ChangeTaskStatusPopupComponent } from '../change-task-status-popup/change-task-status.popup.component';
import { forkJoin, of } from 'rxjs';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientCustomAttributesForUpdateModel } from 'app/shared/models/patient/patient-custom-attributes-for-update.model';
import { PatientClinicalEventExpandedModel } from 'app/shared/models/patient/patient-clinical-event.expanded.model';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';
import { PatientClinicalEventForUpdateModel } from 'app/shared/models/patient/patient-clinical-event-for-update.model';
import { AttachmentAddPopupComponent } from '../../shared/attachment-add-popup/attachment-add.popup.component';
import { PatientMedicationForUpdateModel } from 'app/shared/models/patient/patient-medication-for-update.model';
import { FormADRMedicationPopupComponent } from '../../shared/form-adr-medication-popup/form-adr-medication.popup.component';
import { PatientMedicationDetailModel } from 'app/shared/models/patient/patient-medication.detail.model';
import { PatientExpandedModel } from 'app/shared/models/patient/patient.expanded.model';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
const moment =  _moment;

@Component({
  templateUrl: './clinical-event-task.popup.component.html',
  styles: [`
    .mat-column-executed-date { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-activity { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-execution-event { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-comments { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-source { flex: 0 0 18% !important; width: 18% !important; }
    .mat-column-description { flex: 0 0 30% !important; width: 30% !important; }
    .mat-column-task-type { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-task-status { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-file-name { flex: 0 0 85% !important; width: 85% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],  
  animations: egretAnimations
})
export class ClinicalEventTaskPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();

  public firstFormGroup: FormGroup;
  public secondFormGroup: FormGroup;
  public thirdFormGroup: FormGroup;
  public fourthFormGroup: FormGroup;
  public fifthFormGroup: FormGroup;
  public sixthFormGroup: FormGroup;  

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ClinicalEventTaskPopupData,
    public dialogRef: MatDialogRef<ClinicalEventTaskPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService,
    protected dialog: MatDialog
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.firstFormGroup = this._formBuilder.group({
    });
    self.secondFormGroup = this._formBuilder.group({
      ethnicGroup: [null, Validators.required],
    });
    self.thirdFormGroup = this._formBuilder.group({
      onsetDate: ['', Validators.required],
      regimen: [null, Validators.required],
      sourceDescription: [null, [Validators.required, Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      isSerious: [null],
      seriousness: [null],
      classification: [null, Validators.required],
      weight: [null, [Validators.required, Validators.min(1), Validators.max(159)]],
      height: [null, [Validators.required, Validators.min(1), Validators.max(259)]],
      allergy: ['', [Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      pregnancyStatus: [null],
      comorbidities: ['', [Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
    });
    self.fourthFormGroup = this._formBuilder.group({
      treatmentGiven: [null],
      treatmentDetails: ['', [Validators.maxLength(300), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      outcome: [null, Validators.required],
      dateOfRecovery: [''],
      dateOfDeath: [''],
      sequlae: ['', [Validators.maxLength(300), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      interventions: ['', [Validators.maxLength(300), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
    });
    self.fifthFormGroup = this._formBuilder.group({
    });
    self.sixthFormGroup = this._formBuilder.group({
      reporterName: ['', [Validators.maxLength(100), Validators.pattern("[-a-zA-Z ']*")]],
      contactNumber: ['', [Validators.maxLength(30), Validators.pattern("[-0-9+']*")]],
      emailAddress: ['', Validators.maxLength(100)],
      profession: [null]
    });

    self.getCustomAttributeList();    
    self.viewModel.historyGrid.setupBasic(null, null, null);
    self.viewModel.taskGrid.setupBasic(null, null, null);
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.clinicalEventId > 0) {
      self.loadData();
    }
  } 
  
  setStep(step: number): void {
    this.viewModel.currentStep = step;
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    requestArray.push(self.patientService.getPatientClinicalEventExpanded(self.data.patientId, self.data.clinicalEventId));
    requestArray.push(self.patientService.getPatientExpanded(self.data.patientId));

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.CLog(data[0], 'get clinical event expanded')
          self.CLog(data[1], 'get patient detail')

          self.loadGrids(data[1] as PatientExpandedModel, data[0] as PatientClinicalEventExpandedModel);

          self.loadDataForSecondForm(data[1] as PatientExpandedModel);
          self.loadDataForThirdForm(data[0] as PatientClinicalEventExpandedModel);
          self.loadDataForFourthForm(data[0] as PatientClinicalEventExpandedModel);
          self.loadDataForSixthForm(data[0] as PatientClinicalEventExpandedModel);

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });    
  }

  private loadGrids(patientModel: PatientExpandedModel, clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;
    self.viewModel.historyGrid.updateBasic(clinicalEventModel.activity);
    self.viewModel.taskGrid.updateBasic(clinicalEventModel.tasks);

    self.viewModel.medications = self.mapMedicationForUpdateModels(patientModel.patientMedications);
    self.viewModel.medicationGrid.updateBasic(self.viewModel.medications);
  }

  private mapMedicationForUpdateModels(sourceMedications: PatientMedicationDetailModel[]): PatientMedicationForUpdateModel[] {
    let medications: PatientMedicationForUpdateModel[] = [];

    let index = 0;
    sourceMedications.forEach(sourceMedication => {
      index++;
      let medication: PatientMedicationForUpdateModel = {
        id: sourceMedication.id,
        index,
        medication: sourceMedication.medication,
        sourceDescription: sourceMedication.sourceDescription,
        conceptId: sourceMedication.conceptId,
        productId: sourceMedication.productId,
        startDate: sourceMedication.startDate,
        endDate: sourceMedication.endDate,
        dose: sourceMedication.dose,
        doseFrequency: sourceMedication.doseFrequency,
        doseUnit: sourceMedication.doseUnit,
        attributes: []
      };
      
      sourceMedication.medicationAttributes.forEach(sourceAttribute => {
        let attribute: AttributeValueForPostModel = {
          id: sourceAttribute.id,
          value: sourceAttribute.value
        };
        medication.attributes.push(attribute);
      });

      medications.push(medication);
    });

    return medications;
  }

  private loadDataForSecondForm(patientModel: PatientExpandedModel)
  {
    let self = this;
    let ethnicGroupAttribute = patientModel.patientAttributes.find(pa => pa.key == 'Ethnic Group');

    self.updateForm(self.secondFormGroup, { 'ethnicGroup': ethnicGroupAttribute.value })
  }

  private loadDataForThirdForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.thirdFormGroup, { 'onsetDate': clinicalEventModel.onsetDate })
    self.updateForm(self.thirdFormGroup, { 'regimen': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'regimen')?.value })
    self.updateForm(self.thirdFormGroup, { 'sourceDescription': clinicalEventModel.sourceDescription })
    self.updateForm(self.thirdFormGroup, { 'isSerious': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'is the adverse event serious?')?.value })
    self.updateForm(self.thirdFormGroup, { 'seriousness': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'seriousness')?.value })
    self.updateForm(self.thirdFormGroup, { 'classification': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'classification')?.value })
    self.updateForm(self.thirdFormGroup, { 'weight': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'weight (kg)')?.value })
    self.updateForm(self.thirdFormGroup, { 'height': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'height (cm)')?.value })
    self.updateForm(self.thirdFormGroup, { 'allergy': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'any known allergy')?.value })
    self.updateForm(self.thirdFormGroup, { 'pregnancyStatus': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'pregnancy status')?.value })
    self.updateForm(self.thirdFormGroup, { 'comorbidities': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'comorbidities')?.value })
  }

  private loadDataForFourthForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.fourthFormGroup, { 'treatmentGiven': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'was treatment given?')?.value })
    self.updateForm(self.fourthFormGroup, { 'treatmentDetails': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'treatment details')?.value })
    self.updateForm(self.fourthFormGroup, { 'outcome': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'outcome')?.value })
    self.updateForm(self.fourthFormGroup, { 'dateOfRecovery': clinicalEventModel.resolutionDate })
    self.updateForm(self.fourthFormGroup, { 'sequlae': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'sequlae details')?.value })
    self.updateForm(self.fourthFormGroup, { 'interventions': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'interventions')?.value })
  }

  private loadDataForSixthForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.sixthFormGroup, { 'reporterName': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'name of reporter')?.value })
    self.updateForm(self.sixthFormGroup, { 'contactNumber': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'contact number')?.value })
    self.updateForm(self.sixthFormGroup, { 'emailAddress': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'email address')?.value })
    self.updateForm(self.sixthFormGroup, { 'profession': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'profession')?.value })
  }

  openTaskCommentsPopUp(reportInstanceTaskId: number) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(TaskCommentsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { workFlowGuid: self.viewModel.workFlowId, title: 'Comments', reportInstanceId: self.data.reportInstanceId, reportInstanceTaskId }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        self.loadData();
      })
  }

  openChangeTaskStatusPopUp(reportInstanceTaskId: number) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(ChangeTaskStatusPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { workFlowGuid: self.viewModel.workFlowId, title: 'Change Task Status', reportInstanceId: self.data.reportInstanceId, reportInstanceTaskId }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openAttachmentPopUp() {
    let self = this;
    let title = 'Add Attachment';
    let dialogRef: MatDialogRef<any> = self.dialog.open(AttachmentAddPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.viewModel.attachments.push(res);
        self.viewModel.attachmentGrid.updateBasic(self.viewModel.attachments);
        self.makeSaveButtonVisible();
      })
  }

  removeAttachment(index: number): void {
    let self = this;
    self.viewModel.attachments.splice(index, 1)
    self.viewModel.attachmentGrid.updateBasic(self.viewModel.attachments);

    this.notify("Attachment removed successfully!", "Success");
  }

openMedicationPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Medication' : 'Update Medication';
    let indexToUse = isNew ? self.viewModel.medications.length + 1 : data.index;
    
    let existingMedication = null;
    if (!isNew) {
      let actualIndex = self.viewModel.medications.findIndex(m => m.index == indexToUse);
      self.CLog(actualIndex, 'actual index');
      existingMedication = self.viewModel.medications[actualIndex];
    }
    
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormADRMedicationPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { title: title, medicationId: isNew ? 0: existingMedication.id, index: indexToUse, existingMedication }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        if(isNew) {
          self.viewModel.medications.push(res);
        }
        else {
          let actualIndex = self.viewModel.medications.findIndex(m => m.index == indexToUse);
          self.viewModel.medications[actualIndex] = res;
        }
    
        self.viewModel.medicationGrid.updateBasic(self.viewModel.medications);
        self.fifthFormGroup.reset();
        self.makeSaveButtonVisible();
      })
  }

  removeMedication(index: number): void {
    let self = this;

    let actualIndex = self.viewModel.medications.findIndex(m => m.index == index);
    self.viewModel.medications.splice(actualIndex, 1)
    this.viewModel.medicationGrid.updateBasic(self.viewModel.medications);

    this.notify("Medication removed successfully!", "Medication");
  }  

  submit(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    var patientCustomAttributesForUpdate = self.preparePatientCustomAttributesForUpdateModel();
    requestArray.push(this.patientService.updatePatientCustomAttributes(self.data.patientId, patientCustomAttributesForUpdate));

    var clinicalEventForUpdate = self.prepareClinicalEventForUpdateModel();
    requestArray.push(this.patientService.savePatientClinicalEvent(self.data.patientId, self.data.clinicalEventId, clinicalEventForUpdate));

    self.viewModel.medications.forEach(medicationForUpdate => {
      requestArray.push(this.patientService.savePatientMedication(self.data.patientId, medicationForUpdate.id, medicationForUpdate));
    });

    self.viewModel.attachments.forEach(attachmentForUpdate => {
      requestArray.push(this.patientService.saveAttachment(self.data.patientId, attachmentForUpdate.file, attachmentForUpdate.description));
    });

    forkJoin(requestArray)
    .subscribe(
      data => {
        self.setBusy(false);
        self.notify('Form updated successfully!', 'Success');

        self.secondFormGroup.markAsPristine();
        self.thirdFormGroup.markAsPristine();
        self.fourthFormGroup.markAsPristine();
        self.fifthFormGroup.markAsPristine();
        self.sixthFormGroup.markAsPristine();

        self.loadData();
      },
      error => {
        self.setBusy(false);        
        this.handleError(error, "Error updating form");
      });
  }

  private preparePatientCustomAttributesForUpdateModel(): PatientCustomAttributesForUpdateModel {
    let self = this;

    const attributesForUpdate: AttributeValueForPostModel[] = [];
    attributesForUpdate.push(self.prepareAttributeValue('ethnic group', 'ethnicGroup', self.secondFormGroup));
    
    const patientCustomAttributesForUpdate: PatientCustomAttributesForUpdateModel = 
    {
      attributes: attributesForUpdate
    };

    return patientCustomAttributesForUpdate;
  }

  private prepareAttributeValue(attributeKey: string, formKey: string, sourceForm: FormGroup): AttributeValueForPostModel {
    const self = this;
    let customAttribute = self.viewModel.customAttributeList.find(ca => ca.attributeKey.toLowerCase() == attributeKey.toLowerCase());
    if(customAttribute == null) {
      return null;
    }

    const attributeForPost: AttributeValueForPostModel = {
      id: customAttribute.id,
      value: sourceForm.get(formKey).value
    }    
    return attributeForPost;
  }

  private prepareClinicalEventForUpdateModel(): PatientClinicalEventForUpdateModel {
    let self = this;

    let onsetDate = self.thirdFormGroup.get('onsetDate').value;
    if(moment.isMoment(self.thirdFormGroup.get('onsetDate').value)) {
      onsetDate = self.thirdFormGroup.get('onsetDate').value.format('YYYY-MM-DD');
    }
    let resolutionDate = '';
    if(moment.isMoment(self.fourthFormGroup.get('dateOfRecovery').value)) {
      resolutionDate = self.fourthFormGroup.get('dateOfRecovery').value.format('YYYY-MM-DD');
    }
    else {
      if (self.fourthFormGroup.get('dateOfRecovery').value != '') {
        resolutionDate = self.fourthFormGroup.get('dateOfRecovery').value;
      }
    }

    const attributesForUpdate: AttributeValueForPostModel[] = [];

    attributesForUpdate.push(self.prepareAttributeValue('regimen', 'regimen', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('is the adverse event serious?', 'isSerious', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('seriousness', 'seriousness', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('classification', 'classification', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('weight (kg)', 'weight', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('height (cm)', 'height', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('any known allergy', 'allergy', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('pregnancy status', 'pregnancyStatus', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('comorbidities', 'comorbidities', self.thirdFormGroup));
    
    attributesForUpdate.push(self.prepareAttributeValue('was treatment given?', 'treatmentGiven', self.fourthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('treatment details', 'treatmentDetails', self.fourthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('outcome', 'outcome', self.fourthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('sequlae details', 'sequlae', self.fourthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('interventions', 'interventions', self.fourthFormGroup));

    attributesForUpdate.push(self.prepareAttributeValue('name of reporter', 'reporterName', self.sixthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('contact number', 'contactNumber', self.sixthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('email address', 'emailAddress', self.sixthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('profession', 'profession', self.sixthFormGroup));

    const clinicalEventForUpdate: PatientClinicalEventForUpdateModel = 
    {
      id: self.data.clinicalEventId,
      index: 1,
      onsetDate,
      sourceDescription: self.thirdFormGroup.get('sourceDescription').value,
      resolutionDate,
      sourceTerminologyMedDraId: null,
      attributes: attributesForUpdate
    };

    return clinicalEventForUpdate;
  }  

  private getCustomAttributeList(): void {
    let self = this;

    const requestArray = [];

    requestArray.push(self.customAttributeService.getAllCustomAttributes('PatientClinicalEvent'));
    requestArray.push(self.customAttributeService.getAllCustomAttributes('Patient'));

    forkJoin(requestArray)
      .pipe(
        switchMap((values: any[]) => {
          const mergeAttributeList: CustomAttributeDetailModel[] = [];

          values[0].forEach((attribute) => {
            mergeAttributeList.push(attribute);
          });
          values[1].forEach((attribute) => {
            mergeAttributeList.push(attribute);
          });

          return of(mergeAttributeList);
        })
      )
      .subscribe(
        data => {
          self.viewModel.customAttributeList = data;
        },
        error => {
          this.handleError(error, "Error loading attributes");
        });    
  }

  private makeSaveButtonVisible(): void {
    let self = this;
    self.firstFormGroup.markAsDirty();
  }
}

class ViewModel {
  historyGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['executed-date', 'activity', 'execution-event', 'comments']);
  
  taskGrid: GridModel<TaskModel> =
      new GridModel<TaskModel>
          (['source', 'description', 'task-type', 'task-status', 'actions']);

  attachmentGrid: GridModel<FormAttachmentModel> =
    new GridModel<FormAttachmentModel>
        (['file-name', 'actions']);
  attachments: FormAttachmentModel[] = [];
  
  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'start-date', 'dose', 'actions']);
  medications: PatientMedicationForUpdateModel[] = [];

  workFlowId = '892F3305-7819-4F18-8A87-11CBA3AEE219';
  currentStep = 1;

  customAttributeKey = 'Case Number';
  customAttributeList: CustomAttributeDetailModel[] = [];
}

class GridRecordModel {
  activity: string;
  executionEvent: string;
  executedDate: string;
  comments: string;
}

export interface ClinicalEventTaskPopupData {
  patientId: number;
  clinicalEventId: number;
  reportInstanceId: number;
  title: string;
}

class MedicationGridRecordModel {
  id: number;
  index: number;
  medication: string;
  dose: string;
  doseUnit: string;
  startDate: string;
  endDate: string;
}