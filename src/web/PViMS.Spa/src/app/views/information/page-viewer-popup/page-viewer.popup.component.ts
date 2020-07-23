import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { finalize } from 'rxjs/operators';
import { MetaPageExpandedModel } from 'app/shared/models/meta/meta-page.expanded.model';
import { MetaWidgetDetailModel } from 'app/shared/models/meta/meta-widget.detail.model';
import { MetaService } from 'app/shared/services/meta.service';

@Component({
  templateUrl: './page-viewer.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class PageViewerPopupComponent implements OnInit, AfterViewInit {

  public itemForm: FormGroup;
  
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PageViewerPopupData,
    public dialogRef: MatDialogRef<PageViewerPopupComponent>,
    protected popupService: PopupService,
    protected metaService: MetaService,
    protected formBuilder: FormBuilder,
  ) { }

  metaPage: MetaPageExpandedModel;

  topLeft: MetaWidgetDetailModel;
  topRight: MetaWidgetDetailModel;
  middleLeft: MetaWidgetDetailModel;
  middleRight: MetaWidgetDetailModel;
  bottomLeft: MetaWidgetDetailModel;
  bottomRight: MetaWidgetDetailModel;


  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.metaPageId > 0) {
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
    self.metaService.getMetaPageExpanded(self.data.metaPageId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.metaPage = result;

        self.topLeft = self.metaPage.widgets.find(w => w.widgetLocation == 'TopLeft');
        self.topRight = self.metaPage.widgets.find(w => w.widgetLocation == 'TopRight');
        self.middleLeft = self.metaPage.widgets.find(w => w.widgetLocation == 'MiddleLeft');
        self.middleRight = self.metaPage.widgets.find(w => w.widgetLocation == 'MiddleRight');
        self.bottomLeft = self.metaPage.widgets.find(w => w.widgetLocation == 'BottomLeft');
        self.bottomRight = self.metaPage.widgets.find(w => w.widgetLocation == 'BottomRight');

      }, error => {
        self.throwError(error, error.statusText);
      });
  }

  selectTerm(data: any) {
    this.dialogRef.close(data);    
  }
}

export interface PageViewerPopupData {
  metaPageId: number;
  title: string;
}