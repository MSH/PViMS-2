import { Component, OnInit } from "@angular/core";
import { AccountService } from "app/shared/services/account.service";
import { interval, Subscription } from "rxjs";
import { EventService } from "app/shared/services/event.service";
import { _events } from "app/config/events";

@Component({
  selector: "app-ping",
  templateUrl: "./ping.component.html",
  styleUrls: ["./ping.component.scss"]
})
export class PingComponent implements OnInit {
  
  subscription: Subscription;
  pingInterval: number;
  checking: boolean;

  constructor(
    public accountService: AccountService,
    protected eventService: EventService
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
  }

  pingAPI(): void {
    this.checking = true;
    this.eventService.broadcast(_events.ping_event);    
    this.accountService.ping()
      .subscribe(result => {
        this.accountService.setConnectionStatus(true);
        this.checking = false;
      }, error => {
        this.accountService.setConnectionStatus(false);
        this.checking = false;
      });
  }

}
