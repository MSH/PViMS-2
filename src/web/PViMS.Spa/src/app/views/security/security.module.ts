import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from "@angular/router";
import { SharedMaterialModule } from 'app/shared/shared-material.module';

import { FlexLayoutModule } from '@angular/flex-layout';

// import { CommonDirectivesModule } from './sdirectives/common/common-directives.module';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { LockscreenComponent } from './lockscreen/lockscreen.component';
import { SecurityRoutes } from "./security.routing";
import { NotFoundComponent } from './not-found/not-found.component';
import { LoginComponent } from './login/login.component';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedModule } from 'app/shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { AcceptEulaPopupComponent } from './accept-eula/accept-eula.popup.component';
import { SpontaneousComponent } from './spontaneous/spontaneous.component';
import { SpontaneousLabsPopupComponent } from './spontaneous/spontaneous-labs-popup/spontaneous-labs.popup.component';
import { SpontaneousMedicationsPopupComponent } from './spontaneous/spontaneous-medications-popup/spontaneous-medications.popup.component';
import { LandingComponent } from './landing/landing.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedMaterialModule,
    SharedComponentsModule,
    SharedModule,
    TranslateModule,
    FlexLayoutModule,
    PerfectScrollbarModule,
    RouterModule.forChild(SecurityRoutes)
  ],
  declarations: [
    ForgotPasswordComponent, 
    LockscreenComponent, 
    NotFoundComponent, 
    LoginComponent,
    SpontaneousComponent,
    SpontaneousLabsPopupComponent,
    SpontaneousMedicationsPopupComponent,
    AcceptEulaPopupComponent,
    LandingComponent
  ],
  entryComponents: [
    AcceptEulaPopupComponent,
    SpontaneousLabsPopupComponent,
    SpontaneousMedicationsPopupComponent
  ]
})
export class SessionsModule { }