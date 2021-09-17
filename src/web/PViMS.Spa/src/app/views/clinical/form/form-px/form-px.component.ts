import { AfterViewInit, Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { EventService } from 'app/shared/services/event.service';
import { PatientService } from 'app/shared/services/patient.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { forkJoin, Observable } from 'rxjs';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { _routes } from 'app/config/routes';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { Form } from 'app/shared/indexed-db/appdb';
import { FormCompletePopupComponent } from '../form-complete-popup/form-complete.popup.component';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { ConditionDetailModel } from 'app/shared/models/condition/condition.detail.model';
import { EncounterTypeIdentifierModel } from 'app/shared/models/encounter/encounter-type.identifier.model';
import { PriorityIdentifierModel } from 'app/shared/models/encounter/priority.identifier.model';
import { ConditionService } from 'app/shared/services/condition.service';
import { EncounterTypeService } from 'app/shared/services/encounter-type.service';
import { PriorityService } from 'app/shared/services/priority.service';
import { PatientForCreationModel } from 'app/shared/models/patient/patient-for-creation.model';
import { finalize } from 'rxjs/operators';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';

const moment =  _moment;

@Component({
  templateUrl: './form-px.component.html',
  styles: [`
    .mat-column-file-name { flex: 0 0 85% !important; width: 85% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],   
  animations: egretAnimations
})
export class FormPXComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {
  // @HostListener allows us to also guard against browser refresh, close, etc.
  @HostListener('window:beforeunload')
  
  public scrollConfig = {}

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;
  fourthFormGroup: FormGroup;

  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService,
    protected metaFormService: MetaFormService,
    protected conditionService: ConditionService,
    protected encounterTypeService: EncounterTypeService,
    protected priorityService: PriorityService,
    protected dialog: MatDialog) 
  { 
    super(_router, _location, popupService, accountService, eventService);
  }

  canDeactivate(): Observable<boolean> | boolean {
    // returning true will navigate without confirmation
    // returning false will show a confirm dialog before navigating away
    if(this.firstFormGroup.dirty ||
      this.thirdFormGroup.dirty ||
      this.fourthFormGroup.dirty) {
        return false;
    }
    return true;
  }  

  ngOnInit(): void {
    const self = this;
    self.viewModel.formId = +self._activatedRoute.snapshot.paramMap.get('id');
    
    self.arrayAttributes = [];
    self.viewModelForm = self._formBuilder.group({
      formCompleted: ['']
    });
    self.firstFormGroup = this._formBuilder.group({
      firstName: ['', [Validators.required, Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      lastName: ['', [Validators.required, Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      middleName: ['', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      dateOfBirth: ['', Validators.required],
      facilityName: ['', Validators.required],
    });
    self.secondFormGroup = this._formBuilder.group({
      attributes: this._formBuilder.group([]),
    });
    self.thirdFormGroup = this._formBuilder.group({
      conditionGroupId: ['', Validators.required],
      meddraTermId: ['', Validators.required],
      cohortGroupId: ['', Validators.required],
      enroledDate: ['', Validators.required],
      startDate: ['', Validators.required],
      outcomeDate: [''],
      caseNumber: ['', [Validators.required, Validators.maxLength(50), Validators.pattern("[-a-zA-Z0-9 .()]*")]],
      comments: ['', [Validators.maxLength(100), Validators.pattern("[-a-zA-Z0-9 .,()']*")]],
    });
    self.fourthFormGroup = this._formBuilder.group({
      encounterTypeId: [1, Validators.required],
      priorityId: [1, Validators.required],
      encounterDate: [moment(), Validators.required],
    });

    self.prepareFormData();
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.viewModel.formId > 0) {
       self.loadFormData();
    }
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormPXComponent.name);
  }   

  nextStep(): void {
    this.viewModel.currentStep ++;
  }

  previousStep(): void {
    this.viewModel.currentStep --;
  }

  loadFormData(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getForm(self.viewModel.formId).then(result => {
        let form = result as Form;
        self.CLog(form, 'form');
        
        self.viewModel.formIdentifier = form.formIdentifier;

        self.updateForm(self.viewModelForm, JSON.parse(form.formValues[0].formControlValue));

        self.viewModel.isComplete = form.completeStatus == 'Complete';
        self.viewModel.isSynched = form.synchStatus == 'Synched';

        if(self.viewModel.isSynched) {
          self.viewModelForm.disable();
        }
        self.setBusy(false);
    }, error => {
        self.handleError(error, error.statusText);
    });
  }  

  saveFormOnline(): void {
    const self = this;

    var patientForCreationModel = self.preparePatientForCreationModel();
    self.patientService.savePatient(patientForCreationModel)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Patient successfully saved!", "Success");

      self.firstFormGroup.markAsPristine();
      self.secondFormGroup.markAsPristine();
      self.thirdFormGroup.markAsPristine();
      self.fourthFormGroup.markAsPristine();

      self._router.navigate([_routes.clinical.forms.landing]);
    }, error => {
      this.handleError(error, "Error saving patient");
    });
  }
  
  saveFormOffline(): void {
    const self = this;
    let otherModels:any[]; 
    otherModels = [self.viewModelForm.value];

    // if (self.viewModel.formId == 0) {
    //   self.metaFormService.saveFormToDatabase('FormPx', self.viewModelForm.value, self.firstFormGroup.value, self.viewModel.attachments, otherModels).then(response =>
    //     {
    //       if (response) {
    //         self.setBusy(false);              
    //         self.notify('Form saved successfully!', 'Form Saved');

    //         self.viewModelForm.markAsPristine();
    
    //         self._router.navigate([_routes.clinical.forms.landing]);
    //       }
    //       else {
    //         self.showError('There was an error saving the form locally, please try again !', 'Form Error');
    //       }
    //     });
    //   }
    //   else {
    //     self.metaFormService.updateForm(self.viewModel.formId, this.viewModelForm.value, this.firstFormGroup.value, self.viewModel.attachments, otherModels).then(response =>
    //       {
    //           if (response) {
    //               self.notify('Form updated successfully!', 'Form Saved');

    //               self.viewModelForm.markAsPristine();
      
    //               self._router.navigate([_routes.clinical.forms.landing]);
    //           }
    //           else {
    //               self.showError('There was an error saving the form locally, please try again !', 'Form Error');
    //           }
    //       });
    //   }
  }

  completeFormOffline(): void {
    let self = this;
    let otherModels:any[];

    // otherModels = [self.thirdFormGroup.value, self.fourthFormGroup.value, self.viewModel.medications, self.sixthFormGroup.value];

    // if (self.viewModel.formId == 0) {
    //   self.metaFormService.saveFormToDatabase('FormADR', self.viewModelForm.value, self.firstFormGroup.value, self.viewModel.attachments, otherModels).then(response =>
    //     {
    //         if (response) {
    //           self.setBusy(false);              
    //           self.notify('Form saved successfully!', 'Form Saved');
  
    //           self.viewModelForm.markAsPristine();
      
    //           self.openCompletePopup(+response);
    //         }
    //         else {
    //             self.showError('There was an error saving the form locally, please try again !', 'Form Error');
    //         }
    //     });
    //   }
    //   else {
    //     self.metaFormService.updateForm(self.viewModel.formId, this.viewModelForm.value, this.firstFormGroup.value, self.viewModel.attachments, otherModels).then(response =>
    //       {
    //           if (response) {
    //               self.notify('Form updated successfully!', 'Form Saved');

    //               self.viewModelForm.markAsPristine();
    
    //               this.openCompletePopup(self.viewModel.formId);
    //             }
    //           else {
    //               self.showError('There was an error updating the form locally, please try again !', 'Form Error');
    //           }
    //       });
    //   }
  }

  onConditionSelected(event) {
    const value = event.value;
    this.viewModel.selectedCondition = this.viewModel.conditionList.filter(c => c.id == value)[0];
  }  

  private prepareFormData(): void {
    const self = this;
    self.setBusy(true);

    const requestArray = [];
    
    requestArray.push(self.conditionService.getAllConditions());
    requestArray.push(self.encounterTypeService.getAllEncounterTypes());
    requestArray.push(self.priorityService.getAllPriorities());
    requestArray.push(self.customAttributeService.getAllCustomAttributes('Patient'));

    let attributes = self.secondFormGroup.get('attributes') as FormGroup;

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.CLog(data[0], 'get all conditions')
          self.CLog(data[1], 'get all encounter types')
          self.CLog(data[2], 'get all priorities')
          self.CLog(data[3], 'get all patient custom attributes')

          self.viewModel.conditionList = data[0] as ConditionDetailModel[];
          self.viewModel.encounterTypeList = data[1] as EncounterTypeIdentifierModel[];
          self.viewModel.priorityList = data[2] as PriorityIdentifierModel[];
          self.viewModel.customAttributeList = data[3] as CustomAttributeDetailModel[];

          self.viewModel.facilityList = self.accountService.facilities;

          // Add custom attributes to form group
          self.viewModel.customAttributeList.forEach(attribute => {
            var defaultValue = '';
            if(attribute.customAttributeType == 'Selection') {
              defaultValue = '0';
            }

            let validators = [ ];
            if(attribute.required) {
              validators.push(Validators.required);
            }
            if(attribute.stringMaxLength != null) {
              validators.push(Validators.maxLength(attribute.stringMaxLength));
            }
            if(attribute.numericMinValue != null && attribute.numericMaxValue != null) {
              validators.push(Validators.max(attribute.numericMaxValue));
              validators.push(Validators.min(attribute.numericMinValue));
            }
            attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));
          })

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });

  }

  private preparePatientForCreationModel(): PatientForCreationModel {
    let self = this;
    let outcomeDate = '';
    if(moment.isMoment(self.thirdFormGroup.get('outcomeDate').value)) {
      outcomeDate = self.thirdFormGroup.get('outcomeDate').value.format('YYYY-MM-DD');
    }
    else {
      if (self.thirdFormGroup.get('outcomeDate').value != '') {
        outcomeDate = self.thirdFormGroup.get('outcomeDate').value;
      }
    }

    const attributesForUpdate: AttributeValueForPostModel[] = [];
    self.viewModel.customAttributeList.forEach(attribute => {
      attributesForUpdate.push(self.prepareAttributeValue(attribute.attributeKey, attribute.id.toString()));
    })

    const patientForCreationModel: PatientForCreationModel = 
    {
      firstName: self.firstFormGroup.get('firstName').value,
      lastName: self.firstFormGroup.get('lastName').value,
      middleName: self.firstFormGroup.get('middleName').value,
      dateOfBirth: self.firstFormGroup.get('dateOfBirth').value.format('YYYY-MM-DD'),
      facilityName: self.firstFormGroup.get('facilityName').value,
      conditionGroupId: +self.thirdFormGroup.get('conditionGroupId').value,
      meddraTermId: +self.thirdFormGroup.get('meddraTermId').value,
      cohortGroupId: +self.thirdFormGroup.get('cohortGroupId').value,
      enroledDate: self.thirdFormGroup.get('enroledDate').value.format('YYYY-MM-DD'),
      startDate: self.thirdFormGroup.get('startDate').value.format('YYYY-MM-DD'),
      outcomeDate: outcomeDate,
      caseNumber: self.thirdFormGroup.get('caseNumber').value,
      comments: self.thirdFormGroup.get('comments').value,
      encounterTypeId: +self.fourthFormGroup.get('encounterTypeId').value,
      priorityId: +self.fourthFormGroup.get('priorityId').value,
      encounterDate: self.fourthFormGroup.get('encounterDate').value.format('YYYY-MM-DD'),
      attributes: attributesForUpdate
    };

    return patientForCreationModel;
  }

  private prepareAttributeValue(attributeKey: string, formKey: string): AttributeValueForPostModel {
    const self = this;
    let customAttribute = self.viewModel.customAttributeList.find(ca => ca.attributeKey.toLowerCase() == attributeKey.toLowerCase());
    if(customAttribute == null) {
      return null;
    }

    let attributes = self.secondFormGroup.get('attributes') as FormGroup;
    const attributeForPost: AttributeValueForPostModel = {
      id: customAttribute.id,
      value: attributes.value[formKey]
    }
    self.CLog(attributeForPost, 'attributeForPost');
    return attributeForPost;
  }

  private openCompletePopup(formId: number) {
    let self = this;
    let title = "Form Completed";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormCompletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId, title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }        
        self._router.navigate([_routes.clinical.forms.landing]);        
      })
  }    
}

class ViewModel {
  formId: number;
  formIdentifier: string;
  
  facilityList: string[] = [];
  customAttributeList: CustomAttributeDetailModel[] = [];
  conditionList: ConditionDetailModel[] = [];
  selectedCondition: ConditionDetailModel;
  encounterTypeList: EncounterTypeIdentifierModel[] = [];
  priorityList: PriorityIdentifierModel[] = [];

  currentStep = 1;
  isComplete = false;
  isSynched = false;
  connected: boolean = true;
}