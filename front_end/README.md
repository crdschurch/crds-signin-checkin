# FrontEnd

## Development

#### Install Dependencies

`npm install`

#### Set Environment Variables using [dotenv](https://github.com/bkeepers/dotenv)

Create a .env file in the project root with the following values:
```
(TODO: Update with actual default localhost values)
API_ENDPOINT=http://localhost-api:53454
CMS_ENDPOINT=http://localhost-cms:29348
API_TOKEN=dev-42345454
```

You can update this file rather than setting environment variables through your OS or CLI (.env file will not be checked in to git)

#### Run locally

`npm start`

and visit [http://localhost:8080/]

#### Run tests

```
npm test
npm e2e
```

## Deployment

#### Build for Production

(TODO: other environments needed?)
`npm run build`
