# FrontEnd

## Developer Setup

* Make your editor aware of [TypeScript typings](README_typescript.md)
* Install Dependencies via [yarn](https://yarnpkg.com/en/docs/migrating-from-npm)

```
yarn install
// or you can still npm install
```

* Set Environment Variables using [dotenv](https://github.com/bkeepers/dotenv)

Create a .env file in the project root with the following values (copy .env.example to .env and fill out values)._Tweak hostnames & ports depending on your actual environment setup_

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

#### Run locally

```
npm start
```


#### Run locally with Hot Module Replacement

```
npm run server:dev:hmr

```

#### Run tests

```
npm test
npm e2e (none at the moment)
```

## Deployment

#### Build

```
npm run build
```
