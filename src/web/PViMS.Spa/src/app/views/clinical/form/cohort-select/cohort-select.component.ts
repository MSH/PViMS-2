import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { finalize, takeUntil } from 'rxjs/operators';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { PopupService } from 'app/shared/services/popup.service';
import { _routes } from 'app/config/routes';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { CohortGroupIdentifierModel } from 'app/shared/models/cohort/cohort-group.identifier.model';
import { GridModel } from 'app/shared/models/grid.model';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  templateUrl: './cohort-select.component.html'
})
export class CohortSelectComponent extends BaseComponent implements OnInit, AfterViewInit {

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;
  
  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,    
    protected cohortGroupService: CohortGroupService,
  ) 
  { 
    super(_router, _location, popupService, accountService, eventService);    
  }

  ngOnInit(): void {
    const self = this;
    self.viewModelForm = self._formBuilder.group({
    });

    self.accountService.connected$.subscribe(val => {
      self.viewModel.connected = val;
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.cohortGroupService.getCohortGroupsByDetail(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.mainGrid.updateAdvance(result);
      }, error => {
        self.handleError(error, "Error fetching cohorts");
      });
  }

  formList(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.clinical.forms.formSelect(model.id)]);
  }
}

class ViewModel {
  cohortList: CohortGroupIdentifierModel[] = [];

  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['cohort-name', 'primary-condition', 'patient-count', 'form-count', 'synch-count', 'actions']);

  connected: boolean = true;
}

class GridRecordModel {
  id: number;
  cohortName: string;
  cohortCode: string;
  conditionName: string;
  patientCount: number;
  formCount: number;
  synchCount: number;
}