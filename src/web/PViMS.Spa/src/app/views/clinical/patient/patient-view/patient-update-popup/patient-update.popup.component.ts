import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { finalize } from 'rxjs/operators';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';

@Component({
  templateUrl: './patient-update.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class PatientUpdatePopupComponent extends BasePopupComponent  implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  facilityList: string[] = [];
  customAttributeList: CustomAttributeDetailModel[] = [];
  patientAttributes: AttributeValueModel[];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PatientUpdatePopupData,
    public dialogRef: MatDialogRef<PatientUpdatePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService
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
      notes: [''],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.patientId > 0) {
      self.loadData();
    }
  }  
  
  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientDetail(self.data.patientId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);
        self.patientAttributes = result.patientAttributes;

        self.getCustomAttributeList();
      }, error => {
        self.throwError(error, error.statusText);
      });
  }   

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
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
              let patientAttribute = self.patientAttributes.find(pa => pa.key == attribute.attributeKey);
              attributes.addControl(attribute.id.toString(), new FormControl(patientAttribute != null ? patientAttribute.value : defaultValue, validators));
            })

        }, error => {
          this.handleError(error, "Error loading patient attributes");
        });
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.patientService.savePatient(self.data.patientId, self.viewModelForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Patient successfully updated!", "Success");
        this.dialogRef.close(this.viewModelForm.value);
    }, error => {
      this.handleError(error, "Error saving patient");
    });      
  }
}

export interface PatientUpdatePopupData {
  patientId: number;
  title: string;
}