import { AfterViewInit, Component, Inject, OnDestroy, OnInit, SkipSelf } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { EventService } from 'app/shared/services/event.service';
import { PatientService } from 'app/shared/services/patient.service';
import { finalize, map, takeUntil } from 'rxjs/operators';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AttachmentAddPopupComponent } from '../attachment-add-popup/attachment-add.popup.component';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';
import { GridModel } from 'app/shared/models/grid.model';
import { FormADRMedicationPopupComponent } from './form-adr-medication-popup/form-adr-medication.popup.component';
import { PatientMedicationForUpdateModel } from 'app/shared/models/patient/patient-medication-for-update.model';
import { PatientClinicalEventForUpdateModel } from 'app/shared/models/patient/patient-clinical-event-for-update.model';
import { forkJoin } from 'rxjs';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { _routes } from 'app/config/routes';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientMedicationDetailModel } from 'app/shared/models/patient/patient-medication.detail.model';
const moment =  _moment;

@Component({
  templateUrl: './form-adr.component.html',
  animations: egretAnimations,
  styleUrls: ['./form-adr.component.scss'],  
})
export class FormADRComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  public scrollConfig = {}

  viewModel: ViewModel = new ViewModel();
  public viewModelForm: FormGroup;
  public firstFormGroup: FormGroup;
  public secondFormGroup: FormGroup;
  public thirdFormGroup: FormGroup;
  public fourthFormGroup: FormGroup;
  public fifthFormGroup: FormGroup;
  public sixthFormGroup: FormGroup;

  constructor(
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService,
    protected dialog: MatDialog) 
  { 
    super(_router, _location, popupService, accountService, eventService);
  }

  ngOnInit(): void {
    const self = this;
    
    self.viewModelForm = self._formBuilder.group({
      formCompleted: ['']
    });
    self.firstFormGroup = this._formBuilder.group({
      customAttributeKey: [this.viewModel.customAttributeKey],
      customAttributeValue: ['', Validators.required],
      firstName: [''],
      lastName: [''],
      gender: [''],
      address: [''],
      contactNumber: [''],
      dateOfBirth: [''],
      facilityName: [''],
      facilityRegion: [''],
    });
    self.secondFormGroup = this._formBuilder.group({
      ethnicGroup: [null, Validators.required],
    });
    self.thirdFormGroup = this._formBuilder.group({
      dateOfOnset: ['', Validators.required],
      regimen: [null, Validators.required],
      sourceDescription: [null, [Validators.required, Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      isSerious: [null],
      seriousness: [null],
      weight: [null, [Validators.required, Validators.min(0), Validators.max(159)]],
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
  }

  ngAfterViewInit(): void {
    let self = this;
    // if (self.id > 0) {
    //   self.loadData();
    // }
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormADRComponent.name);
  }   

  nextStep(): void {
    this.viewModel.currentStep ++;
  }

  previousStep(): void {
    this.viewModel.currentStep --;
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.searchPatientByCondition(self.firstFormGroup.value)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        this.CLog(result);
        if(result == null) {
          self.viewModel.errorFindingPatient = true;
          self.viewModel.patientFound = false;
        }
        else {
          self.viewModel.patientId = result.id;
          self.viewModel.errorFindingPatient = false;
          self.viewModel.patientFound = true;

          self.updateForm(self.firstFormGroup, result);
          self.updateForm(self.firstFormGroup, {gender: self.getValueFromAttribute(result.patientAttributes, "Gender")});
          self.updateForm(self.firstFormGroup, {contactNumber: self.getValueFromAttribute(result.patientAttributes, "Contact Number")});
          self.updateForm(self.firstFormGroup, {address: self.getValueFromAttribute(result.patientAttributes, "Address")});
          
          self.viewModel.medications = self.mapMedicationForUpdateModels(result.patientMedications);
          self.viewModel.medicationGrid.updateBasic(self.viewModel.medications);

          self.CLog(self.viewModel.medications);
        }
      }, error => {
        self.handleError(error, "Error fetching patient");
      });
  }

  getCustomAttributeList(): void {
    let self = this;

    self.customAttributeService.getAllCustomAttributes('PatientClinicalEvent')
      .subscribe(result => {
        self.viewModel.customAttributeList = result;
      }, error => {
        self.throwError(error, error.statusText);
      });
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
        self.CLog(self.viewModel.attachments);
      })
  }

  removeAttachment(index: number): void {
    let self = this;
    self.viewModel.attachments.splice(index, 1)

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
      width: '720px',
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
      })
  }

  removeMedication(index: number): void {
    let self = this;

    let actualIndex = self.viewModel.medications.findIndex(m => m.index == index);
    self.viewModel.medications.splice(actualIndex, 1)
    this.viewModel.medicationGrid.updateBasic(self.viewModel.medications);

    this.notify("Medication removed successfully!", "Medication");
  }

  save(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    var clinicalEventForUpdate = self.prepareClinicalEventForUpdateModel();
    requestArray.push(this.patientService.savePatientClinicalEvent(self.viewModel.patientId, 0, clinicalEventForUpdate));

    self.viewModel.medications.forEach(medicationForUpdate => {
      requestArray.push(this.patientService.savePatientMedication(self.viewModel.patientId, medicationForUpdate.id, medicationForUpdate));
    });

    forkJoin(requestArray)
    .subscribe(
      data => {
        console.log(data);
        self.setBusy(false);
        self.notify('Form added successfully!', 'Success');
        self._router.navigate([_routes.clinical.forms.landing]);
      },
      error => {
        this.handleError(error, "Error adding form");
      });
  }

  private getValueFromAttribute(attributes: AttributeValueModel[], key: string): string {
    let attribute = attributes.find(a => a.key == key);
    if(attribute?.selectionValue != '') {
      return attribute?.selectionValue;
    }
     return attribute?.value;
  }

  private prepareClinicalEventForUpdateModel(): PatientClinicalEventForUpdateModel {
    let self = this;
    let onsetDate = self.thirdFormGroup.get('dateOfOnset').value.format('YYYY-MM-DD');
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
      id: 0,
      index: 1,
      onsetDate,
      sourceDescription: self.thirdFormGroup.get('sourceDescription').value,
      resolutionDate,
      sourceTerminologyMedDraId: null,
      attributes: attributesForUpdate
    };

    return clinicalEventForUpdate;
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
}

class ViewModel {
  patientId: number;
  patientFound = false;
  errorFindingPatient = false;

  currentStep = 1;
  isComplete = false;
  isSynched = false;

  customAttributeKey = 'Case Number';
  customAttributeList: CustomAttributeDetailModel[] = [];
  
  attachments: FormAttachmentModel[] = [];

  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'start-date', 'end-date', 'dose', 'actions']);
  medications: PatientMedicationForUpdateModel[] = [];
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