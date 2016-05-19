## crds_signin_checkin Angular 2 Front End code

## Prerequisites

We are using Angular-CLI for scaffolding and generating project resources.

The project has dependencies that require **Node 4 or greater**.

## Basics of Angualr-CLI Installation and usage

**BEFORE YOU INSTALL:** please read the [prerequisites](#prerequisites)
```bash
npm install -g angular-cli
ng --help

ng serve
```
Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.
You can configure the default HTTP port and the one used by the LiveReload server with two command-line options :

```bash
ng serve --port 4201 --live-reload-port 49153
```

You can use the `ng generate` (or just `ng g`) command to generate Angular components:

```
You can find all possible blueprints in the table below:

Scaffold  | Usage
---       | ---
Component | `ng g component my-new-component`
Directive | `ng g directive my-new-directive`
Pipe      | `ng g pipe my-new-pipe`
Service   | `ng g service my-new-service`

You can generate a new route by with the following command (note the singular
used in `hero`):

```bash
ng generate route hero
```

This will create a folder which will contain the hero component and related test and style files.

The generated route will also be registered with the parent component's `@RouteConfig` decorator. 

By default the route will be designated as a **lazy** route which means that it will be loaded into the browser when needed, not upfront as part of a bundle.

In order to visually distinguish lazy routes from other routes the folder for the route will be prefixed with a `+` per the above example the folder will be named `+hero`.
This is done in accordance with the [style guide](https://angular.io/styleguide#!#prefix-lazy-loaded-folders-with-).

The default lazy nature of routes can be turned off via the lazy flag (`--lazy false`)

There is an optional flag for `skip-router-generation` which will not add the route to the parent component's `@RouteConfig` decorator.

### Creating a build

```bash
ng build
```

The build artifacts will be stored in the `dist/` directory.

### Environments

At build time, the `src/client/app/environment.ts` will be replaced by either
`config/environment.dev.ts` or `config/environment.prod.ts`, depending on the
current cli environment.

Environment defaults to `dev`, but you can generate a production build via
the `-prod` flag in either `ng build -prod` or `ng serve -prod`.

### Running unit tests

```bash
ng test
```

Tests will execute after a build is executed via [Karma](http://karma-runner.github.io/0.13/index.html)

If run with the watch argument `--watch` (shorthand `-w`) builds will run when source files have changed
and tests will run after each successful build
