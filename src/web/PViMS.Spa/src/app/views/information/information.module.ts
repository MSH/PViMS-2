import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PageViewerComponent } from './page-viewer/page-viewer.component';
import { PageListComponent } from './page-list/page-list.component';
import { RouterModule } from '@angular/router';
import { InformationRoutes } from './information-routing.module';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { SharedModule } from 'app/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PageViewerPopupComponent } from './page-viewer-popup/page-viewer.popup.component';

@NgModule({
  declarations: [
    PageViewerComponent, 
    PageListComponent,
    PageViewerPopupComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedComponentsModule,
    SharedMaterialModule,
    SharedModule,
    TranslateModule,
    PerfectScrollbarModule,
    RouterModule.forChild(InformationRoutes)
  ],
  entryComponents:
  [
    PageViewerPopupComponent
  ]
})
export class InformationModule { }
