import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ContactDetailService } from 'app/shared/services/contact-detail.service';

@Component({
  selector: 'contactdetail-popup',
  templateUrl: './contact-detail.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class ContactDetailPopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ContactDetailPopupData,
    public dialogRef: MatDialogRef<ContactDetailPopupComponent>,
    protected contactDetailService: ContactDetailService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      contactType: [this.data.payload.contactType || ''],
      organisationName: [this.data.payload.organisationName || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]],
      contactFirstName: [this.data.payload.contactFirstName || '', [Validators.required, Validators.maxLength(30), Validators.pattern('[a-zA-Z ]*')]],
      contactLastName: [this.data.payload.contactLastName || '', [Validators.required, Validators.maxLength(30), Validators.pattern('[a-zA-Z ]*')]],
      streetAddress: [this.data.payload.streetAddress || '', [Validators.required, Validators.maxLength(100), Validators.pattern('[a-zA-Z0-9 ]*')]],
      city: [this.data.payload.city || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z ]*')]],
      state: [this.data.payload.state || '', [Validators.maxLength(50), Validators.pattern('[a-zA-Z ]*')]],
      countryCode: [this.data.payload.countryCode || '', [Validators.maxLength(10), Validators.pattern('[0-9 ]*')]],
      postCode: [this.data.payload.postCode || '', [Validators.maxLength(20), Validators.pattern('[a-zA-Z0-9 ]*')]],
      contactNumber: [this.data.payload.contactNumber || '', [Validators.maxLength(50), Validators.pattern('[0-9 ]*')]],
      contactEmail: [this.data.payload.contactEmail || '', [Validators.maxLength(50), Validators.pattern('[-a-zA-Z0-9@._]*')]]
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.contactId > 0) {
        self.loadData();
    }
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

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.contactDetailService.getContactDetail(self.data.contactId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        self.throwError(error, error.statusText);
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.contactDetailService.saveContact(self.data.contactId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Contact saved successfully", "Contact Details");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        if(error.status == 400) {
          self.showInfo(error.error.message[0], error.statusText);
        } else {
          self.throwError(error, error.statusText);
        }
    });
  }
}

export interface ContactDetailPopupData {
  contactId: number;
  title: string;
  payload: any;
}