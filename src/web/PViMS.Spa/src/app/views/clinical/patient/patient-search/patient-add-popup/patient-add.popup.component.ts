import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { finalize } from 'rxjs/operators';
import { ConditionService } from 'app/shared/services/condition.service';
import { EncounterTypeService } from 'app/shared/services/encounter-type.service';
import { EncounterTypeIdentifierModel } from 'app/shared/models/encounter/encounter-type.identifier.model';
import { PriorityIdentifierModel } from 'app/shared/models/encounter/priority.identifier.model';
import { PriorityService } from 'app/shared/services/priority.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { ConditionDetailModel } from 'app/shared/models/condition/condition.detail.model';

@Component({
  templateUrl: './patient-add.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class PatientAddPopupComponent extends BasePopupComponent implements OnInit {
  
  public viewModelForm: FormGroup;
  protected busy: boolean = false;

  facilityList: string[] = [];
  customAttributeList: CustomAttributeDetailModel[] = [];
  conditionList: ConditionDetailModel[] = [];
  selectedCondition: ConditionDetailModel;
  encounterTypeList: EncounterTypeIdentifierModel[] = [];
  priorityList: PriorityIdentifierModel[] = [];

  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PatientAddPopupData,
    public dialogRef: MatDialogRef<PatientAddPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService,
    protected conditionService: ConditionService,
    protected encounterTypeService: EncounterTypeService,
    protected priorityService: PriorityService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      firstName: ['', [Validators.required, Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      lastName: ['', [Validators.required, Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      middleName: ['', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      dateOfBirth: ['', Validators.required],
      facilityName: ['', Validators.required],
      attributes: this._formBuilder.group([]),
      conditionGroupId: ['', Validators.required],
      meddraTermId: ['', Validators.required],
      cohortGroupId: [''],
      enroledDate: [''],
      startDate: ['', Validators.required],
      outcomeDate: [''],
      comments: ['', [Validators.maxLength(100), Validators.pattern("[-a-zA-Z0-9 ']*")]],
      encounterTypeId: ['', Validators.required],
      priorityId: ['', Validators.required],
      encounterDate: ['', Validators.required],
    })

    self.getCustomAttributeList();
  }

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
    self.getConditionList();
    self.getEncounterTypeList();
    self.getPriorityList();
  }  

  getFacilityList(): void {
    let self = this;
    self.facilityList = self.accountService.facilities;
  }

  getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;

    self.customAttributeService.getAllCustomAttributes('Patient')
        .subscribe(result => {
            self.customAttributeList = result;

            // Add custom attributes to form group
            self.customAttributeList.forEach(attribute => {
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

        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  getConditionList(): void {
    let self = this;

    self.conditionService.getAllConditions()
        .subscribe(result => {
          self.conditionList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  getEncounterTypeList(): void {
    let self = this;

    self.encounterTypeService.getAllEncounterTypes()
        .subscribe(result => {
          self.encounterTypeList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  getPriorityList(): void {
    let self = this;

    self.priorityService.getAllPriorities()
        .subscribe(result => {
          self.priorityList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  public onConditionSelected(event) {
    const value = event.value;
    var selections = this.conditionList.filter(c => c.id == value);
    this.selectedCondition = this.conditionList.filter(c => c.id == value)[0];
  }

  submit() {
    let self = this;
    self.setBusy(true);

    if(self.data.patientId == 0) {
      self.patientService.savePatient(self.viewModelForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify("Patient successfully saved!", "Success");
          this.dialogRef.close(this.viewModelForm.value);
      }, error => {
        this.handleError(error, "Error saving patient");
      });
    }
  }
}

export interface PatientAddPopupData {
  patientId: number;
  title: string;
  payload: any;
}