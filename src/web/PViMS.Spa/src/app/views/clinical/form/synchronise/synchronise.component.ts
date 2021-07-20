import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { GridModel } from 'app/shared/models/grid.model';
import { _routes } from 'app/config/routes';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ViewErrorPopupComponent } from './viewerror-popup/viewerror.popup.component';

@Component({
  selector: 'app-synchronise',
  templateUrl: './synchronise.component.html',
  styleUrls: ['./synchronise.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class SynchroniseComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected metaFormService: MetaFormService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog,) 
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

  selectToggleFlag = false;

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

    self.accountService.connected$.subscribe(val => {
        console.log(`Marked ${val}`);
        self.viewModel.connected = val;
      });

    self.loadGrid();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(SynchroniseComponent.name);
  }

  selectToggleAll() {
    this.selectToggleFlag = !this.selectToggleFlag;
    this.viewModel.mainGrid.records.data.forEach(form => { form.selected = this.selectToggleFlag });
  }

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.searchUnsynchedForms().then(result => {
        self.viewModel.mainGrid.updateAdvance(result);
        self.setBusy(false);
    }, error => {
        self.throwError(error, error.statusText);
    });
  }  

  synchForms(): void {
    let self = this;

    if (this.viewModel.mainGrid.records.data.filter(form => form.selected).length == 0){
      self.showInfo("Please specify one form", "Synchronise");
      return;
    }

    // Loop through all selected forms
    this.viewModel.mainGrid.records.data
      .filter(form => form.selected)
      .forEach((form, i)  => {
        setTimeout(() => {
          this.displaySynchResult(form);
        }, i * 1000);
      });
  }

  displaySynchResult(form: GridRecordModel) {
    let self = this;

    // Notify UI that the form is about to be loaded
    form.synchStatus = "InProgress";

    console.log(form);
    self.metaFormService.saveFormToAPI(form)
      .subscribe(result => {
        form.synchStatus = "Successful";
        form.selected = false;
        this.metaFormService.markFormAsSynched(form.id);
      }, error => {
        self.CLog(error, 'error saving form to API');
        form.synchStatus = "Error";
        let messages = [];
        if(error.message) {
          if(Array.isArray(error.message)) {
            form.synchMessages = error.message;
          } 
          else {
            messages.push(error.message);
            form.synchMessages = messages;
          }
        }
        else {
          messages.push('Unhandled exception processing form. Please contact a system administrator');
          form.synchMessages = messages;
        }
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

  viewMessagesPopUp(data: string[]) {
    let self = this;
    let title = 'Synchronisation messages';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ViewErrorPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, messages: data }
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
          (['selected', 'identifier', 'patient identifier', 'synch status', 'actions']);

  connected: boolean = true;
}

class GridRecordModel {
  id: number;
  selected: any;
  formIdentifier: string;
  patientIdentifier: string;
  formType: string;
  attachment: any;
  synchStatus: 'None' | 'Error' | 'Successful' | 'InProgress' ;
  synchMessages: string[] = [];
}