import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { MetaFormDetailModel } from 'app/shared/models/meta/meta-form.detail.model';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { finalize, map, takeUntil } from 'rxjs/operators';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { PopupService } from 'app/shared/services/popup.service';
import { _routes } from 'app/config/routes';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';

@Component({
  templateUrl: './form-select.component.html',
  animations: egretAnimations
})
export class FormSelectComponent extends BaseComponent implements OnInit, AfterViewInit {

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected cohortGroupService: CohortGroupService,
    protected metaFormService: MetaFormService,
  ) 
  { 
    super(_router, _location, popupService, accountService, eventService);    
  }

  ngOnInit(): void {
    const self = this;
    
    self.viewModel.cohortGroupId = +self._activatedRoute.snapshot.paramMap.get('cohortGroupId');

    self.accountService.connected$.subscribe(val => {
      self.viewModel.connected = val;
    });

    self.viewModelForm = self._formBuilder.group({
      cohortName: [''],
      cohortCode: [''],
      conditionName: [''],
    });    
  }

  ngAfterViewInit(): void {
    let self = this;

    self.loadData();
    self.getMetaFormList();
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.cohortGroupService.getCohortGroupDetail(self.viewModel.cohortGroupId)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, (self.viewModel = result));
      }, error => {
        this.handleError(error, "Error fetching cohort group");
      });
  }  

  getMetaFormList(): void {
    let self = this;
    let cohortId = self.viewModel.cohortGroupId;
    self.metaFormService.getAllMetaForms()
    .pipe(
      map((result: MetaFormDetailModel[]) => {
        result.forEach(function (value) {
          self.metaFormService.getAllFormsForType('FormADR').then(result => {
            value.unsynchedCount = result.value.filter(v => v.synchStatus == 'Not Synched').length;
            value.completedCount = result.value.filter(v => v.synchStatus == 'Not Synched' && v.completeStatus == 'Complete').length;
            value.synchedCount = result.value.filter(v => v.synchStatus == 'Synched').length;
          }, error => {
            self.throwError(error, error.statusText);
          });              
        })

        return result;
      })
    )    
    .pipe(takeUntil(self._unsubscribeAll))
    .subscribe(result => {
        self.CLog(cohortId, 'cohortId');
        let forms = result as MetaFormDetailModel[];
        self.viewModel.formList = forms;
        if(cohortId == 2) {
          self.viewModel.formList = forms?.filter(f => f.id == 5);
        }
        if(cohortId == 3) {
          self.viewModel.formList = forms?.filter(f => f.id == 1 || f.id == 2 || f.id == 3);
        }
        if(cohortId == 4) {
          self.viewModel.formList = forms?.filter(f => f.id == 6 || f.id == 7);
        }
        if(cohortId == 5) {
          self.viewModel.formList = forms?.filter(f => f.id == 8);
        }
    }, error => {
        self.throwError(error, error.statusText);
    });
  }

  addForm(selectedOption: string): void {
    let self = this;
    switch (selectedOption) {
      case 'Form A':
        self._router.navigate([_routes.clinical.forms.viewFormA(0)]);
        break;

      case 'Form B':
        self._router.navigate([_routes.clinical.forms.viewFormB(0)]);
        break;

      case 'Form C':
        self._router.navigate([_routes.clinical.forms.viewFormC(0)]);
        break;

      case 'ADR Form':
        self._router.navigate([_routes.clinical.forms.viewFormADR(0)]);
        break;

      case 'Form A TPT':
        self._router.navigate([_routes.clinical.forms.viewFormATPT(0)]);
        break;

      case 'Form B TPT':
        self._router.navigate([_routes.clinical.forms.viewFormBTPT(0)]);
        break;
    }
  }
  
  listForm(selectedOption: string): void {
    let self = this;
    switch (selectedOption) {
      case 'Form A':
        self._router.navigate([_routes.clinical.forms.listForm('FormA')]);
        break;

      case 'Form B':
        self._router.navigate([_routes.clinical.forms.listForm('FormB')]);
        break;

      case 'Form C':
        self._router.navigate([_routes.clinical.forms.listForm('FormC')]);
        break;

      case 'ADR Form':
        self._router.navigate([_routes.clinical.forms.listForm('FormADR')]);
        break;
    }

  }

  synchroniseForm(selectedOption: string): void {
    let self = this;
    switch (selectedOption) {
      case 'Form A':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormA')]);
        break;

      case 'Form B':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormB')]);
        break;

      case 'Form C':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormC')]);
        break;

      case 'ADR Form':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormADR')]);
        break;
    }
  }
  
  navigateToCohortSelect(): void {
    let self = this;
    self._router.navigate([_routes.clinical.forms.cohortselect]);
  }  
}

class ViewModel {
  cohortGroupId: number;

  cohortName: string;
  cohortCode: string;
  conditionName: string;
    
  formList: MetaFormDetailModel[] = [];
  filteredFormList: MetaFormDetailModel[] = [];
  connected: boolean = true;
}