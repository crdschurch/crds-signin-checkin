var webpack = require('webpack');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var helpers = require('./helpers');
var DotenvPlugin = require('webpack-dotenv-plugin');

module.exports = {
  entry: {
    'polyfills': ['./src/polyfills.ts'],
    'vendor': ['./src/vendor.ts'],
    'app': ['./src/main.ts']
  },

  resolve: {
    extensions: ['', '.js', '.ts']
  },

  module: {
    loaders: [
      {
        test: /\.ts$/,
        loaders: ['awesome-typescript-loader', 'angular2-template-loader']
      },
      {
        test: /\.html$/,
        loader: 'html'
      },
      {
        test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
        loader: 'file?name=assets/[name].[hash].[ext]'
      },
      {
        // test: /\.scss$/,
        // include: helpers.root('src', 'app'),
        // loaders: ['raw-loader', 'sass-loader']
        test: /\.scss$/,
        exclude: /node_modules/,
        loaders: ['raw-loader', 'sass-loader']
      }
    ]
  },

  plugins: [
    new webpack.optimize.CommonsChunkPlugin({
      name: ['app', 'vendor', 'polyfills']
    }),
    new DotenvPlugin({
      sample: './.env.default',
      path: './.env'
    }),
    new HtmlWebpackPlugin({
      template: 'src/index.html'
    })
  ]
};
