import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { DatasetElementService } from 'app/shared/services/dataset-element.service';

@Component({
  templateUrl: './dataset-element.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class DatasetElementPopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DatasetElementPopupData,
    public dialogRef: MatDialogRef<DatasetElementPopupComponent>,
    protected datasetElementService: DatasetElementService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      elementName: [this.data.payload.elementName || '', [Validators.required, Validators.maxLength(100), Validators.pattern('[a-zA-Z() ]*')]],
      oid: ['', [Validators.maxLength(50), Validators.pattern('[-a-zA-Z0-9 ]*')]],
      defaultValue: ['', [Validators.maxLength(150), Validators.pattern('[-a-zA-Z0-9 ]*')]],
      mandatory: ['', Validators.required],
      anonymise: ['', Validators.required],
      system: ['', Validators.required],
      singleDatasetRule: ['', Validators.required],
      fieldTypeName: [this.data.payload.fieldTypeName || '', Validators.required],
      maxLength: [''],
      decimals: [''],
      minSize: [''],
      maxSize: [''],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.datasetElementId > 0) {
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
    self.datasetElementService.getDatasetElementExpanded(self.data.datasetElementId)
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

    self.datasetElementService.saveDatasetElement(self.data.datasetElementId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset element saved successfully", "Dataset Elements");
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

export interface DatasetElementPopupData {
  datasetElementId: number;
  title: string;
  payload: any;
}