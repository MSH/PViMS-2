import { Component, OnInit,  OnDestroy, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, RequiredValidator, ValidatorFn } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription, Observable } from 'rxjs';
import { FacilityService } from 'app/shared/services/facility.service';
// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { GridModel } from 'app/shared/models/grid.model';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { _routes } from 'app/config/routes';
import { MatDialogRef, MatDialog } from '@angular/material';
import { SpontaneousLabsPopupComponent } from './spontaneous-labs-popup/spontaneous-labs.popup.component';
import { SpontaneousMedicationsPopupComponent } from './spontaneous-medications-popup/spontaneous-medications.popup.component';
import { DatasetService } from 'app/shared/services/dataset.service';

const moment =  _moment;

@Component({
  templateUrl: './spontaneous.component.html',
  styleUrls: ['./spontaneous.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class SpontaneousComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected datasetService: DatasetService,
    protected dialog: MatDialog,
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

  busy: boolean = false;

  isComplete = false;
  linear = false;

  identifier: string;
  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;
  viewPatientModelForm: FormGroup;
  viewMedicationModelForm: FormGroup;
  viewLabTestModelForm: FormGroup;
  viewReactionModelForm: FormGroup;
  viewReporterModelForm: FormGroup;

  medications: MedicationGridRecordModel[] = [];
  labTests: LabTestGridRecordModel[] = [];

  ngOnInit(): void {
    const self = this;

    self.viewPatientModelForm = self._formBuilder.group({
      initials: ['', [Validators.required]],
      identification: [''],
      identificationType: [''],
      birthDate: [''],
      age: [''],
      ageUnitOfMeasure: [''],
      weight: ['', [Validators.max(199), Validators.min(0)]],
      sex: [''],
      ethnic: [''],
      additional: ['']
    });

    self.viewMedicationModelForm = self._formBuilder.group({
    });

    self.viewLabTestModelForm = self._formBuilder.group({
    });

    self.viewReactionModelForm = self._formBuilder.group({
      reaction: ['', [Validators.required]],
      startDate: [''],
      estimatedStartDate: [''],
      reactionHappen: [''],
      treatmentGiven: [''],
      whatTreatment: [''],
      treatmentOutcome: [''],
      recoveryDate: [''],
      deceasedDate: [''],
      otherInfo: [''],
    });

    self.viewReporterModelForm = self._formBuilder.group({
      reporter: ['', Validators.required],
      telephoneNumber: [''],
      email: [''],
      profession: [''],
      reference: [''],
      place: [''],
      confidential: ['']
    });
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(SpontaneousComponent.name);
  }    

  openLabPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Test Result' : 'Update Test Result';
    let dialogRef: MatDialogRef<any> = self.dialog.open(SpontaneousLabsPopupComponent, {
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
    let dialogRef: MatDialogRef<any> = self.dialog.open(SpontaneousMedicationsPopupComponent, {
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
          product: res.product,
          suspected: res.suspected,
          strength: res.strength,
          strengthUnit: res.strengthUnit,
          dose: res.dose,
          doseUnit: res.doseUnit,
          route: res.route,
          startDate: res.medicationStartDate.format('YYYY-MM-DD'),
          endDate: medicationEndDate,
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

  saveForm(): void {
    let self = this;
    self.setBusy(true);

    let allModels:any[]; 

    allModels = [this.viewPatientModelForm.value, this.labTests, this.medications, this.viewReactionModelForm.value, this.viewReporterModelForm.value];

    self.datasetService.saveSpontaneousInstance(32, allModels)
        .subscribe(result => {
            self.notify("Report created successfully", "Spontaneous Report");
            self._router.navigate([_routes.security.landing]);
        }, error => {
            self.handleError(error, "Error saving spontaneous report");
        });
  }

  public isBusy(): boolean {
    return this.busy;
  }

  public setBusy(value: boolean): void {
    setTimeout(() => { this.busy = value; });
  }  

  removeMedication(index: number): void {
    let self = this;
    self.medications.splice(index, 1)
    this.viewModel.medicationGrid.updateBasic(this.medications);

    this.notify("Product removed successfully!", "Medication");
  }  

  removeLabTest(index: number): void {
    let self = this;
    self.labTests.splice(index, 1)
    this.viewModel.labTestGrid.updateBasic(this.labTests);

    this.notify("Lab test removed successfully!", "Lab Test");
  }
}

class ViewModel {
  labTestGrid: GridModel<LabTestGridRecordModel> =
  new GridModel<LabTestGridRecordModel>
      (['lab test', 'test date', 'test result', 'actions']);

  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'suspected', 'start-date', 'actions']);
}

class LabTestGridRecordModel {
  index: number;
  testResultDate: string;
  labTest: string;
  testResultValue: string;
}

class MedicationGridRecordModel {
  index: number;
  product: string;
  suspected: string;
  strength: string;
  strengthUnit: string;
  dose: string;
  doseUnit: string;
  route: string;
  startDate: string;
  endDate: string;
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