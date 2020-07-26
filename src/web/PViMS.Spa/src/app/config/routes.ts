import { _paths } from './paths';

export const _routes = {
  error: {
    _401: `${_paths.error}/${_paths.errorPath._401}`,
    _403: `${_paths.error}/${_paths.errorPath._403}`,
    _404: `${_paths.error}/${_paths.errorPath._404}`,
    _500: `${_paths.error}/${_paths.errorPath._500}`,
    _501: `${_paths.error}/${_paths.errorPath._501}`,
    general: `${_paths.error}/${_paths.errorPath.general}`,
  },
  security: {
      login: `${_paths.security}/${_paths.securityPath.login}`,
      forgotPassword: `${_paths.security}/${_paths.securityPath.forgotPassword}`,
      lockscreen: `${_paths.security}/${_paths.securityPath.lockscreen}`,
      spontaneous: `${_paths.security}/${_paths.securityPath.spontaneous}`,
  },
  clinical: {
      patients: {
          search: `${_paths.clinical}/${_paths.clinicalPath.patients.search}`,
          view(patientId: number): string {
              return `${_paths.clinical}/${_paths.clinicalPath.patients.view.replace(':patientId', patientId.toString())}`;
          }
      },
      encounters: {
          search: `${_paths.clinical}/${_paths.clinicalPath.encounters.search}`,
          view(patientId: number, encounterId: number): string {
              return `${_paths.clinical}/${_paths.clinicalPath.encounters.view.replace(':encounterId', encounterId.toString()).replace(':patientId', patientId.toString())}`;
          }
      },
      cohorts: {
        search: `${_paths.clinical}/${_paths.clinicalPath.cohorts.search}`,
        view(cohortGroupId: number): string {
            return `${_paths.clinical}/${_paths.clinicalPath.cohorts.view.replace(':cohortGroupId', cohortGroupId.toString())}`;
        }
      },      
      forms: {
          list: `${_paths.clinical}/${_paths.clinicalPath.forms.list}`,
          viewFormA(formId: number): string {
              return `${_paths.clinical}/${_paths.clinicalPath.forms.forma.replace(':formId', formId.toString())}`;
          },
          viewFormB(formId: number): string {
              return `${_paths.clinical}/${_paths.clinicalPath.forms.formb.replace(':formId', formId.toString())}`;
          },
          viewFormC(formId: number): string {
              return `${_paths.clinical}/${_paths.clinicalPath.forms.formc.replace(':formId', formId.toString())}`;
          }
      }
    },
    analytical: {
      landing: `${_paths.analytical}/${_paths.analyticalPath.landing}`,
      reports: {
        search(workFlowId: string): string {
          return `${_paths.analytical}/${_paths.analyticalPath.reports.search.replace(':wuid', workFlowId)}`;
        },
        activity(workFlowId: string, reportInstanceId: number): string {
          return `${_paths.analytical}/${_paths.analyticalPath.reports.activity.replace(':wuid', workFlowId).replace(':reportinstanceid', reportInstanceId.toString())}`;
        }
      }
    },
    reports: {
      patienttreatment: `${_paths.reports}/${_paths.reportPath.patienttreatment}`
    },
    information: {
      home(id: number): string {
          return `${_paths.information}/${_paths.informationPath.home.replace(':id', id.toString())}`;
      }
    },
    administration: {
      landing: `${_paths.administration}/${_paths.administrationPath.landing}`,
      work: {
        datasetcategoryView(datasetId: number): string {
            return `${_paths.administration}/${_paths.administrationPath.work.datasetcategory.replace(':datasetid', datasetId.toString())}`;
        }
      }
    }
};