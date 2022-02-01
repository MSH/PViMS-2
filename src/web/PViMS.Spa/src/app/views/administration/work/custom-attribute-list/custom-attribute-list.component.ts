import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
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
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';

@Component({
  templateUrl: './custom-attribute-list.component.html',
  styles: [`
    .addButton { display: flex; justify-content: space-between; button { margin-left: auto; } }
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-identifier { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-patient { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-medication-summary { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],
  animations: egretAnimations
})
export class CustomAttributeListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected dialog: MatDialog,
    protected customAttributeService: CustomAttributeService,
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
      extendableTypeName: [''],
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

    self.customAttributeService.getCustomAttributes(self.viewModel.extendableTypeName, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching attributes");
        });
  }

  openPopUp(data: any = {}, isNew?) {
    // let self = this;
    // let title = isNew ? 'Add Element' : 'Update Element';
    // let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetElementPopupComponent, {
    //   width: '920px',
    //   minHeight: '530px',
    //   disableClose: true,
    //   data: { datasetElementId: isNew ? 0: data.id, title: title, payload: data }
    // })
    // dialogRef.afterClosed()
    //   .subscribe(res => {
    //     if(!res) {
    //       // If user press cancel
    //       return;
    //     }
    //     self.loadGrid();
    //   })
  }

  openDeletePopUp(data: any = {}) {
    // let self = this;
    // let title = 'Delete Element';
    // let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetElementDeletePopupComponent, {
    //   width: '720px',
    //   disableClose: true,
    //   data: { datasetElementId: data.id, title: title, payload: data }
    // })
    // dialogRef.afterClosed()
    //   .subscribe(res => {
    //     if(!res) {
    //       // If user press cancel
    //       return;
    //     }
    //     self.loadGrid();
    //   })
  }

  selectExtendableType(extendableTypeName: string): void {
    this.viewModel.extendableTypeName = extendableTypeName;
    this.loadGrid();
  }  

}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['id', 'attribute-key', 'category', 'attribute-type', 'actions']);
  
  extendableTypeName: string = 'Patient';
}

class GridRecordModel {
  id: number;
  extendableTypeName: string;
  attributeKey: string;
  category: string;
  customAttributeType: string;
}