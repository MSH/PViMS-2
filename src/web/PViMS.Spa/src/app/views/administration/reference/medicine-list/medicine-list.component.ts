import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatPaginator, MatDialogRef, MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MedicationPopupComponent } from './medication-popup/medication.popup.component';
import { ConceptService } from 'app/shared/services/concept.service';
import { MedicationDeletePopupComponent } from './medication-delete-popup/medication-delete.popup.component';

@Component({
  templateUrl: './medicine-list.component.html',
  styleUrls: ['./medicine-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class MedicineListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected conceptService: ConceptService,
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

  @ViewChild('mainGridPaginator', { static: false }) mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
      searchTerm: [this.viewModel.searchTerm || ''],
      active: ['Both']
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

  setupTable() {
  };

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.conceptService.searchProductsByDetail(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error searching for products");
        });
  }

  openPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Medication' : 'Update Medication';
    let dialogRef: MatDialogRef<any> = self.dialog.open(MedicationPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { productId: isNew ? 0: data.id, title: title, payload: data }
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
    let title = 'Delete Medication';
    let dialogRef: MatDialogRef<any> = self.dialog.open(MedicationDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { productId: data.id, title: title, payload: data }
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
          (['id', 'product', 'active-ingredient', 'form', 'manufacturer', 'active', 'actions']);

  searchTerm: string;
}

class GridRecordModel {
  id: number;
  productName: string;
  conceptName: string;
  formName: string;
  manufacturer: string;
  active: string;
}
