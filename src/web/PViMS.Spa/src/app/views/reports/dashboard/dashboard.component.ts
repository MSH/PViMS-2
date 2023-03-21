import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { AccountService } from 'app/shared/services/account.service';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { EventService } from 'app/shared/services/event.service';
import { PopupService } from 'app/shared/services/popup.service';
import { ApexAxisChartSeries, ApexChart, ApexDataLabels, ApexPlotOptions, ApexYAxis, ApexXAxis, ApexFill, ApexTooltip, ApexStroke, ApexLegend, ChartComponent, ApexNonAxisChartSeries, ApexResponsive, ApexTitleSubtitle } from 'ng-apexcharts';
import { takeUntil, finalize } from 'rxjs/operators';
import { CohortGroupDetailModel } from 'app/shared/models/cohort/cohort-group.detail.model';
import { DashboardService } from 'app/shared/services/dashboard.service';

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

export type PieChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  responsive: ApexResponsive[];
  labels: any;
};

export type BarChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  xaxis: ApexXAxis;
};

export type MultipleChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  markers: any; //ApexMarkers;
  stroke: any; //ApexStroke;
  yaxis: ApexYAxis | ApexYAxis[];
  dataLabels: ApexDataLabels;
  title: ApexTitleSubtitle;
  legend: ApexLegend;
  fill: ApexFill;
  tooltip: ApexTooltip;
};

@Component({
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent extends BaseComponent implements OnInit, AfterViewInit {
  @ViewChild("chart") chart: ChartComponent;
  public facilityDistributionChartOptions: Partial<PieChartOptions>;
  public initCohortEnrollmentCumulativeChartOptions: Partial<ChartOptions>;
  public followUpCumulativeChartOptions: Partial<ChartOptions>;
  public genderDistributionChartOptions: Partial<PieChartOptions>;
  public labDistributionChartOptions: Partial<BarChartOptions>;
  public conditionDistributionChartOptions: Partial<BarChartOptions>;
  public followUpBreakdownChartOptions: Partial<MultipleChartOptions>;

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

      // // prepare elements
      // let chart: ChartOptions = {
      //     axisSeries: [
      //       {
      //         name: "GP SIZWE TROPICAL DISEASES HOSPITAL",
      //         data: [8, 0, 0, 18, 0]
      //       },
      //       {
      //         name: "EC JOSE PEARSON TB HOSPITAL",
      //         data: [0,	0, 0, 45, 13]
      //       },
      //       {
      //         name: "GP HELEN JOSEPH HOSPITAL",
      //         data: [0,	0, 0, 4, 0]
      //       },
      //       {
      //         name: "KZ KING DINUZULU HOSPITAL",
      //         data: [0,	0,	0,	36, 3]
      //       }
      //     ],
      //     chart: {
      //       type: "bar",
      //       height: 350
      //     },
      //     colors: [
      //       "#33b2df",
      //       "#546E7A",
      //       "#d4526e",
      //       "#13d8aa",
      //       "#A5978B",
      //       "#2b908f",
      //       "#f9a3a4",
      //       "#90ee7e",
      //       "#f48024",
      //       "#69d2e7"
      //     ],      
      //     dataLabels: {
      //       enabled: false
      //     },
      //     xaxis: {
      //       categories: ['May-22', 'Jun-22', 'July-22', 'Aug-22', 'Sep-22']
      //     },
      //     legend: {
      //       horizontalAlign: "left",
      //       offsetX: 40
      //     }
      // }; 
      // this.viewModel.charts.push(chart);    
      // chart = {
      //   nonAxisSeries: [67, 60],
      //   chart: {
      //     type: "pie",
      //     height: 350
      //   },
      //   colors: [
      //     "#33b2df",
      //     "#546E7A",
      //     "#d4526e",
      //     "#13d8aa",
      //     "#A5978B",
      //     "#2b908f",
      //     "#f9a3a4",
      //     "#90ee7e",
      //     "#f48024",
      //     "#69d2e7"
      //   ],      
      //   dataLabels: {
      //     enabled: false
      //   },
      //   legend: {
      //     horizontalAlign: "left",
      //     offsetX: 40
      //   },
      //   labels: ["Male", "Female"],
      // }; 
      // this.viewModel.charts.push(chart);    
  }

  ngOnInit() {
    let self = this;
    // self.initCohortEnrollmentCumulativeChart();
    // self.followUpCumulativeChart();
    // self.facilityDistributionChart();
    // self.genderDistributionChart();
    // // self.labDistributionChart();
    // // self.conditionDistributionChart();
    // self.followUpBreakdownChart();
  }

  ngAfterViewInit(): void {
    let self = this;
    self.loadCohorts();
    self.selectDashboard();
  }

  loadCohorts(): void {
    let self = this;
    self.setBusy(true);

    self.cohortGroupService.getAllCohortGroups()
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.CLog(result, 'result');
        self.viewModel.cohortList = result;
      }, error => {
        self.handleError(error, "Error fetching cohorts");
      });
  } 
  
  selectCohort(cohort: CohortGroupDetailModel): void {
    let self = this;
    self.viewModel.selectedCohort = cohort;
  }  
  
  selectDashboard(): void {
    let self = this;

    self.dashboardService.generateDashboard(1)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.CLog(result, 'result');
        self.viewModel.charts = result as ChartOptions[];
      }, error => {
        self.handleError(error, "Error fetching cohorts");
      });
  } 
    
  // private initCohortEnrollmentCumulativeChart() {
  //   this.initCohortEnrollmentCumulativeChartOptions = {
  //     series: [
  //       {
  //         name: "GP SIZWE TROPICAL DISEASES HOSPITAL",
  //         data: [8, 0, 0, 18, 0]
  //       },
  //       {
  //         name: "EC JOSE PEARSON TB HOSPITAL",
  //         data: [0,	0, 0, 45, 13]
  //       },
  //       {
  //         name: "GP HELEN JOSEPH HOSPITAL",
  //         data: [0,	0, 0, 4, 0]
  //       },
  //       {
  //         name: "KZ KING DINUZULU HOSPITAL",
  //         data: [0,	0,	0,	36, 3]
  //       }
  //     ],
  //     chart: {
  //       type: "bar",
  //       height: 350
  //     },
  //     colors: [
  //       "#33b2df",
  //       "#546E7A",
  //       "#d4526e",
  //       "#13d8aa",
  //       "#A5978B",
  //       "#2b908f",
  //       "#f9a3a4",
  //       "#90ee7e",
  //       "#f48024",
  //       "#69d2e7"
  //     ],      
  //     plotOptions: {
  //       bar: {
  //         horizontal: false
  //       }
  //     },
  //     dataLabels: {
  //       enabled: false
  //     },
  //     stroke: {
  //       show: true,
  //       width: 2,
  //       colors: ["transparent"]
  //     },
  //     xaxis: {
  //       categories: ['May-22', 'Jun-22', 'July-22', 'Aug-22', 'Sep-22']
  //     },
  //     yaxis: {
  //       title: {
  //         text: ""
  //       }
  //     },
  //     fill: {
  //       opacity: 1
  //     },
  //     tooltip: {
  //       y: {
  //         formatter: function(val) {
  //           return val + " enrolments";
  //         }
  //       }
  //     }
  //   };
  // }

  // private followUpCumulativeChart() {
  //   this.followUpCumulativeChartOptions = {
  //     series: [
  //       {
  //         name: "GP SIZWE TROPICAL DISEASES HOSPITAL",
  //         data: [0, 0, 0, 0, 0, 0, 0, 0]
  //       },
  //       {
  //         name: "EC JOSE PEARSON TB HOSPITAL",
  //         data: [0, 0, 0, 0, 0, 0, 0, 0]
  //       },
  //       {
  //         name: "GP HELEN JOSEPH HOSPITAL",
  //         data: [0, 0, 2, 0, 2, 0, 0, 0]
  //       },
  //       {
  //         name: "KZ KING DINUZULU HOSPITAL",
  //         data: [0, 0, 0, 0, 0, 0, 0, 0]
  //       }
  //     ],
  //     chart: {
  //       type: "bar",
  //       height: 350,
  //       stacked: true
  //     },
  //     colors: [
  //       "#33b2df",
  //       "#546E7A",
  //       "#d4526e",
  //       "#13d8aa",
  //       "#A5978B",
  //       "#2b908f",
  //       "#f9a3a4",
  //       "#90ee7e",
  //       "#f48024",
  //       "#69d2e7"
  //     ],      
  //     plotOptions: {
  //       bar: {
  //         horizontal: true,
  //       }
  //     },
  //     dataLabels: {
  //       enabled: false
  //     },
  //     stroke: {
  //       show: true,
  //       width: 2,
  //       colors: ["transparent"]
  //     },
  //     xaxis: {
  //       categories: ['8am to 9am', '9:00am to 10am', '10am to 11am', '12pm to 1pm', '1pm to 2pm', '2pm to 3pm', '3pm to 4pm', '4pm to 5pm']
  //     },
  //     yaxis: {
  //       title: {
  //         text: ""
  //       }
  //     },
  //     fill: {
  //       opacity: 1
  //     },
  //     tooltip: {
  //       y: {
  //         formatter: function(val) {
  //           return val + " reports";
  //         }
  //       }
  //     }
  //   };
  // }

  // private facilityDistributionChart() {
  //   this.facilityDistributionChartOptions = {
  //     series: [58, 4, 26, 39],
  //     chart: {
  //       height: 360,
  //       type: "pie"
  //     },
  //     labels: [
  //       "EC JOSE PEARSON TB HOSPITAL", 
  //       "GP HELEN JOSEPH HOSPITAL", 
  //       "GP SIZWE TROPICAL DISEASES HOSPITAL", 
  //       "KZ KING DINUZULU HOSPITAL"
  //     ],
  //     responsive: [
  //       {
  //         breakpoint: 480,
  //         options: {
  //           chart: {
  //             width: 200
  //           },
  //           legend: {
  //             position: "bottom"
  //           }
  //         }
  //       }
  //     ]
  //   };
  // }

  // private genderDistributionChart() {
  //   this.genderDistributionChartOptions = {
  //     series: [67, 60],
  //     chart: {
  //       height: 360,
  //       type: "pie"
  //     },
  //     labels: ["Male", "Female"],
  //     responsive: [
  //       {
  //         breakpoint: 480,
  //         options: {
  //           chart: {
  //             width: 200
  //           },
  //           legend: {
  //             position: "bottom"
  //           }
  //         }
  //       }
  //     ]
  //   };
  // }

  // private labDistributionChart() {
  //   this.labDistributionChartOptions = {
  //     series: [
  //       {
  //         name: "basic",
  //         data: [1, 1, 1, 1, 1, 7, 7, 8, 24, 245, 269, 364, 1209]
  //       }
  //     ],
  //     chart: {
  //       type: "bar",
  //       height: 350
  //     },
  //     plotOptions: {
  //       bar: {
  //         horizontal: true,
  //         dataLabels: {
  //           position: "bottom" // top, center, bottom
  //         }          
  //       }
  //     },
  //     dataLabels: {
  //       enabled: true
  //     },
  //     xaxis: {
  //       categories: [
  //         "Duracao de QRS",
  //         "Albumina",
  //         "Contagem de WBC",
  //         "Depuracao da creatinina",
  //         "Acuidade visual",
  //         "Glucose",
  //         "Glicose no sangue",
  //         "Creatinina serica SCr",
  //         "Hemoglobina",
  //         "AST (SGOT)",
  //         "ALT (SGPT)",
  //         "Contagem do CD4",
  //         "Carga Viral"
  //       ]
  //     }
  //   };    
  // }

  // private conditionDistributionChart() {
  //   this.conditionDistributionChartOptions = {
  //     series: [
  //       {
  //         name: "basic",
  //         data: [77, 22, 13, 7, 3, 2, 2, 2, 2, 2, 2, 1, 1]
  //       }
  //     ],
  //     chart: {
  //       type: "bar",
  //       height: 350
  //     },
  //     plotOptions: {
  //       bar: {
  //         horizontal: true,
  //         dataLabels: {
  //           position: "bottom" // top, center, bottom
  //         }          
  //       }
  //     },
  //     dataLabels: {
  //       enabled: true
  //     },
  //     xaxis: {
  //       categories: [
  //         "Tuberculoses DS (TB)",
  //         "Doenças Cardiovasculares",
  //         "Tuberculose",
  //         "Diabetes",
  //         "MDR - TB",
  //         "Insonia",
  //         "Insuficiência Hepática",
  //         "Insuficiência Renal (aguda ou crónica)",
  //         "Malaria",
  //         "Problemas mentais",
  //         "Nao",
  //         "Neurop perferica",
  //         "Otite Media"
  //       ]
  //     }
  //   };    
  // }

  // private followUpBreakdownChart() {
  //   this.followUpBreakdownChartOptions = {
  //     series: [
  //       {
  //         name: "Enrolments",
  //         type: "column",
  //         data: [26, 58, 4, 39]
  //       },
  //       {
  //         name: "Follow ups",
  //         type: "column",
  //         data: [0, 0, 0, 0]
  //       },
  //       {
  //         name: "Events Reported",
  //         type: "column",
  //         data: [0, 0, 0, 0]
  //       },
  //     ],
  //     chart: {
  //       height: 350,
  //       type: "line",
  //       stacked: false
  //     },
  //     dataLabels: {
  //       enabled: false
  //     },
  //     stroke: {
  //       width: [4, 4, 4]
  //     },
  //     xaxis: {
  //       categories: ['GP SIZWE TROPICAL DISEASES HOSPITAL', 'EC JOSE PEARSON TB HOSPITAL', 'GP HELEN JOSEPH HOSPITAL', 'KZ KING DINUZULU HOSPITAL']
  //     },
  //     yaxis: [
  //       {
  //         axisTicks: {
  //           show: true
  //         },
  //         axisBorder: {
  //           show: true,
  //           color: "#008FFB"
  //         },
  //         title: {
  //           text: "Follow up Visits",
  //           style: {
  //             color: "#008FFB"
  //           }
  //         },
  //         tooltip: {
  //           enabled: true
  //         }
  //       },
  //       {
  //         seriesName: "Income",
  //         opposite: true,
  //         axisTicks: {
  //           show: true
  //         },
  //         axisBorder: {
  //           show: true,
  //           color: "#00E396"
  //         },
  //         title: {
  //           text: "Number of events reported",
  //           style: {
  //             color: "#00E396"
  //           }
  //         }
  //       },
  //       {
  //         seriesName: "Revenue",
  //         opposite: true,
  //         axisTicks: {
  //           show: true
  //         },
  //         axisBorder: {
  //           show: true,
  //           color: "#FEB019"
  //         },
  //         title: {
  //           text: "Number of pregnancy outcomes recorded",
  //           style: {
  //             color: "#FEB019"
  //           }
  //         }
  //       }
  //     ],
  //     tooltip: {
  //       fixed: {
  //         enabled: true,
  //         position: "topLeft", // topRight, topLeft, bottomRight, bottomLeft
  //         offsetY: 30,
  //         offsetX: 60
  //       }
  //     },
  //     legend: {
  //       horizontalAlign: "left",
  //       offsetX: 40
  //     }
  //   };    
  //}

}

class ViewModel {
  cohortList: CohortGroupDetailModel[] = [];
  charts: ChartOptions[] = [];
  selectedCohort: CohortGroupDetailModel;
  selectedDashboard: 'Demographics' | 'Clinical' | 'Facility Management' | 'NPC Management' | 'NPC Signals';
}