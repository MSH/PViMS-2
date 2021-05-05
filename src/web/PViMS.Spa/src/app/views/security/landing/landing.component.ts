import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { _routes } from 'app/config/routes';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AboutPopupComponent } from 'app/shared/components/about/about.popup.component';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss']
})
export class LandingComponent implements OnInit {

  constructor(
    protected _router: Router,
    protected dialog: MatDialog
  ) { }

  ngOnInit() {
  }

  navigateToSpontaneous(): void {
    let self = this;
    self._router.navigate([_routes.public.spontaneous]);
  }

  navigateToLogin(): void {
    let self = this;
    self._router.navigate([_routes.security.login]);
  }

  openAboutPopUp(data: any = {}) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(AboutPopupComponent, {
      width: '920px',
      disableClose: true
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
      })
  }  
}
