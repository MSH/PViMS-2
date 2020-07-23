import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize, startWith, map, takeUntil } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ConceptService } from 'app/shared/services/concept.service';
import { ConceptIdentifierModel } from 'app/shared/models/concepts/concept.identifier.model';
import { Observable } from 'rxjs';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { MedicationFormIdentifierModel } from 'app/shared/models/concepts/medication-form.identifier.model';

@Component({
  selector: 'medication-popup',
  templateUrl: './medication.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class MedicationPopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  conceptList: ConceptIdentifierModel[] = [];
  filteredConceptList: Observable<ConceptIdentifierModel[]>;

  formList: MedicationFormIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MedicationPopupData,
    public dialogRef: MatDialogRef<MedicationPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected conceptService: ConceptService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this._formBuilder.group({
      conceptName: [this.data.payload.conceptName || '', [Validators.required, Validators.maxLength(1000), Validators.pattern('[a-zA-Z0-9 ]*')]],
      formName: [this.data.payload.formName || '', Validators.required],
      productName: [this.data.payload.productName || '', [Validators.required, Validators.maxLength(200), Validators.pattern('[a-zA-Z0-9 ]*')]],
      manufacturer: [this.data.payload.manufacturer || '', [Validators.required, Validators.maxLength(200), Validators.pattern('[a-zA-Z0-9 ]*')]],
      active: [this.data.payload.active, Validators.required]
    })

    this.filteredConceptList = this.itemForm.controls['conceptName'].valueChanges
      .pipe(
        startWith(''),
        map(value => this._conceptFilter(value))
    );    
  }

  loadDropDowns(): void {
    let self = this;
    self.getConceptList();
    self.getMedicationFormList();
  }

  getMedicationFormList(): void {
    let self = this;
    self.conceptService.getAllMedicationForms()
        .subscribe(result => {
            self.formList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  getConceptList(): void {
    let self = this;
    // self.conceptService.getConceptList()
    //     .subscribe(result => {
    //         self.conceptList = result.value;
    //     }, error => {
    //         self.throwError(error, error.statusText);
    //     });
  }

  submit() {
    let self = this;
    self.setBusy(true);

    // self.medicationService.saveMedication(self.data.medicationId, self.itemForm.value)
    //   .pipe(finalize(() => self.setBusy(false)))
    //   .subscribe(result => {
    //     self.notify("Medication saved successfully", "Medications");
    //     this.dialogRef.close(this.itemForm.value);
    // }, error => {
    //     if(error.status == 400) {
    //       self.showInfo(error.error.message, error.statusText);
    //     } else {
    //       self.throwError(error, error.statusText);
    //     }
    // });
  }

  private _conceptFilter(value: string): ConceptIdentifierModel[] {
    const filterValue = value.toLowerCase();

    return this.conceptList.filter(option => option.conceptName.toLowerCase().includes(filterValue));
  }  
}

export interface MedicationPopupData {
  productId: number;
  title: string;
  payload: any;
}