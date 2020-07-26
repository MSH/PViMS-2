export const _paths = {
  error: `error`,
  errorPath: {
      _401: `401`,
      _403: `403`,
      _404: `404`,
      _500: `500`,
      _501: `501`,
      general: `general`,
  },
  security: `security`,
  securityPath: {
      login: `login`,
      forgotPassword: `forgot-password`,
      lockscreen: `lockscreen`,
      spontaneous: `spontaneous`,
  },
  clinical: `clinical`,
  clinicalPath: {
      patients: {
          search: `patientsearch`,
          view: `patientview/:patientId`
      },
      encounters: {
          search: `encountersearch`,
          view: `encounterview/:patientId/:encounterId`
      },
      cohorts: {
          search: `cohortsearch`,
          view: `cohortenrolment/:cohortGroupId`
      },
      forms: {
          list: `formlist`,
          forma: `forma/:formId`,
          formb: `formb/:formId`,
          formc: `formc/:formId`
      }
  },
  analytical: `analytical`,
  analyticalPath: {
    landing: `landing`,
    reports: {
      search: `reportsearch/:wuid`,
      activity: `activityhistory/:wuid/:reportinstanceid`
    }
  },
  reports: `reports`,
  reportPath: {
    patienttreatment: `system/patienttreatment`
  },
  information: `information`,
  informationPath: {
    home: `pageviewer/:id`
  },
  administration: `administration`,
  administrationPath: {
    landing: `landing`,
    work: {
        datasetcategory: `work/datasetcategory/:datasetid`
    }
  }
};
