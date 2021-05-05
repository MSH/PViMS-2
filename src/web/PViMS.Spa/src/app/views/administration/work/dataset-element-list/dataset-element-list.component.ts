import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { DatasetElementService } from 'app/shared/services/dataset-element.service';
import { DatasetElementPopupComponent } from './dataset-element-popup/dataset-element.popup.component';
import { DatasetElementDeletePopupComponent } from './dataset-element-delete-popup/dataset-element-delete.popup.component';

@Component({
  templateUrl: './dataset-element-list.component.html',
  styleUrls: ['./dataset-element-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class DatasetElementListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected dialog: MatDialog,
    protected datasetElementService: DatasetElementService,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);
  }

  formControl: FormControl = new FormControl();
  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
      elementName: [this.viewModel.elementName || ''],
    });    
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  } 

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.datasetElementService.getDatasetElements(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching dataset elements");
        });
  }

  openPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Element' : 'Update Element';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetElementPopupComponent, {
      width: '920px',
      minHeight: '530px',
      disableClose: true,
      data: { datasetElementId: isNew ? 0: data.id, title: title, payload: data }
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

  openDeletePopUp(data: any = {}) {
    let self = this;
    let title = 'Delete Element';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetElementDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { datasetElementId: data.id, title: title, payload: data }
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
          (['id', 'element', 'field-type', 'actions']);
  
  elementName: string;
}

class GridRecordModel {
  id: number;
  elementName: string;
  fieldTypeName: string;
}