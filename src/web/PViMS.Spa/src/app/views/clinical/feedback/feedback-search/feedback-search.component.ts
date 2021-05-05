import { Component, OnInit, OnDestroy, ViewChild, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MatPaginator, MatDialogRef, MatDialog, MatTabGroup } from '@angular/material';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { Moment } from 'moment';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { TerminologyMedDraModel } from 'app/shared/models/terminologymeddra.model';
import { _routes } from 'app/config/routes';

import { PatientService } from 'app/shared/services/patient.service';
import { WorkFlowService } from 'app/shared/services/work-flow.service';
import { ClinicalEventViewPopupComponent } from '../../shared/clinical-event-view-popup/clinical-event-view.popup.component';
import { WorkFlowDetailModel } from 'app/shared/models/work-flow/work-flow.detail.model';

@Component({
  templateUrl: './feedback-search.component.html',
  styleUrls: ['./feedback-search.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FeedbackSearchComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected reportInstanceService: ReportInstanceService,
    protected eventService: EventService,
    protected patientService: PatientService,
    protected workFlowService: WorkFlowService,    
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          //this.setupTable();
      }
    });
  }

  navigationSubscription;

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  workflowId = '892F3305-7819-4F18-8A87-11CBA3AEE219';
  workFlow: WorkFlowDetailModel;
    
  searchContext: '' | 'New' | 'Term' = '';
  selectedTab = 0;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;
  
  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
      qualifiedName: [self.viewModel.qualifiedName || ''],
      searchTerm: [self.viewModel.searchTerm || ''],
    });    
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
      null, null, self.mainGridPaginator)
      .subscribe(() => { self.loadGrid(); });

  
    self.searchContext = "New";
    self.loadData();
    self.loadGrid();
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.workFlowService.getWorkFlowDetail(self.workflowId)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.workFlow = result;
      }, error => {
        this.handleError(error, "Error loading work flow");
      });
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FeedbackSearchComponent.name);
  } 

  searchByTerm(): void {
    let self = this;

    self.searchContext = "Term";
    self.loadGrid();
  }

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    switch (self.searchContext) {
      case "New":
        self.reportInstanceService.getFeedbackReportInstancesByDetail(self.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching for new feedback");
        });
  
        break;

      case "Term":
        self.reportInstanceService.searchFeedbackInstanceByTerm(self.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching for feedback instances by term");
        });
    
        break;
    }    
  }

  openClinicalEventViewPopUp(data: any = {}) {
    let self = this;
    let title = 'View Adverse Event';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ClinicalEventViewPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: data.patientId, clinicalEventId: data.patientClinicalEventId, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        return;
      })
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['created', 'identifier', 'patient', 'adverse-event', 'meddra-term', 'actions']);

  qualifiedName: string;
  searchFrom: Moment;
  searchTo: Moment;
  searchTerm: string;
}

class GridRecordModel {
  id: number;
  createdDetail: string;
  identifier: string;
  patientIdentifier: string;
  sourceIdentifier: string;
  terminologyMedDra?: TerminologyMedDraModel;
  patientId: number;
  patientClinicalEventId: number;
}