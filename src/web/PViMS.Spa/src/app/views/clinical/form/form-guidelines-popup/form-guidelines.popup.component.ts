import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';

@Component({
  templateUrl: './form-guidelines.popup.component.html',
  animations: egretAnimations
})
export class FormGuidelinesPopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<FormGuidelinesPopupComponent>,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;
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
    self.notify("Condition saved successfully", "Conditions");
    this.dialogRef.close(this.itemForm.value);
  }
}

export interface PopupData {
  title: string;
  type: 'A' | 'B' | 'C';
}