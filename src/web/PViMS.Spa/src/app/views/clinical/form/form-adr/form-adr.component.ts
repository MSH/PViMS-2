import { AfterViewInit, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { EventService } from 'app/shared/services/event.service';
import { PatientService } from 'app/shared/services/patient.service';
import { finalize, takeUntil } from 'rxjs/operators';
import { PatientExpandedModel } from 'app/shared/models/patient/patient.expanded.model';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';

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

  constructor(
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected patientService: PatientService) 
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
      ethnicGroup: ['', Validators.required],
    });
    self.thirdFormGroup = this._formBuilder.group({
      programStatus: ['', Validators.required],
      enrolmentDate: [''],
      orgCaregiverId: ['', [Validators.required, Validators.maxLength(21)]],
    });
    self.fourthFormGroup = this._formBuilder.group({
      caregiverFirstName: ['', [Validators.required, Validators.maxLength(21), Validators.pattern(/^[a-zA-Z][a-zA-Z\-' ]*[a-zA-Z ]$/)]],
      caregiverLastName: ['', [Validators.required, Validators.maxLength(21), Validators.pattern(/^[a-zA-Z][a-zA-Z\-' ]*[a-zA-Z ]$/)]],
      caregiverKnownAs: ['', Validators.maxLength(21)],
      dateOfBirth: [''],
      idType: ['', Validators.required],
      idNumber: ['', Validators.required],
      nationality: ['', Validators.required],
      nationalityOther: ['', Validators.maxLength(50)],
      sex: ['', Validators.required],
      race: ['', Validators.required],
      maritalStatus: ['', Validators.required],
      disability: ['', Validators.required],
      disabilityOther: ['', Validators.maxLength(50)],
      homeLanguage: ['', Validators.required],
      homeLanguageOther: ['', Validators.maxLength(50)],
      cellNumber: ['', Validators.pattern(/^0[87][23467]((\d{7})|((\d{3}))(\d{4})|(\d{7}))/)],
      altCellNumber: ['', Validators.pattern(/^0[87][23467]((\d{7})|((\d{3}))(\d{4})|(\d{7}))/)],
    });
    self.fifthFormGroup = this._formBuilder.group({
      headship: ['', Validators.required]
    });

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
          self.viewModel.errorFindingPatient = false;
          self.viewModel.patientFound = true;

          self.updateForm(self.firstFormGroup, result);
          self.updateForm(self.firstFormGroup, {gender: self.getValueFromAttribute(result.patientAttributes, "Gender")});
          self.updateForm(self.firstFormGroup, {contactNumber: self.getValueFromAttribute(result.patientAttributes, "Contact Number")});
          self.updateForm(self.firstFormGroup, {address: self.getValueFromAttribute(result.patientAttributes, "Address")});
          
        }
        // self.updateForm(self.viewNotesForm, (self.viewModel = result));
        // self.updateForm(self.viewAuditForm, (self.viewModel = result));
        
        // self.viewGridModel.customGrid.updateBasic(result.patientAttributes);
        // self.viewGridModel.appointmentGrid.updateBasic(result.appointments);
        // self.viewGridModel.attachmentGrid.updateBasic(result.attachments);
        // self.viewGridModel.statusGrid.updateBasic(result.patientStatusHistories);
        // self.viewGridModel.encounterGrid.updateBasic(result.encounters);
        // self.viewGridModel.cohortGrid.updateBasic(result.cohortGroups);
        // self.viewGridModel.conditionGroupGrid.updateBasic(result.conditionGroups);
        // self.viewGridModel.analyticalGrid.updateBasic(result.activity);

        // self.viewGridModel.conditionGrid.updateBasic(result.patientConditions);
        // self.viewGridModel.clinicalEventGrid.updateBasic(result.patientClinicalEvents);
        // self.viewGridModel.medicationGrid.updateBasic(result.patientMedications);
        // self.viewGridModel.labTestGrid.updateBasic(result.patientLabTests);

      }, error => {
        self.handleError(error, "Error fetching patient");
      });
  }  

  finish(): void{
    let self = this;

    // self.entityService.saveNewHousehold(this.firstFormGroup.value, this.secondFormGroup.value, this.thirdFormGroup.value, this.fourthFormGroup.value, this.fifthFormGroup.value).then(response =>
    //   {
    //     if (response) {
    //       self.notify('Household successfully registered!', 'Success');
    //       self.dialogRef.close("Saved");
    //     }
    //     else {
    //       self.showError('There was an error registering the household, please try again !', 'Error');
    //     }
    //   });
  }

  private getValueFromAttribute(attributes: AttributeValueModel[], key: string): string {
    let attribute = attributes.find(a => a.key == key);
    if(attribute?.selectionValue != '') {
      return attribute?.selectionValue;
    }
     return attribute?.value;
  }
}

class ViewModel {
  patientFound = false;
  errorFindingPatient = false;
  currentStep = 1;
  isComplete = false;
  isSynched = false;
  id: number;
  customAttributeKey = 'Case Number';
}