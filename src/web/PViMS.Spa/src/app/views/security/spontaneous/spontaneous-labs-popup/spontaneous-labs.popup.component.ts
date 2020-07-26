import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { LabTestIdentifierModel } from 'app/shared/models/labs/lab-test.identifier.model';
import { LabTestService } from 'app/shared/services/lab-test.service';

@Component({
  templateUrl: './spontaneous-labs.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class SpontaneousLabsPopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: SpontaneousLabsPopupData,
    public dialogRef: MatDialogRef<SpontaneousLabsPopupComponent>,
    protected labTestService: LabTestService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  labTestList: LabTestIdentifierModel[] = [];

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      labTestDate: [new Date(this.data.payload.testResultDate) || '', Validators.required],
      labTest: [this.data.payload.labTest || '', Validators.required],
      labTestResult: [this.data.payload.testResultValue || '', Validators.required],
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
    self.notify("Test result saved successfully", "Test Results");
    self.dialogRef.close(this.itemForm.value);
  }
}

export interface SpontaneousLabsPopupData {
  title: string;
  payload: any;
}