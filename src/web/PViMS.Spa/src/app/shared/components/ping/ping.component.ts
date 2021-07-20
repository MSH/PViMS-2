import { Component, OnInit } from "@angular/core";
import { AccountService } from "app/shared/services/account.service";
import { interval, Subscription } from "rxjs";
import { EventService } from "app/shared/services/event.service";
import { _events } from "app/config/events";
import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { OnlineStatusPopupComponent } from "../popup/online-status-popup/online-status.popup.component";

@Component({
  selector: "app-ping",
  templateUrl: "./ping.component.html",
  styleUrls: ["./ping.component.scss"]
})
export class PingComponent implements OnInit {
  
  subscription: Subscription;
  pingInterval: number;
  
  viewModel: ViewModel = new ViewModel();

  constructor(
    protected accountService: AccountService,
    protected eventService: EventService,
    protected dialog: MatDialog
  ) {
    this.pingAPI();

    // set up interval for pinging API
    this.pingInterval = 600;
    const source = interval(this.pingInterval*1000);
    
    this.subscription = source.subscribe(val => 
      {
        this.pingAPI();
      });
  }

  ngOnInit() {
    const self = this;
    self.accountService.connected$.subscribe(val => {
      console.log(`Marked connected in ping as ${val}`);
      self.viewModel.connected = val;
    });
  }

  pingAPI(): void {
    const self = this;
    self.viewModel.checking = true;
    self.eventService.broadcast(_events.ping_event);    
    self.accountService.ping()
      .subscribe(result => {
        self.accountService.setConnectionStatus(true);
        self.viewModel.checking = false;
      }, error => {
        self.accountService.setConnectionStatus(false);
        self.viewModel.checking = false;
      });
  }

  openOnlineStatusPopup() {
    let self = this;
    let title = 'View Online Status';
    let dialogRef: MatDialogRef<any> = self.dialog.open(OnlineStatusPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
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

class ViewModel {
  checking: boolean = false;
  connected: boolean = true;
}
