import { Routes } from '@angular/router';
import { SpontaneousAnalyserComponent } from './analyser/spontaneous-analyser/spontaneous-analyser.component';
import { ActivityhistoryComponent } from './report/activity-history/activity-history.component';
import { ReportSearchComponent } from './report/report-search/report-search.component';
import { LandingComponent } from './landing/landing.component';
import { ActiveAnalyserComponent } from './analyser/active-analyser/active-analyser.component';

export const AnalyticalRoutes: Routes = [
  {
    path: '',
    children: [
    {
        path: 'landing',
        component: LandingComponent,
        data: { title: 'Landing', breadcrumb: 'Landing' }
    },
    {
      path: 'activeanalyser',
      component: ActiveAnalyserComponent,
      data: { title: 'Active Reporting Analyser', breadcrumb: 'Analyser' }
    },
    {
      path: 'spontaneousanalyser',
      component: SpontaneousAnalyserComponent,
      data: { title: 'Spontaneous Reporting Analyser', breadcrumb: 'Analyser' }
    },
    {
      path: 'reportsearch/:wuid',
      component: ReportSearchComponent,
      data: { title: 'Search For a Report', breadcrumb: 'Reports' },
      runGuardsAndResolvers: 'always'
    },
    {
      path: 'activityhistory/:wuid/:id',
      component: ActivityhistoryComponent,
      data: { title: 'Report Activities', breadcrumb: 'Reports' }
    }]
  }
];
