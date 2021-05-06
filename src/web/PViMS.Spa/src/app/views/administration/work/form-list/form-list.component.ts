import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaFormService } from 'app/shared/services/meta-form.service';

@Component({
  templateUrl: './form-list.component.html',
  styleUrls: ['./form-list.component.scss'],
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
  }

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
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

    self.metaFormService.getMetaForms(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  openFormPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Form' : 'Update Form';
    // let dialogRef: MatDialogRef<any> = self.dialog.open(LabResultPopupComponent, {
    //   width: '720px',
    //   disableClose: true,
    //   data: { labResultId: isNew ? 0: data.id, title: title, payload: data }
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
    let self = this;
    let title = 'Delete Form';
    // let dialogRef: MatDialogRef<any> = self.dialog.open(LabResultDeletePopupComponent, {
    //   width: '720px',
    //   disableClose: true,
    //   data: { labResultId: data.id, title: title, payload: data }
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

}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['id', 'form-name', 'unique-identifier', 'system', 'actions']);
}

class GridRecordModel {
  id: number;
  system: string;
  version: string;
  formName: string;
  actionName: string;
  metaFormGuid: string;
}