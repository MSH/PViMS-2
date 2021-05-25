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
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AttachmentAddPopupComponent } from '../attachment-add-popup/attachment-add.popup.component';

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
      allergy: ['', Validators.maxLength(500)],
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
      headship: ['', Validators.required]
    });
    self.sixthFormGroup = this._formBuilder.group({
      reporterName: ['', [Validators.maxLength(100), Validators.pattern("[-a-zA-Z ']*")]],
      contactNumber: ['', [Validators.maxLength(30), Validators.pattern("[-0-9+']*")]],
      emailAddress: ['', Validators.maxLength(100)],
      profession: [null]
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

  save(): void{
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
        //self.loadData();
      })
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