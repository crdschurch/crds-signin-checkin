# FrontEnd

## Development

## Developer Setup
* Make your editor aware of [TypeScript typings](README_typescript.md)

### Install Dependencies via [yarn](https://yarnpkg.com/en/docs/migrating-from-npm)
```
yarn install
// or you can still npm install
```

### Set Environment Variables using [dotenv](https://github.com/bkeepers/dotenv)

Create a .env file in the project root with the following values.

_TODO: Tweak hostnames & ports depending on your actual environment setup_
```

# To access local development API
ECHECK_API_ENDPOINT=http://localhost:49390/api
# To access integration API
# ECHECK_API_ENDPOINT=https://echeck-int.crossroads.net/proxy/SignInCheckIn/api

# To access local development CMS
CRDS_CMS_ENDPOINT=http://localhost:81/
# To access integration CMS
# CRDS_CMS_ENDPOINT=https://contentint.crossroads.net/

# Domain-locked API key - this will vary based on what ECHECK_API_ENDPOINT (local or int) is being used
ECHECK_API_TOKEN=[get appropriate value from MinistryPlatform "Client API Keys" table]
```

You can update this file rather than setting environment variables through your OS or CLI (.env file will not be checked in to git)

### Run locally

```
npm start
```

and visit [http://localhost:8080/](http://localhost:8080/)


### Run locally with Hot Module Replacement

```
npm run server:dev:hmr

```

and visit [http://localhost:8080/](http://localhost:8080/)

### Run tests

```
npm test
npm e2e
```

## Deployment

### Build

(TODO: other environments needed?)
```
npm run build
```
