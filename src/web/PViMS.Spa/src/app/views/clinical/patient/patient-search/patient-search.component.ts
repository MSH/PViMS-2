import { Component, OnInit, ViewChild, OnDestroy, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatPaginator, MatDialog, MatDialogRef } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from 'app/shared/services/event.service';
import { AccountService } from 'app/shared/services/account.service';
import { PopupService } from 'app/shared/services/popup.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { BaseComponent } from 'app/shared/base/base.component';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { PatientService } from 'app/shared/services/patient.service';
import { _routes } from 'app/config/routes';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { CustomAttributeIdentifierModel } from 'app/shared/models/custom-attribute/custom-attribute.identifier.model';
import { PatientAddPopupComponent } from './patient-add-popup/patient-add.popup.component';

import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';

const moment =  _moment;

@Component({
  templateUrl: './patient-search.component.html',
  styleUrls: ['./patient-search.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class PatientSearchComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

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
    protected dialog: MatDialog,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          this.setupTable();
      }
    });    
  }

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  facilityList: string[] = [];
  customAttributeList: CustomAttributeIdentifierModel[] = [];

  @ViewChild('mainGridPaginator', { static: false }) mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.viewModelForm = self._formBuilder.group({
      facilityName: [this.viewModel.facilityName || ''],
      patientId: [this.viewModel.patientId],
      firstName: [this.viewModel.firstName, [Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      lastName: [this.viewModel.lastName, [Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]], 
      dateOfBirth: [this.viewModel.dateOfBirth],
      customAttributeId: [this.viewModel.customAttributeId],
      customAttributeValue: [this.viewModel.customAttributeValue, [Validators.maxLength(150), Validators.pattern("[-a-zA-Z0-9 ']*")]]
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    console.log('paginator was ' + self.mainGridPaginator);
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(PatientSearchComponent.name);
  }  

  setupTable() {
    if (this.currentScreenWidth === 'xs') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['last-name', 'actions']);
    }
    if (this.currentScreenWidth === 'sm') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['first-name', 'last-name', 'facility', 'actions']);
    }

  }; 

  loadGrid(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.searchPatient(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.throwError(error, error.statusText);
        });
  }  

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
    self.getCustomAttributeList();
  }

  getFacilityList(): void {
    let self = this;
    self.facilityList = self.accountService.facilities;
  }

  getCustomAttributeList(): void {
    let self = this;
    self.customAttributeService.getPatientCustomAttributeSearchableList()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.customAttributeList = result.value;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  detail(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.clinical.patients.view(model ? model.id : 0)]);
  }

  openPatientPopUp(data: any = {}) {
    let self = this;
    let title = 'Add Patient';
    let dialogRef: MatDialogRef<any> = self.dialog.open(PatientAddPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: 0, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }

        self.updateForm(self.viewModelForm, { firstName: res.firstName });
        self.updateForm(self.viewModelForm, { lastName: res.lastName });

        self.loadGrid();
      })
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['id', 'first-name', 'last-name', 'facility', 'medical-record-number',
              'date-of-birth', 'last-encounter', 'actions']);

  facilityName: string;
  patientId: number;
  firstName: string;
  lastName: string;
  dateOfBirth: Moment;
  customAttributeId: number;
  customAttributeValue: string;
}

class GridRecordModel {
  id: number;
  firstName: string;
  lastName: string;
  currentFacility: string;
  medicalRecordNumber: string;
  dateOfBirth: string;
  age: number;
  latestEncounterDate: string;
}