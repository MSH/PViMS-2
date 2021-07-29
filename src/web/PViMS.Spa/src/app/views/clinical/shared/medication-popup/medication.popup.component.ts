import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material';
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
import { ConceptSelectPopupComponent } from 'app/shared/components/popup/concept-select-popup/concept-select.popup.component';

@Component({
  templateUrl: './medication.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class MedicationPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  medicationAttributes: AttributeValueModel[];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MedicationPopupData,
    public dialogRef: MatDialogRef<MedicationPopupComponent>,
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

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      sourceDescription: ['', [Validators.required, Validators.maxLength(200), Validators.pattern("[-a-zA-Z0-9 .,()']*")]],
      conceptId: ['', Validators.required],
      productId: [''],
      medication: [''],
      startDate: ['', Validators.required],
      endDate: [''],
      dose: ['', [Validators.maxLength(30), Validators.pattern("[a-zA-Z0-9.]*")]],
      doseUnit: [''],
      doseFrequency: ['', [Validators.maxLength(30), Validators.pattern("[a-zA-Z0-9.]*")]],
      attributes: this._formBuilder.group([])
    })

    if(self.data.medicationId == 0) {
      self.getCustomAttributeList();
    }
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.medicationId > 0) {
      self.loadData();
    }
  }  
  
  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientMedicationDetail(self.data.patientId, self.data.medicationId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);
        self.medicationAttributes = result.medicationAttributes;

        self.getCustomAttributeList();
        self.markFormGroupTouched(self.viewModelForm);        
      }, error => {
        self.throwError(error, error.statusText);
      });
  }   

  getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;
    self.customAttributeService.getAllCustomAttributes('PatientMedication')
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
              if(self.data.medicationId > 0) {
                let medicationAttribute = self.medicationAttributes.find(pa => pa.key == attribute.attributeKey);
                attributes.addControl(attribute.id.toString(), new FormControl(medicationAttribute != null ? medicationAttribute.value : defaultValue, validators));
              }
              else {
                attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));                
              }
            })

        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  openConceptPopup() {
    let self = this;
    let title = 'Select Medication';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ConceptSelectPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, productOnly: false }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        if(result.source == "Concept") {
          self.updateForm(self.viewModelForm, {conceptId: result.id});
          self.updateForm(self.viewModelForm, {productId: 0});
          self.updateForm(self.viewModelForm, {medication: result.displayName});
        }
        else {
          self.updateForm(self.viewModelForm, {conceptId: 0});
          self.updateForm(self.viewModelForm, {productId: result.id});
          self.updateForm(self.viewModelForm, {medication: result.displayName});
        }
      })
  }

  submit() {
    let self = this;
    self.setBusy(true);
    self.patientService.savePatientMedication(self.data.patientId, self.data.medicationId, self.viewModelForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Medication successfully updated!", "Success");
        this.dialogRef.close(this.viewModelForm.value);
    }, error => {
      this.handleError(error, "Error saving medication");
    });      
  }
}

export interface MedicationPopupData {
  patientId: number;
  medicationId: number;
  title: string;
}