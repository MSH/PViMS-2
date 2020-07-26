import { Component, OnInit, ViewChild, OnDestroy, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatSort, MatPaginator, MatDialog, MatDialogRef } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from 'app/shared/services/event.service';
import { AccountService } from 'app/shared/services/account.service';
import { PopupService } from 'app/shared/services/popup.service';
import { BaseComponent } from 'app/shared/base/base.component';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { _routes } from 'app/config/routes';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { takeUntil } from 'rxjs/operators';
import { MetaFormDetailModel } from 'app/shared/models/meta/meta-form.detail.model';
import { AttachmentCaptureComponent } from '../attachment-capture/attachment-capture.component';
import { AttachmentViewComponent } from '../attachment-view/attachment-view.component';
import { FormDeletePopupComponent } from '../form-delete-popup/form-delete.popup.component';

@Component({
  selector: 'app-formslist',
  templateUrl: './formlist.component.html',
  styleUrls: ['./formlist.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected metaFormService: MetaFormService,
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

  synchRequired = false;

  formList: MetaFormDetailModel[] = [];

  @ViewChild('mainGridSort', { static: false }) mainGridSort: MatSort;
  @ViewChild('mainGridPaginator', { static: false }) mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    self.getMetaFormList();
    self.checkSynchRequired();

    self.viewModelForm = self._formBuilder.group({
      searchTerm: [this.viewModel.searchTerm],
      synchForms: [this.viewModel.synchForms],
      compForms: [this.viewModel.compForms],
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, self.mainGridSort, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormListComponent.name);
  } 

  checkSynchRequired(): void{
    const self = this;
    self.metaFormService.checkSynchRequired().then(response =>
      {
        self.synchRequired = response;          
      });
  }
  
  setupTable() {
    if (this.currentScreenWidth === 'xs') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['identifier', 'actions']);
    }
    if (this.currentScreenWidth === 'sm') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['identifier', 'patient identifier', 'synch status', 'actions']);
    }

  }; 

  getMetaFormList(): void {
    let self = this;
    self.metaFormService.getAllMetaForms()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.formList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.searchForms(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value)).then(result => {
      self.viewModel.mainGrid.updateAdvance(result);
      self.setBusy(false);
    }, error => {
      self.throwError(error, error.statusText);
    });
  }

  detail(model: GridRecordModel = null): void {
    let self = this;
    if (model.formType == 'FormA') {
      self._router.navigate([_routes.clinical.forms.viewFormA(model.id)]);
    } else {
      if (model.formType == 'FormB') {
        self._router.navigate([_routes.clinical.forms.viewFormB(model.id)]);
      }
      else {
        self._router.navigate([_routes.clinical.forms.viewFormC(model.id)]);
      }
    }
  }

  addForm(selectedOption: string): void {
    let self = this;
    if (selectedOption == 'Form A') {
      self._router.navigate([_routes.clinical.forms.viewFormA(0)]);
    } else {
      if (selectedOption == 'Form B') {
        self._router.navigate([_routes.clinical.forms.viewFormB(0)]);
      }
      else {
        self._router.navigate([_routes.clinical.forms.viewFormC(0)]);
      }
    }
  }

  openCameraPopup(id: number, index: number) {
    let self = this;
    let title = "Capture Image";
    let dialogRef: MatDialogRef<any> = self.dialog.open(AttachmentCaptureComponent, {
      width: '720px',
      disableClose: true,
      data: { formId: id, title, index }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }   

  openImageView(id: number, index: number) {
    let self = this;
    let title = "View Image";
    let dialogRef: MatDialogRef<any> = self.dialog.open(AttachmentViewComponent, {
      width: '720px',
      disableClose: true,
      data: { formId: id, title, index }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }

  openFormDelete(id: number) {
    let self = this;
    let title = "Delete Form";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { id: id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }   
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['created', 'form-type', 'identifier', 'patient identifier', 'patient name', 'complete status', 'synch status' , 'actions']);

  searchTerm: string;
  synchForms: any;
  compForms: any;
}

class GridRecordModel {
  id: number;
  created: string;
  formIdentifier: string;
  patientIdentifier: string;
  patientName: string;
  completeStatus: string;
  synchStatus: string;
  formType: string;
  hasAttachment: boolean;
  hasSecondAttachment: boolean;
}