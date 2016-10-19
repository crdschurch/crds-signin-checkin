# FrontEnd

## Developer Setup

#### Make your editor aware of [TypeScript typings](README_typescript.md)
#### Install Dependencies via [yarn](https://yarnpkg.com/en/docs/migrating-from-npm)

```sh
yarn install # or you can still npm install
```

#### Set Environment Variables using [dotenv](https://github.com/bkeepers/dotenv)

  * Create a .env file in the project root with the following values (copy _.env.example_ to _.env_ and fill out values).
  * see an [example](README_env.md)



#### Run locally

```sh
npm start
```


#### Run locally with Hot Module Replacement

```sh
npm run server:dev:hmr

```

#### Run tests

```sh
npm test
npm e2e # no e2e yet
```

## Deployment

#### Build

```sh
npm run build
```
