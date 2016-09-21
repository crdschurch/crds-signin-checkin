/***********************************************************************************************
 * User Configuration.
 **********************************************************************************************/
/** Map relative paths to URLs. */
const map: any = {
    'ng2-bootstrap': 'vendor/ng2-bootstrap',
    'lodash': 'vendor/lodash',
    'moment': 'vendor/moment/moment.js'
};

/** User packages configuration. */
const packages: any = {
    'vendor/ng2-bootstrap': {
      main: "bundles/ng2-bootstrap.js",
      defaultExtension: 'js'
    },
    // Moment.js
    'moment': {
      format: 'cjs'
    },
    'lodash': {
      format: 'cjs',
      main: 'index.js',
      defaultExtension: "js"
    }
};

////////////////////////////////////////////////////////////////////////////////////////////////
/***********************************************************************************************
 * Everything underneath this line is managed by the CLI.
 **********************************************************************************************/
const barrels: string[] = [
  // Angular specific barrels.
  '@angular/core',
  '@angular/common',
  '@angular/compiler',
  '@angular/http',
  '@angular/router',
  '@angular/forms',
  '@angular/platform-browser',
  '@angular/platform-browser-dynamic',

  // Thirdparty barrels.
  'rxjs',

  // App specific barrels.
  'app',
  'app/shared',
  'app/search',
  'app/+results',
  'app/+assignment',
  'app/search/+noresults',
  'app/+results/+serving-length',
  'app/+guest',
  'app/+room',
  /** @cli-barrel */
];

const cliSystemConfigPackages: any = {};
barrels.forEach((barrelName: string) => {
  cliSystemConfigPackages[barrelName] = { main: 'index' };
});

/** Type declaration for ambient System. */
declare var System: any;

// Apply the CLI SystemJS configuration.
System.config({
  map: {
    '@angular': 'vendor/@angular',
    'rxjs': 'vendor/rxjs',
    'main': 'main.js'
  },
  packages: cliSystemConfigPackages
});

// Apply the user's configuration.
System.config({ map, packages });
