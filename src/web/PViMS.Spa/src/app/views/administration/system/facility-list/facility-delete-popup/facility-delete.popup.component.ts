import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { FacilityService } from 'app/shared/services/facility.service';

@Component({
  templateUrl: './facility-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FacilityDeletePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FacilityPopupData,
    public dialogRef: MatDialogRef<FacilityDeletePopupComponent>,
    protected facilityService: FacilityService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      facilityName: [this.data.payload.facilityName],
      facilityCode: [this.data.payload.facilityCode],
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

  // loadData(): void {
  //   let self = this;
  //   self.setBusy(true);
  //   self.labTestService.getLabTestIdentifier(self.data.labTestId)
  //     .pipe(finalize(() => self.setBusy(false)))
  //     .subscribe(result => {
  //       self.updateForm(self.itemForm, (self.data.payload = result));
  //     }, error => {
  //       self.throwError(error, error.statusText);
  //     });
  // }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.facilityService.deleteFacility(self.data.facilityId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Facility deleted successfully", "Facilities");
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

export interface FacilityPopupData {
  facilityId: number;
  title: string;
  payload: any;
}