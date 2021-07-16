import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
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
import { GridModel } from 'app/shared/models/grid.model';

@Component({
  templateUrl: './clinical-event-view.popup.component.html',
  styles: [`
    .mat-column-identifier { flex: 0 0 55% !important; width: 55% !important; }
    .mat-column-naranjo { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-who { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-adverse-event { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-meddra-term { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-task-count { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-status { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }
  `],  
  animations: egretAnimations
})
export class ClinicalEventViewPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  clinicalEventAttributes: AttributeValueModel[];

  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ClinicalEventPopupData,
    public dialogRef: MatDialogRef<ClinicalEventViewPopupComponent>,
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
      sourceDescription: ['', [Validators.required, Validators.maxLength(250), Validators.pattern("[-a-zA-Z ']*")]],
      sourceTerminologyMedDraId: ['', Validators.required],
      medDraTerm: ['', Validators.required],
      onsetDate: ['', Validators.required],
      resolutionDate: [''],
      attributes: this._formBuilder.group([])
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.clinicalEventId > 0) {
      self.loadData();
      self.viewModel.mainGrid.setupBasic(null, null, null);
      self.viewModel.medicineGrid.setupBasic(null, null, null);
    }
  }  
  
  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientClinicalEventExpanded(self.data.patientId, self.data.clinicalEventId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.CLog(result, 'patient clinical event expanded');
        self.updateForm(self.viewModelForm, result);

        self.viewModel.mainGrid.updateBasic(result.activity);
        self.viewModel.medicineGrid.updateBasic(result.medications);

        self.clinicalEventAttributes = result.clinicalEventAttributes;
        self.viewModel.setMedDraTerm = result.setMedDraTerm;
        self.viewModel.setClassification = result.setClassification;

        self.getCustomAttributeList();
      }, error => {
        this.handleError(error, "Error fetching clinical event");
      });
  }   

  getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;
    self.customAttributeService.getAllCustomAttributes('PatientClinicalEvent')
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
              if(self.data.clinicalEventId > 0) {
                let clinicalEventAttribute = self.clinicalEventAttributes.find(pa => pa.key == attribute.attributeKey);
                let value = '';
                if (clinicalEventAttribute != null) {
                  value = clinicalEventAttribute.value;
                  if (clinicalEventAttribute.selectionValue != '') {
                    value = clinicalEventAttribute.selectionValue;
                  }
                }
                attributes.addControl(attribute.id.toString(), new FormControl(value, validators));
              }
              else {
                attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));                
              }
            })

        }, error => {
          this.handleError(error, "Error fetching clinical event attributes");
        });
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['executed-date', 'activity', 'execution-event', 'comments']);
  
  medicineGrid: GridModel<MedicineGridRecordModel> =
      new GridModel<MedicineGridRecordModel>
          (['identifier', 'naranjo', 'who']);

  setMedDraTerm: string;
  setClassification: string;
}

class GridRecordModel {
  activity: string;
  executionEvent: string;
  executedDate: string;
  comments: string;
}

class MedicineGridRecordModel {
  medicationIdentifier: string;
  naranjoCausality: string;
  whoCausality: string;
}

export interface ClinicalEventPopupData {
  patientId: number;
  clinicalEventId: number;
  title: string;
}