import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
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
import { ClinicalEventTaskPopupComponent } from '../clinical-event-task-popup/clinical-event-task.popup.component';

@Component({
  templateUrl: './feedback-search.component.html',
  styles: [`
    .mat-column-identifier { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-created { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-patient { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-adverse-event { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-meddra-term { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-task-count { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-status { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }
  `],   
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
  
    self.loadData();
    self.loadGrid();
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.workFlowService.getWorkFlowDetail(self.viewModel.workflowId)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.workFlow = result;
      }, error => {
        this.handleError(error, "Error loading work flow");
      });
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FeedbackSearchComponent.name);
  }

  selectActivity(qualifiedName: string): void {
    let self = this;

    self.updateForm(self.viewModelForm, {qualifiedName: qualifiedName});

    switch(qualifiedName) { 
      case 'Confirm Report Data': { 
        self.viewModel.mainGrid.updateDisplayedColumns(['identifier', 'created', 'patient', 'adverse-event', 'task-count', 'status', 'actions'])
        break; 
      } 
      case 'Set MedDRA and Causality': { 
        self.viewModel.mainGrid.updateDisplayedColumns(['identifier', 'created', 'patient', 'adverse-event', 'meddra-term', 'status', 'actions'])
         break; 
      } 
      default: { 
         //statements; 
         break; 
      } 
    } 


    self.viewModel.searchContext = "Activity";
    self.loadGrid();
  }

  hasActivity(): boolean {
    let self = this;

    let index = self.viewModel?.workFlow?.feedbackActivity.findIndex(fa => fa.reportInstanceCount > 0);
    return index > -1;
  }

  searchByTerm(): void {
    let self = this;

    self.viewModel.searchContext = "Term";
    self.loadGrid();
  }

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    switch (self.viewModel.searchContext) {
      case "Activity":
        self.reportInstanceService.getFeedbackReportInstancesByDetail(self.viewModel.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.CLog(result, 'feedback results by activity');
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error getting report instances by activity");
        });
  
        break;

      case "Term":
        self.reportInstanceService.searchFeedbackInstanceByTerm(self.viewModel.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.CLog(result, 'feedback results by term');
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching for feedback instances by term");
        });
    
        break;
    }    
  }

  openClinicalEventTaskPopUp(data: any = {}) {
    let self = this;
    let title = 'View Adverse Event';
    if(data.qualifiedName == 'Confirm Report Data') {
      let dialogRef: MatDialogRef<any> = self.dialog.open(ClinicalEventTaskPopupComponent, {
        width: '920px',
        disableClose: true,
        data: { patientId: data.patientId, clinicalEventId: data.patientClinicalEventId, reportInstanceId: data.id, title: title }
      })
      dialogRef.afterClosed()
        .subscribe(res => {
          return;
        })
    }
    else {
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
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['created', 'identifier', 'patient', 'adverse-event', 'meddra-term', 'actions']);

  workflowId = '892F3305-7819-4F18-8A87-11CBA3AEE219';
  workFlow: WorkFlowDetailModel;

  selectedTab = 0;
  searchContext: '' | 'Activity' | 'Term' = '';

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
  attachmentId: number;
  taskCount: number;
  qualifiedName: string;
  currentStatus: string;
}