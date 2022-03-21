// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  apiURL: 'http://localhost:5000/api',
  //apiURL: 'https://localhost:44380/api',
  //apiURL: 'https://pvims-api.doh.gov.ph/api',
  apiURLBase: 'http://localhost:5000',
  //apiURLBase: 'https://pvims-api.doh.gov.ph',
  appVersion: '2.2.2203011405',
  appName: 'PViMS Training',
  installationDate: '2022-03-21',
  countryISOCode: 'mz'
};
