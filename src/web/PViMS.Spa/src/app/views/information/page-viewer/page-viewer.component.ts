import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { MatDialog, MatDialogRef } from '@angular/material';
import { Subscription } from 'rxjs';
import { MetaService } from 'app/shared/services/meta.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { MetaPageExpandedModel } from 'app/shared/models/meta/meta-page.expanded.model';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaWidgetDetailModel } from 'app/shared/models/meta/meta-widget.detail.model';
import { PageViewerPopupComponent } from '../page-viewer-popup/page-viewer.popup.component';

@Component({
  templateUrl: './page-viewer.component.html',
  styleUrls: ['./page-viewer.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class PageViewerComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected metaService: MetaService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          //this.setupTable();
      }
    });

    // Force an event to refresh the page if the paramter changes (but not the route)
    this.navigationSubscription = this._router.events.subscribe((e: any) => {
      // If it is a NavigationEnd event re-initalise the component
      if (e instanceof NavigationEnd) {
        this.initialiseReport();
      }
    });    
  }

  navigationSubscription;
  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  id: number;
  metaPage: MetaPageExpandedModel;

  topLeft: MetaWidgetDetailModel;
  topRight: MetaWidgetDetailModel;
  middleLeft: MetaWidgetDetailModel;
  middleRight: MetaWidgetDetailModel;
  bottomLeft: MetaWidgetDetailModel;
  bottomRight: MetaWidgetDetailModel;

  ngOnInit() {
    const self = this;
    self.initialiseReport();
  }

  // Force an event to refresh the page if the paramter changes (but not the route)
  initialiseReport(): void {
    // Set default values and re-fetch any data you need.
    const self = this;

    self.id = +self._activatedRoute.snapshot.paramMap.get('id');
    self.loadData();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(PageViewerComponent.name);
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaService.getMetaPageExpanded(self.id)
      .pipe(takeUntil(self._unsubscribeAll))
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

  openPageViewerPopup(id: number) {
    let self = this;
    let title = '';
    let dialogRef: MatDialogRef<any> = self.dialog.open(PageViewerPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { metaPageId: id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
      })
  }  
}
