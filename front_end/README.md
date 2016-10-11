# FrontEnd

## Development

#### Install Dependencies

```
npm install
```

#### Set Environment Variables using [dotenv](https://github.com/bkeepers/dotenv)

Create a .env file in the front_end root with the following values:
```
(TODO: Update with actual default localhost values)
ECHECK_API_TOKEN=dev-42345454
ECHECK_API_TOKEN=http://localhost-api:53454
CRDS_CMS_ENDPOINT=http://localhost-cms:29348
```

You can update this file rather than setting environment variables through your OS or CLI (.env file will not be checked in to git)

#### Run locally

```
npm start
```

and visit [http://localhost:8080/](http://localhost:8080/)

#### Run tests

```
npm test
npm e2e
```

## Deployment

#### Build

```
npm run build
```
