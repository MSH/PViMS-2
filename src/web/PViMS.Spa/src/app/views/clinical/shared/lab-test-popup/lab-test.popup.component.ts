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
import { LabTestIdentifierModel } from 'app/shared/models/labs/lab-test.identifier.model';
import { LabResultIdentifierModel } from 'app/shared/models/labs/lab-result.identifier.model';
import { LabTestUnitIdentifierModel } from 'app/shared/models/labs/lab-test-unit.identifier.model';
import { LabTestService } from 'app/shared/services/lab-test.service';

@Component({
  templateUrl: './lab-test.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class LabTestPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  labTestAttributes: AttributeValueModel[];
  labTestList: LabTestIdentifierModel[] = [];
  labResultList: LabResultIdentifierModel[] = [];
  labTestUnitList: LabTestUnitIdentifierModel[] = [];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: LabTestPopupData,
    public dialogRef: MatDialogRef<LabTestPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected labTestService: LabTestService,
    protected customAttributeService: CustomAttributeService,
    protected dialog: MatDialog
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      labTest: ['', Validators.required],
      testDate: ['', Validators.required],
      testResultCoded: [''],
      testResultValue: ['', [Validators.maxLength(20), Validators.pattern("[-a-zA-Z0-9 .]*")]],
      testUnit: [''],
      referenceLower: ['', [Validators.maxLength(20), Validators.pattern("[-a-zA-Z0-9 .]*")]],
      referenceUpper: ['', [Validators.maxLength(20), Validators.pattern("[-a-zA-Z0-9 .]*")]],
      attributes: this._formBuilder.group([])
    })

    if(self.data.labTestId == 0) {
      self.getCustomAttributeList();
    }
  }

  loadDropDowns(): void {
    let self = this;
    self.getLabTestList();
    self.getLabResultList();
    self.getLabTestUnitList();
  }

  getLabTestList(): void {
    let self = this;

    self.labTestService.getAllLabTests()
        .subscribe(result => {
          console.log(result);
          self.labTestList = result;
        }, error => {
          this.handleError(error, "Error fetching lab tests");
        });
  }

  getLabResultList(): void {
    let self = this;

    self.labTestService.getAllLabResults()
        .subscribe(result => {
          console.log(result);
          self.labResultList = result;
        }, error => {
          this.handleError(error, "Error fetching lab results");
        });
  }

  getLabTestUnitList(): void {
    let self = this;

    self.labTestService.getAllLabTestUnits()
        .subscribe(result => {
          console.log(result);
          self.labTestUnitList = result;
        }, error => {
          this.handleError(error, "Error fetching lab test units");
        });
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.labTestId > 0) {
      self.loadData();
    }
  }  
  
  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientLabTestDetail(self.data.patientId, self.data.labTestId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);
        self.labTestAttributes = result.labTestAttributes;

        self.getCustomAttributeList();
        self.markFormGroupTouched(self.viewModelForm);        
      }, error => {
        this.handleError(error, "Error fetching patient lab test");
      });
  }   

  getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;
    self.customAttributeService.getAllCustomAttributes('PatientLabTest')
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
              if(self.data.labTestId > 0) {
                let labTestAttribute = self.labTestAttributes.find(pa => pa.key == attribute.attributeKey);
                attributes.addControl(attribute.id.toString(), new FormControl(labTestAttribute != null ? labTestAttribute.value : defaultValue, validators));
              }
              else {
                attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));                
              }
            })

        }, error => {
          this.handleError(error, "Error fetching patient lab test attributes");
        });
  }

  submit() {
    let self = this;
    self.setBusy(true);
    self.patientService.savePatientLabTest(self.data.patientId, self.data.labTestId, self.viewModelForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Lab test successfully updated!", "Success");
        this.dialogRef.close(this.viewModelForm.value);
    }, error => {
      this.handleError(error, "Error saving lab test");
    });      
  }
}

export interface LabTestPopupData {
  patientId: number;
  labTestId: number;
  title: string;
}