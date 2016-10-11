// Karma configuration file, see link for more information
// https://karma-runner.github.io/0.13/config/configuration-file.html

module.exports = function (config) {
  config.set({
    basePath: './src',
    frameworks: ['jasmine'],
    plugins: [
      require('karma-jasmine'),
      require('karma-phantomjs-launcher'),
      require('karma-teamcity-reporter'),
      require('karma-remap-istanbul'),
      require('karma-mocha-reporter'),
      require('karma-webpack'),
      require('karma-sourcemap-loader')
    ],
    files: [
      { pattern: './test.js', watched: false }
    ],
    preprocessors: {
      './karma-test-shim.js': ['webpack', 'sourcemap']
    },
    remapIstanbulReporter: {
      reports: {
        html: 'coverage',
        lcovonly: './coverage/coverage.lcov'
      }
    },
    // reporter(s) to use - set to 'teamcity' for ci server
    reporters: ['mocha'],
    port: 9876,
    // enable / disable colors in the output (reporters and logs)
    colors: true,
    // level of logging
    // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
    logLevel: config.LOG_INFO,
    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: true,
    browsers: ['PhantomJS'],
    // Continuous Integration mode
    // if true, Karma captures browsers, runs the tests and exits; set to true for ci server
    singleRun: false
  });
};
