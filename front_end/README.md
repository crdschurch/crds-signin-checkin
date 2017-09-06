# FrontEnd

## Developer Setup

#### Make your editor aware of [TypeScript typings](README_typescript.md)
#### Install Dependencies via [yarn](https://yarnpkg.com/en/docs/migrating-from-npm)

```sh
# install yarn globally if you haven't
npm install -g yarn
yarn install
```

#### Set Environment Variables using [dotenv](https://github.com/bkeepers/dotenv)

  * Create a .env file in the project root with the following values (copy _.env.example_ to _.env_ and fill out values).
  * environment variables [explained](README_env.md)



#### Run locally

```sh
yarn start
```

#### Clean and reinstall dependencies

```sh
yarn run refresh
```


#### Run locally with Hot Module Replacement (not sure if this works)

```sh
yarn run server:dev:hmr

```

#### Run tests

```sh
yarn test
yarn e2e # no e2e yet
```

#### Run linter

```sh
yarn run lint
```

## Deployment

#### Build

```sh
yarn run build
```
