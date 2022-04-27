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

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  colors: string[];
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  fill: ApexFill;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  legend: ApexLegend;
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
  ) 
  { 
    super(_router, _location, popupService, accountService, eventService);    
  }

  ngOnInit() {
    let self = this;
    self.initCohortEnrollmentCumulativeChart();
    self.followUpCumulativeChart();
    self.genderDistributionChart();
    self.labDistributionChart();
    self.conditionDistributionChart();
    self.followUpBreakdownChart();
  }

  ngAfterViewInit(): void {
    let self = this;
    self.loadCohorts();
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
  
  selectDashboard(cohort: CohortGroupDetailModel): void {
    let self = this;
    self.viewModel.selectedCohort = cohort;
  }  

  private initCohortEnrollmentCumulativeChart() {
    this.initCohortEnrollmentCumulativeChartOptions = {
      series: [
        {
          name: "CS Carmelo",
          data: [294, 294, 294, 300, 300]
        },
        {
          name: "CS Cuamba",
          data: [308,	323, 384,	390, 390]
        },
        {
          name: "CS Machava II",
          data: [354,	416,	417,	417,	417]
        },
        {
          name: "CS Macia",
          data: [130,	181,	196,	220,	224]
        },
        {
          name: "CS Mavalane",
          data: [285,	285,	285,	285,	285]
        },
        {
          name: "CS Namacurra-Sede",
          data: [425,	430,	432,	438,	438]
        },
        {
          name: "CS Ndlavela",
          data: [191,	193,	194,	194,	211]
        },
        {
          name: "HD Gondola",
          data: [378,	495,	556,	556,	562]
        },
        {
          name: "HG Marrere",
          data: [316,	328,	340,	341,	341]
        }
      ],
      chart: {
        type: "bar",
        height: 350
      },
      colors: [
        "#33b2df",
        "#546E7A",
        "#d4526e",
        "#13d8aa",
        "#A5978B",
        "#2b908f",
        "#f9a3a4",
        "#90ee7e",
        "#f48024",
        "#69d2e7"
      ],      
      plotOptions: {
        bar: {
          horizontal: false
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        show: true,
        width: 2,
        colors: ["transparent"]
      },
      xaxis: {
        categories: ['Mar-21', 'Jun-21', 'Sep-21', 'Dec-21', 'Mar-22']
      },
      yaxis: {
        title: {
          text: ""
        }
      },
      fill: {
        opacity: 1
      },
      tooltip: {
        y: {
          formatter: function(val) {
            return val + " reports";
          }
        }
      }
    };
  }

  private followUpCumulativeChart() {
    this.followUpCumulativeChartOptions = {
      series: [
        {
          name: "CS Carmelo",
          data: [324,	447,	490,	593,	688]
        },
        {
          name: "CS Cuamba",
          data: [125,	161,	215,	307,	341]
        },
        {
          name: "CS Machava II",
          data: [79,	290,	290,	290,	290]
        },
        {
          name: "CS Macia",
          data: [88,	106,	107,	276,	471]
        },
        {
          name: "CS Mavalane",
          data: [240,	254,	257,	258,	258]
        },
        {
          name: "CS Namacurra-Sede",
          data: [211,	253,	274,	274,	275]
        },
        {
          name: "CS Ndlavela",
          data: [8,	10,	14,	14,	14]
        },
        {
          name: "HD Gondola",
          data: [623,	793,	793,	794,	845]
        },
        {
          name: "HG Marrere",
          data: [523,	650,	754,	825,	839]
        }
      ],
      chart: {
        type: "bar",
        height: 350,
        stacked: true
      },
      colors: [
        "#33b2df",
        "#546E7A",
        "#d4526e",
        "#13d8aa",
        "#A5978B",
        "#2b908f",
        "#f9a3a4",
        "#90ee7e",
        "#f48024",
        "#69d2e7"
      ],      
      plotOptions: {
        bar: {
          horizontal: true,
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        show: true,
        width: 2,
        colors: ["transparent"]
      },
      xaxis: {
        categories: ['Mar-21', 'Jun-21', 'Sep-21', 'Dec-21', 'Mar-22']
      },
      yaxis: {
        title: {
          text: ""
        }
      },
      fill: {
        opacity: 1
      },
      tooltip: {
        y: {
          formatter: function(val) {
            return val + " reports";
          }
        }
      }
    };
  }

  private genderDistributionChart() {
    this.genderDistributionChartOptions = {
      series: [1012, 2157],
      chart: {
        height: 360,
        type: "pie"
      },
      labels: ["Male", "Female"],
      responsive: [
        {
          breakpoint: 480,
          options: {
            chart: {
              width: 200
            },
            legend: {
              position: "bottom"
            }
          }
        }
      ]
    };
  }

  private labDistributionChart() {
    this.labDistributionChartOptions = {
      series: [
        {
          name: "basic",
          data: [1, 1, 1, 1, 1, 7, 7, 8, 24, 245, 269, 364, 1209]
        }
      ],
      chart: {
        type: "bar",
        height: 350
      },
      plotOptions: {
        bar: {
          horizontal: true,
          dataLabels: {
            position: "bottom" // top, center, bottom
          }          
        }
      },
      dataLabels: {
        enabled: true
      },
      xaxis: {
        categories: [
          "Duracao de QRS",
          "Albumina",
          "Contagem de WBC",
          "Depuracao da creatinina",
          "Acuidade visual",
          "Glucose",
          "Glicose no sangue",
          "Creatinina serica SCr",
          "Hemoglobina",
          "AST (SGOT)",
          "ALT (SGPT)",
          "Contagem do CD4",
          "Carga Viral"
        ]
      }
    };    
  }

  private conditionDistributionChart() {
    this.conditionDistributionChartOptions = {
      series: [
        {
          name: "basic",
          data: [77, 22, 13, 7, 3, 2, 2, 2, 2, 2, 2, 1, 1]
        }
      ],
      chart: {
        type: "bar",
        height: 350
      },
      plotOptions: {
        bar: {
          horizontal: true,
          dataLabels: {
            position: "bottom" // top, center, bottom
          }          
        }
      },
      dataLabels: {
        enabled: true
      },
      xaxis: {
        categories: [
          "Tuberculoses DS (TB)",
          "Doenças Cardiovasculares",
          "Tuberculose",
          "Diabetes",
          "MDR - TB",
          "Insonia",
          "Insuficiência Hepática",
          "Insuficiência Renal (aguda ou crónica)",
          "Malaria",
          "Problemas mentais",
          "Nao",
          "Neurop perferica",
          "Otite Media"
        ]
      }
    };    
  }

  private followUpBreakdownChart() {
    this.followUpBreakdownChartOptions = {
      series: [
        {
          name: "Follow Up Visits",
          type: "column",
          data: [688, 341, 290, 471, 258, 275, 14, 845, 842]
        },
        {
          name: "Events Reported",
          type: "column",
          data: [8, 7, 0, 3, 0, 11, 1, 3, 9]
        },
        {
          name: "Pregnancies Reported",
          type: "column",
          data: [12, 30, 0, 7, 0, 1, 0, 4, 0]
        }
      ],
      chart: {
        height: 350,
        type: "line",
        stacked: false
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        width: [1, 1, 4]
      },
      xaxis: {
        categories: ['CS Carmelo', 'CS Cuamba', 'CS Machava II', 'CS Macia', 'CS Mavalane', 'CS Namacurra-Sede', 'CS Ndlavela', 'HD Gondola', 'HG Marrere']
      },
      yaxis: [
        {
          axisTicks: {
            show: true
          },
          axisBorder: {
            show: true,
            color: "#008FFB"
          },
          title: {
            text: "Follow up Visits",
            style: {
              color: "#008FFB"
            }
          },
          tooltip: {
            enabled: true
          }
        },
        {
          seriesName: "Income",
          opposite: true,
          axisTicks: {
            show: true
          },
          axisBorder: {
            show: true,
            color: "#00E396"
          },
          title: {
            text: "Number of events reported",
            style: {
              color: "#00E396"
            }
          }
        },
        {
          seriesName: "Revenue",
          opposite: true,
          axisTicks: {
            show: true
          },
          axisBorder: {
            show: true,
            color: "#FEB019"
          },
          title: {
            text: "Number of pregnancy outcomes recorded",
            style: {
              color: "#FEB019"
            }
          }
        }
      ],
      tooltip: {
        fixed: {
          enabled: true,
          position: "topLeft", // topRight, topLeft, bottomRight, bottomLeft
          offsetY: 30,
          offsetX: 60
        }
      },
      legend: {
        horizontalAlign: "left",
        offsetX: 40
      }
    };    
  }

}

class ViewModel {
  cohortList: CohortGroupDetailModel[] = [];
  selectedCohort: CohortGroupDetailModel;
}