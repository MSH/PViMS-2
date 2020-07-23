import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { FacilityService } from 'app/shared/services/facility.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { FacilityTypeIdentifierModel } from 'app/shared/models/facility/facilitytype.identifier.model';

@Component({
  selector: 'facility-popup',
  templateUrl: './facility.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FacilityPopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  typeList: FacilityTypeIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FacilityPopupData,
    public dialogRef: MatDialogRef<FacilityPopupComponent>,
    protected facilityService: FacilityService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this.formBuilder.group({
        facilityName: [this.data.payload.facilityName || '', [Validators.required, Validators.maxLength(100), Validators.pattern("[-a-zA-Z0-9. '()]*")]],
        facilityCode: [this.data.payload.facilityCode || '', [Validators.required, Validators.maxLength(10), Validators.pattern("[-a-zA-Z0-9]*")]],
        facilityType: [this.data.payload.facilityType || '', Validators.required],
        contactNumber: [this.data.payload.contactNumber || '', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z0-9]*")]],
        mobileNumber: [this.data.payload.mobileNumber || '', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z0-9]*")]],
        faxNumber: [this.data.payload.faxNumber || '', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z0-9]*")]],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.facilityId > 0) {
        self.loadData();
    }
  }
  
  loadDropDowns(): void {
    let self = this;
    self.getFacilityTypeList();
  }

  getFacilityTypeList(): void {
    let self = this;
    self.facilityService.getFacilityTypeList()
        .subscribe(result => {
            self.typeList = result.value;
        }, error => {
            self.throwError(error, error.statusText);
        });
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
    self.facilityService.getFacilityDetail(self.data.facilityId)
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

    if(self.data.facilityId == 0) {
      self.facilityService.saveFacility(self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify("Facility successfully saved!", "Success");
          this.dialogRef.close(this.itemForm.value);
      }, error => {
          if(error.status == 400) {
            self.showInfo(error.error.message[0], error.statusText);
          } else {
            self.throwError(error, error.statusText);
          }
      });
    }
    else {
      self.facilityService.updateFacility(self.data.facilityId, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify("Facility successfully updated!", "Success");
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
}

export interface FacilityPopupData {
  facilityId: number;
  title: string;
  payload: any;
}