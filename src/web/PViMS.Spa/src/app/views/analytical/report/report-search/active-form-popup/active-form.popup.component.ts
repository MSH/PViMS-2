import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { switchMap } from 'rxjs/operators';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { GridModel } from 'app/shared/models/grid.model';
import { forkJoin, of } from 'rxjs';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientClinicalEventExpandedModel } from 'app/shared/models/patient/patient-clinical-event.expanded.model';
import { PatientMedicationForUpdateModel } from 'app/shared/models/patient/patient-medication-for-update.model';
import { PatientMedicationDetailModel } from 'app/shared/models/patient/patient-medication.detail.model';
import { PatientExpandedModel } from 'app/shared/models/patient/patient.expanded.model';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';
const moment =  _moment;

@Component({
  templateUrl: './active-form.popup.component.html',
  styles: [`
    .mat-column-executed-date { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-activity { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-execution-event { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-comments { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-source { flex: 0 0 18% !important; width: 18% !important; }
    .mat-column-description { flex: 0 0 30% !important; width: 30% !important; }
    .mat-column-task-type { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-task-status { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-file-name { flex: 0 0 85% !important; width: 85% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],  
  animations: egretAnimations
})
export class ActiveFormPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();

  public firstFormGroup: FormGroup;
  public secondFormGroup: FormGroup;
  public thirdFormGroup: FormGroup;
  public fourthFormGroup: FormGroup;
  public fifthFormGroup: FormGroup;
  public sixthFormGroup: FormGroup;  

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<ActiveFormPopupComponent>,
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

    self.firstFormGroup = this._formBuilder.group({
      patientId: [''],
      patientIdentifier: [''],
      patientFirstName: [''],
      patientLastName: [''],
      gender: [''],
      ethnicity: [''],
      dateOfBirth: [''],
      age: [''],
      ageGroup: [''],
      facilityName: [''],
      facilityRegion: [''],      
    });
    self.thirdFormGroup = this._formBuilder.group({
      onsetDate: ['', Validators.required],
      regimen: [null, Validators.required],
      sourceDescription: [null, [Validators.required, Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      isSerious: [null],
      seriousness: [null],
      classification: [null, Validators.required],
      weight: [null, [Validators.required, Validators.min(1), Validators.max(159)]],
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
    if (self.data.clinicalEventId > 0) {
      self.loadData();
    }
  } 
  
  setStep(index: number) {
    this.viewModel.step = index;
  }

  nextStep() {
    this.viewModel.step++;
  }

  prevStep() {
    this.viewModel.step--;
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    requestArray.push(self.patientService.getPatientClinicalEventExpanded(self.data.patientId, self.data.clinicalEventId));
    requestArray.push(self.patientService.getPatientExpanded(self.data.patientId));

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.CLog(data[0], 'get clinical event expanded')
          self.CLog(data[1], 'get patient detail')

          self.loadGrids(data[1] as PatientExpandedModel, data[0] as PatientClinicalEventExpandedModel);

          self.loadDataForFirstForm(data[1] as PatientExpandedModel);
          self.loadDataForThirdForm(data[0] as PatientClinicalEventExpandedModel);
          self.loadDataForFourthForm(data[0] as PatientClinicalEventExpandedModel);
          self.loadDataForSixthForm(data[0] as PatientClinicalEventExpandedModel);

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });    
  }

  private loadGrids(patientModel: PatientExpandedModel, clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;
    self.viewModel.medications = self.mapMedicationForUpdateModels(patientModel.patientMedications);
    self.viewModel.medicationGrid.updateBasic(self.viewModel.medications);
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

  private loadDataForFirstForm(patientModel: PatientExpandedModel)
  {
    let self = this;
    self.updateForm(self.firstFormGroup, patientModel);
    self.updateForm(self.firstFormGroup, {patientId: patientModel.id});
    self.updateForm(self.firstFormGroup, {patientFirstName: patientModel.firstName});
    self.updateForm(self.firstFormGroup, {patientLastName: patientModel.lastName});
    self.updateForm(self.firstFormGroup, {gender: self.getValueOrSelectedValueFromAttribute(patientModel.patientAttributes, "Gender")});
    self.updateForm(self.firstFormGroup, {ethnicity: self.getValueOrSelectedValueFromAttribute(patientModel.patientAttributes, "Ethnic Group")});
  }

  private loadDataForThirdForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.thirdFormGroup, { 'onsetDate': clinicalEventModel.onsetDate })
    self.updateForm(self.thirdFormGroup, { 'regimen': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'regimen')?.value })
    self.updateForm(self.thirdFormGroup, { 'sourceDescription': clinicalEventModel.sourceDescription })
    self.updateForm(self.thirdFormGroup, { 'isSerious': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'is the adverse event serious?')?.value })
    self.updateForm(self.thirdFormGroup, { 'seriousness': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'seriousness')?.value })
    self.updateForm(self.thirdFormGroup, { 'classification': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'classification')?.value })
    self.updateForm(self.thirdFormGroup, { 'weight': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'weight (kg)')?.value })
    self.updateForm(self.thirdFormGroup, { 'height': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'height (cm)')?.value })
    self.updateForm(self.thirdFormGroup, { 'allergy': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'any known allergy')?.value })
    self.updateForm(self.thirdFormGroup, { 'pregnancyStatus': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'pregnancy status')?.value })
    self.updateForm(self.thirdFormGroup, { 'comorbidities': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'comorbidities')?.value })
  }

  private loadDataForFourthForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.fourthFormGroup, { 'treatmentGiven': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'was treatment given?')?.value })
    self.updateForm(self.fourthFormGroup, { 'treatmentDetails': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'treatment details')?.value })
    self.updateForm(self.fourthFormGroup, { 'outcome': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'outcome')?.value })
    self.updateForm(self.fourthFormGroup, { 'dateOfRecovery': clinicalEventModel.resolutionDate })
    self.updateForm(self.fourthFormGroup, { 'sequlae': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'sequlae details')?.value })
    self.updateForm(self.fourthFormGroup, { 'interventions': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'interventions')?.value })
  }

  private loadDataForSixthForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.sixthFormGroup, { 'reporterName': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'name of reporter')?.value })
    self.updateForm(self.sixthFormGroup, { 'contactNumber': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'contact number')?.value })
    self.updateForm(self.sixthFormGroup, { 'emailAddress': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'email address')?.value })
    self.updateForm(self.sixthFormGroup, { 'profession': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'profession')?.value })
  }

  private getCustomAttributeList(): void {
    let self = this;

    const requestArray = [];

    requestArray.push(self.customAttributeService.getAllCustomAttributes('PatientClinicalEvent'));
    requestArray.push(self.customAttributeService.getAllCustomAttributes('Patient'));

    forkJoin(requestArray)
      .pipe(
        switchMap((values: any[]) => {
          const mergeAttributeList: CustomAttributeDetailModel[] = [];

          values[0].forEach((attribute) => {
            mergeAttributeList.push(attribute);
          });
          values[1].forEach((attribute) => {
            mergeAttributeList.push(attribute);
          });

          return of(mergeAttributeList);
        })
      )
      .subscribe(
        data => {
          self.viewModel.customAttributeList = data;
        },
        error => {
          this.handleError(error, "Error loading attributes");
        });    
  }

  private getValueOrSelectedValueFromAttribute(attributes: AttributeValueModel[], key: string): string {
    let attribute = attributes.find(a => a.key == key);
    if(attribute?.selectionValue != '') {
      return attribute?.selectionValue;
    }
     return attribute?.value;
  }  
}

class ViewModel {
  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'start-date', 'dose']);
  medications: PatientMedicationForUpdateModel[] = [];

  workFlowId = '892F3305-7819-4F18-8A87-11CBA3AEE219';
  step = 0;

  customAttributeKey = 'Case Number';
  customAttributeList: CustomAttributeDetailModel[] = [];
}

export interface PopupData {
  patientId: number;
  clinicalEventId: number;
  title: string;
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