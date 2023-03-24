import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { AccountService } from 'app/shared/services/account.service';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { EventService } from 'app/shared/services/event.service';
import { PopupService } from 'app/shared/services/popup.service';
import { ApexAxisChartSeries, ApexChart, ApexDataLabels, ApexXAxis, ApexLegend, ChartComponent, ApexNonAxisChartSeries } from 'ng-apexcharts';
import { takeUntil, finalize } from 'rxjs/operators';
import { CohortGroupDetailModel } from 'app/shared/models/cohort/cohort-group.detail.model';
import { DashboardService } from 'app/shared/services/dashboard.service';
import { DashboardDetailModel } from 'app/shared/models/dashboard/dashboard.detail.model';
import { forkJoin } from 'rxjs';

export type ChartOptions = {
  axisSeries?: ApexAxisChartSeries;
  nonAxisSeries?: ApexNonAxisChartSeries;
  chart: ApexChart;
  colors: string[];
  dataLabels: ApexDataLabels;
  legend: ApexLegend;
  xaxis?: ApexXAxis;
  labels?: any;
};

@Component({
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent extends BaseComponent implements OnInit, AfterViewInit {
  @ViewChild("chart") chart: ChartComponent;

  viewModel: ViewModel = new ViewModel();

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected cohortGroupService: CohortGroupService,
    protected dashboardService: DashboardService,
  ) 
  { 
    super(_router, _location, popupService, accountService, eventService);
  }

  ngOnInit() {
    let self = this;
  }

  ngAfterViewInit(): void {
    let self = this;
    self.loadData();
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    requestArray.push(self.cohortGroupService.getAllCohortGroups());
    requestArray.push(self.dashboardService.getAllDashboards());

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.CLog(data[0], 'data[0]');
          self.viewModel.cohorts = data[0] as CohortGroupDetailModel[];

          self.CLog(data[1], 'data[1]');
          self.viewModel.dashboards = data[1] as DashboardDetailModel[];
          
          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });    
  }  
  
  selectCohort(cohort: CohortGroupDetailModel): void {
    let self = this;
    self.viewModel.selectedCohort = cohort;
  }  
  
  selectDashboard(dashboard: DashboardDetailModel): void {
    let self = this;

    self.viewModel.selectedDashboard = dashboard;    
    self.dashboardService.generateDashboard(dashboard.id)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.CLog(result, 'result');
        self.viewModel.charts = result as ChartOptions[];
      }, error => {
        self.handleError(error, "Error generating dashboards");
      });
  } 
}

class ViewModel {
  cohorts: CohortGroupDetailModel[] = [];
  dashboards: DashboardDetailModel[] = [];
  charts: ChartOptions[] = [];

  selectedCohort: CohortGroupDetailModel;
  selectedDashboard: DashboardDetailModel;
}