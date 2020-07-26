import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import * as moment from 'moment';

@Component({
  templateUrl: './spontaneous-medications.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class SpontaneousMedicationsPopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: SpontaneousMedicationsPopupData,
    public dialogRef: MatDialogRef<SpontaneousMedicationsPopupComponent>,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      product: [this.data.payload.product || '', Validators.required],
      suspected: [this.data.payload.suspected || '', Validators.required],
      strength: [this.data.payload.strength || ''],
      strengthUnit: [this.data.payload.strengthUnit || ''],
      dose: [this.data.payload.dose || ''],
      doseUnit: [this.data.payload.doseUnit || ''],
      route: [this.data.payload.route || ''],
      medicationStartDate: [ Object.keys(this.data.payload).length > 0 ? moment(this.data.payload.startDate, "YYYY-MM-DD") : '', Validators.required],
      medicationEndDate: [ Object.keys(this.data.payload).length > 0 ? this.data.payload.endDate.length > 0 ? moment(this.data.payload.endDate, "YYYY-MM-DD") : '' : '' ],
    })
  }

  public setBusy(value: boolean): void {
    setTimeout(() => { this.busy = value; });
  }

  public isBusy(): boolean {
    return this.busy;
  }

  protected notify(message: string, action: string) {
    return this.popupService.notify(message, action);
  }

  protected showError(errorMessage: any, title: string = "Error") {
    this.popupService.showErrorMessage(errorMessage, title);
  }

  protected showInfo(message: string, title: string = "Info") {
    this.popupService.showInfoMessage(message, title);
  }

  protected updateForm(form: FormGroup, value: any): void {
    form.patchValue(value);
  }  

  protected throwError(errorObject: any, title: string = "Exception") {
    if (errorObject.status == 401) {
        this.showError(errorObject.error.message, errorObject.error.statusCodeType);
    } else {
        this.showError(errorObject.message, title);
    }
  }

  submit() {
    let self = this;
    self.notify("Product saved successfully", "Products");
    this.dialogRef.close(this.itemForm.value);
  }
}

export interface SpontaneousMedicationsPopupData {
  title: string;
  payload: any;
}