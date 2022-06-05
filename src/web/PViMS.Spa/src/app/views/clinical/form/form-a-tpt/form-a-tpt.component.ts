import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ValidatorFn } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { from, Subscription } from 'rxjs';
import { FacilityService } from 'app/shared/services/facility.service';
import { concatMap, finalize, takeUntil } from 'rxjs/operators';
import { Moment } from 'moment';
// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { GridModel } from 'app/shared/models/grid.model';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { _routes } from 'app/config/routes';
import { Form } from 'app/shared/indexed-db/appdb';
import { FacilityIdentifierModel } from 'app/shared/models/facility/facility.identifier.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormATPTLabsPopupComponent } from './form-a-tpt-labs-popup/form-a-tpt-labs.popup.component';
import { FormATPTMedicationsPopupComponent } from './form-a-tpt-medications-popup/form-a-tpt-medications.popup.component';
import { FormCompletePopupComponent } from '../form-complete-popup/form-complete.popup.component';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';
import { FormGuidelinesPopupComponent } from '../form-guidelines-popup/form-guidelines.popup.component';
import { FormATPTConditionsPopupComponent } from './form-a-tpt-conditions-popup/form-a-tpt-conditions.popup.component';
import { PatientMedicationForUpdateModel } from 'app/shared/models/patient/patient-medication-for-update.model';
import { PatientService } from 'app/shared/services/patient.service';

const moment =  _moment;

@Component({
  templateUrl: './form-a-tpt.component.html',
  animations: egretAnimations
})
export class FormATPTComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected facilityService: FacilityService,
    protected patientService: PatientService,
    protected dialog: MatDialog,
    protected metaFormService: MetaFormService,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          //this.setupTable();
      }
    });    
  }

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  isComplete = false;
  isSynched = false;

  id: number;
  identifier: string;
  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;
  viewPatientModelForm: FormGroup;
  viewConditionModelForm: FormGroup;
  viewLabTestModelForm: FormGroup;
  viewMedicationModelForm: FormGroup;
  viewOtherModelForm: FormGroup;

  facilityList: FacilityIdentifierModel[] = [];

  conditions: ConditionGridRecordModel[] = [];
  medications: MedicationGridRecordModel[] = [];
  labTests: LabTestGridRecordModel[] = [];

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.viewModel.formId = +self._activatedRoute.snapshot.paramMap.get('id');
    
    self.viewModelForm = self._formBuilder.group({
      formCompleted: ['']
    });

    self.viewPatientModelForm = self._formBuilder.group({
      treatmentSiteId: [this.viewModel.treatmentSiteId, [conditionalValidator(() => this.viewModelForm.get('formCompleted').value, Validators.required, '')]],
      asmNumber: [this.viewModel.asmNumber, [Validators.required]],
      patientFirstName: [this.viewModel.patientFirstName, Validators.required],
      patientLastName: [this.viewModel.patientLastName, Validators.required],
      patientIdentityNumber: [this.viewModel.patientIdentityNumber, [Validators.required, Validators.maxLength(20)]],
      birthDate: [this.viewModel.birthDate],
      age: [this.viewModel.age],
      gender: [this.viewModel.gender],
      weight: [this.viewModel.weight, [Validators.max(199), Validators.min(0)]],
      address: [this.viewModel.address, Validators.maxLength(100)],
      contactNumber: [this.viewModel.contactNumber, Validators.maxLength(15)],
      alcoholConsumption: [this.viewModel.alcoholConsumption],
      smoker: [this.viewModel.smoker],
      otherSubstance: [this.viewModel.otherSubstance],
    });

    this.viewModelForm.get('formCompleted').valueChanges
        .subscribe(value => {
            this.viewPatientModelForm.get('treatmentSiteId').updateValueAndValidity();
        });    

    self.viewConditionModelForm = self._formBuilder.group({
    });

    self.viewLabTestModelForm = self._formBuilder.group({
    });

    self.viewMedicationModelForm = self._formBuilder.group({
    });

    self.viewOtherModelForm = self._formBuilder.group({
      adherenceReason: [this.viewModel.adherenceReason],
      followUpDate: [this.viewModel.followUpDate|| '', Validators.required],
      nameReporter: [this.viewModel.nameReporter],
      currentDate: [this.viewModel.currentDate || moment(), Validators.required],
      telephoneReporter: [this.viewModel.telephoneReporter],
      professionReporter: [this.viewModel.professionReporter]
    });

    self.accountService.connected$.subscribe(val => {
      self.viewModel.connected = val;
    });

  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.id > 0) {
      self.loadData();
    }
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormATPTComponent.name);
  }

  nextStep(): void {
    this.viewModel.currentStep ++;
  }

  previousStep(): void {
    this.viewModel.currentStep --;
  }

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getForm(self.id).then(result => {
        let form = result as Form;
        
        self.identifier = form.formIdentifier;

        self.updateForm(self.viewModelForm, JSON.parse(form.formValues[0].formControlValue));
        self.updateForm(self.viewPatientModelForm, JSON.parse(form.formValues[1].formControlValue));
        self.updateForm(self.viewOtherModelForm, JSON.parse(form.formValues[5].formControlValue));

        self.viewModel.conditionGrid.updateBasic(JSON.parse(form.formValues[2].formControlValue));
        self.viewModel.labTestGrid.updateBasic(JSON.parse(form.formValues[3].formControlValue));
        self.viewModel.medicationGrid.updateBasic(JSON.parse(form.formValues[4].formControlValue));
      
        self.conditions = JSON.parse(form.formValues[2].formControlValue);
        self.labTests = JSON.parse(form.formValues[3].formControlValue);
        self.medications = JSON.parse(form.formValues[4].formControlValue);

        self.isComplete = form.completeStatus == 'Complete';
        self.isSynched = form.synchStatus == 'Synched';

        if(self.isComplete || self.isSynched) {
          self.viewPatientModelForm.disable();
          self.viewConditionModelForm.disable();
          self.viewLabTestModelForm.disable();
          self.viewMedicationModelForm.disable();
          self.viewOtherModelForm.disable();
        }

        self.setBusy(false);
    }, error => {
          self.throwError(error, error.statusText);
    });
  }

  openConditionPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Condition' : 'Update Condition';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormATPTConditionsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        let condition: ConditionGridRecordModel = {
          index: isNew ? this.conditions.length : data.index,
          condition: res.condition,
          conditionStatus: res.conditionStatus
        };
        if(isNew) {
          this.conditions.push(condition);
        }
        else {
          this.conditions[data.index] = condition;
        }
        this.viewModel.conditionGrid.updateBasic(this.conditions);
        this.viewConditionModelForm.reset();
      })
  }  

  openLabPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Test Result' : 'Update Test Result';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormATPTLabsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        let labTest: LabTestGridRecordModel = {
          index: isNew ? this.labTests.length : data.index,
          labTest: res.labTest,
          testResultDate: res.labTestDate.format('YYYY-MM-DD'),
          testResultValue: res.labTestResult
        };
        if(isNew) {
          this.labTests.push(labTest);
        }
        else {
          this.labTests[data.index] = labTest;
        }
        this.viewModel.labTestGrid.updateBasic(this.labTests);
        this.viewLabTestModelForm.reset();
      })
  }  

  openMedicationPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Medication' : 'Update Medication';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormATPTMedicationsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        let medicationEndDate = '';
        if(moment.isMoment(res.medicationEndDate)) {
          medicationEndDate = res.medicationEndDate.format('YYYY-MM-DD');
        }
    
        let medication: MedicationGridRecordModel = {
          index: isNew ? this.medications.length : data.index,
          medication: res.medication,
          dose: res.dose,
          frequency: res.frequency,
          startDate: res.medicationStartDate.format('YYYY-MM-DD'),
          endDate: medicationEndDate,
          continued: res.continued
        };
        if(isNew) {
          this.medications.push(medication);
        }
        else {
          this.medications[data.index] = medication;
        }
        this.viewModel.medicationGrid.updateBasic(this.medications);
        this.viewMedicationModelForm.reset();
      })
  }

  openCompletePopup(formId: number) {
    let self = this;
    let title = "Form Completed";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormCompletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        self._router.navigate([_routes.clinical.forms.cohortselect]);        
      })
  } 
  
  openGuidelinesPopup() {
    let self = this;
    let title = "GUIDELINES FOR COMPLETING THE TREATMENT INITIATION FORM (FORM A)";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormGuidelinesPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { title: title, type: 'A' }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
      })
  }  

  saveFormOnline(): void {
    const self = this;
    self.viewModel.saving = true;
    from(self.viewModel.medications).pipe(
      concatMap(medicationForUpdate => self.patientService.savePatientMedication(self.viewModel.patientId, medicationForUpdate.id, medicationForUpdate))
    ).pipe(
      finalize(() => self.saveOnlineMedicationsComplete()),
    ).subscribe(
      data => {
        self.CLog('subscription to save meds');
      },
      error => {
        this.handleError(error, "Error saving medications");
      });    
  }
  
  saveFormOffline(): void {
    const self = this;
    let otherModels:any[]; 
    otherModels = [self.viewModel.medications, self.viewOtherModelForm.value];

    if (self.viewModel.formId == 0) {
      self.metaFormService.saveFormToDatabase('FormATPT', self.viewModelForm.value, self.viewPatientModelForm.value, null, otherModels).then(response =>
        {
          if (response) {
            self.setBusy(false);              
            self.notify('Form saved successfully!', 'Form Saved');

            self.viewPatientModelForm.markAsPristine();
            self.viewConditionModelForm.markAsPristine();
            self.viewLabTestModelForm.markAsPristine();
            self.viewMedicationModelForm.markAsPristine();
            self.viewOtherModelForm.markAsPristine();
    
            self._router.navigate([_routes.clinical.forms.cohortselect]);
          }
          else {
            self.showError('There was an error saving the form locally, please try again !', 'Form Error');
          }
        });
      }
      else {
        self.metaFormService.updateForm(self.viewModel.formId, this.viewModelForm.value, this.viewPatientModelForm.value, null, otherModels).then(response =>
          {
              if (response) {
                  self.notify('Form updated successfully!', 'Form Saved');

                  self.viewPatientModelForm.markAsPristine();
                  self.viewConditionModelForm.markAsPristine();
                  self.viewLabTestModelForm.markAsPristine();
                  self.viewMedicationModelForm.markAsPristine();
                  self.viewOtherModelForm.markAsPristine();
            
                  self._router.navigate([_routes.clinical.forms.cohortselect]);
              }
              else {
                  self.showError('There was an error saving the form locally, please try again !', 'Form Error');
              }
          });
      }
  }

  completeFormOffline(): void {
    let self = this;
    let otherModels:any[];

    otherModels = [self.viewModel.medications, self.viewOtherModelForm.value];

    if (self.viewModel.formId == 0) {
      self.metaFormService.saveFormToDatabase('FormATPT', self.viewModelForm.value, self.viewPatientModelForm.value, null, otherModels).then(response =>
        {
            if (response) {
              self.setBusy(false);              
              self.notify('Form saved successfully!', 'Form Saved');
  
              self.viewPatientModelForm.markAsPristine();
              self.viewConditionModelForm.markAsPristine();
              self.viewLabTestModelForm.markAsPristine();
              self.viewMedicationModelForm.markAsPristine();
              self.viewOtherModelForm.markAsPristine();
  
              self.openCompletePopup(+response);
            }
            else {
                self.showError('There was an error saving the form locally, please try again !', 'Form Error');
            }
        });
      }
      else {
        self.metaFormService.updateForm(self.viewModel.formId, this.viewModelForm.value, this.viewPatientModelForm.value, null, otherModels).then(response =>
          {
              if (response) {
                  self.notify('Form updated successfully!', 'Form Saved');

                  self.viewPatientModelForm.markAsPristine();
                  self.viewConditionModelForm.markAsPristine();
                  self.viewLabTestModelForm.markAsPristine();
                  self.viewMedicationModelForm.markAsPristine();
                  self.viewOtherModelForm.markAsPristine();
    
                  this.openCompletePopup(self.viewModel.formId);
                }
              else {
                  self.showError('There was an error updating the form locally, please try again !', 'Form Error');
              }
          });
      }
  }   

  getFacilityList(): void {
    let self = this;
    self.facilityService.getAllFacilities()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.facilityList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }  

  removeMedication(index: number): void {
    let self = this;
    self.medications.splice(index, 1)
    this.viewModel.medicationGrid.updateBasic(this.medications);

    this.notify("Medication removed successfully!", "Medication");
  }  

  removeCondition(index: number): void {
    let self = this;
    self.conditions.splice(index, 1)
    this.viewModel.conditionGrid.updateBasic(this.conditions);

    this.notify("Condition removed successfully!", "Condition");
  }  

  removeLabTest(index: number): void {
    let self = this;
    self.labTests.splice(index, 1)
    this.viewModel.labTestGrid.updateBasic(this.labTests);

    this.notify("Lab test removed successfully!", "Lab Test");
  }

  private saveOnlineMedicationsComplete(): void {
    const self = this;
    const requestArray = [];

    // var clinicalEventForUpdate = self.prepareClinicalEventForUpdateModel();
    // requestArray.push(this.patientService.savePatientClinicalEvent(self.viewModel.patientId, 0, clinicalEventForUpdate));

    // self.viewModel.attachments.forEach(attachmentForUpdate => {
    //   requestArray.push(this.patientService.saveAttachment(self.viewModel.patientId, attachmentForUpdate.file, attachmentForUpdate.description));
    // });

    // forkJoin(requestArray)
    // .subscribe(
    //   data => {
    //     self.setBusy(false);
    //     self.notify('Form added successfully!', 'Success');

    //     self.firstFormGroup.markAsPristine();
    //     self.thirdFormGroup.markAsPristine();
    //     self.fourthFormGroup.markAsPristine();
    //     self.fifthFormGroup.markAsPristine();
    //     self.sixthFormGroup.markAsPristine();

    //     self._router.navigate([_routes.clinical.forms.cohortselect]);
    //   },
    //   error => {
    //     this.handleError(error, "Error adding form");
    //   });
  }
}

class ViewModel {
  formId: number;
  formIdentifier: string;

  currentStep = 1;
  isComplete = false;
  isSynched = false;
  connected: boolean = true;
  saving: boolean = false;

  treatmentSiteId: string;
  asmNumber: string;
  patientFirstName: string;
  patientLastName: string;
  patientIdentityNumber: string;
  birthDate: Moment;
  age: number;
  gender: string;
  weight: number;
  pregnant: string;
  lmpDate: Moment;
  gestAge: number;
  address: string;
  contactNumber: string;
  alcoholConsumption: string;
  smoker: string;
  otherSubstance: string;

  conditionGrid: GridModel<ConditionGridRecordModel> =
  new GridModel<ConditionGridRecordModel>
      (['condition', 'condition status', 'actions']);

  labTestGrid: GridModel<LabTestGridRecordModel> =
  new GridModel<LabTestGridRecordModel>
      (['lab test', 'test date', 'test result', 'actions']);

  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'start-date', 'end-date', 'dose', 'actions']);
  medications: PatientMedicationForUpdateModel[] = [];
    
  adherenceReason: string;
  followUpDate: Moment;
  nameReporter: string;
  currentDate: Moment;
  telephoneReporter: string;
  professionReporter: string;
}

class ConditionGridRecordModel {
  index: number;
  condition: string;
  conditionStatus: string;
}

class LabTestGridRecordModel {
  index: number;
  labTest: string;
  testResultDate: string;
  testResultValue: string;
}

class MedicationGridRecordModel {
  index: number;
  medication: string;
  dose: string;
  frequency: string;
  startDate: string;
  endDate: string;
  continued: string;
}

export interface BooleanFn {
  (): boolean;
}

/**
 * A conditional validator generator. Assigns a validator to the form control if the predicate function returns true on the moment of validation
 * @example
 * Here if the myCheckbox is set to true, the myEmailField will be required and also the text will have to have the word 'mason' in the end.
 * If it doesn't satisfy these requirements, the errors will placed to the dedicated `illuminatiError` namespace.
 * Also the myEmailField will always have `maxLength`, `minLength` and `pattern` validators.
 * ngOnInit() {
 *   this.myForm = this.fb.group({
 *    myCheckbox: [''],
 *    myEmailField: ['', [
 *       Validators.maxLength(250),
 *       Validators.minLength(5),
 *       Validators.pattern(/.+@.+\..+/),
 *       conditionalValidator(() => this.myForm.get('myCheckbox').value,
 *                            Validators.compose([
 *                            Validators.required,
 *                            Validators.pattern(/.*mason/)
 *         ]),
 *        'illuminatiError')
 *        ]]
 *     })
 * }
 * @param predicate
 * @param validator
 * @param errorNamespace optional argument that creates own namespace for the validation error
 */
export function conditionalValidator(predicate: BooleanFn,
                validator: ValidatorFn,
                errorNamespace?: string): ValidatorFn {
  return (formControl => {
    if (!formControl.parent) {
      return null;
    }
    let error = null;
    if (predicate()) {
      error = validator(formControl);
    }
    if (errorNamespace && error) {
      const customError = {};
      customError[errorNamespace] = error;
      error = customError
    }
    return error;
  })
}