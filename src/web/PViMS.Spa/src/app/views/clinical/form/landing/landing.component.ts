import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { MetaFormDetailModel } from 'app/shared/models/meta/meta-form.detail.model';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { takeUntil } from 'rxjs/operators';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { PopupService } from 'app/shared/services/popup.service';
import { _routes } from 'app/config/routes';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss']
})
export class LandingComponent extends BaseComponent implements OnInit {

  formList: MetaFormDetailModel[] = [];

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,    
    protected metaFormService: MetaFormService,
  ) 
  { 
    super(_router, _location, popupService, accountService, eventService);    
  }

  ngOnInit(): void {
    const self = this;
    self.getMetaFormList();
  }

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
          
    }    
  }  
}