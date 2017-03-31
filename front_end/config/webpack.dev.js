var webpackMerge = require('webpack-merge');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var commonConfig = require('./webpack.common.js');
var webpack = require('webpack');
var helpers = require('./helpers');
const ENV = 'development';

module.exports = webpackMerge(commonConfig, {
  devtool: 'cheap-module-eval-source-map',

  module: {
    preLoaders: [
      {
        test: /\.ts$/,
        loader: 'tslint-loader',
        exclude: [helpers.root('node_modules')]
      }
    ]
  },

  tslint: {
    emitErrors: false,
    failOnHint: false,
    resourcePath: 'src'
  },


  output: {
    path: helpers.root('dist'),
    publicPath: 'http://localhost:8080/',
    filename: '[name].js',
    chunkFilename: '[id].chunk.js'
  },

  plugins: [
    new ExtractTextPlugin('[name].css'),
  ],

  devServer: {
    historyApiFallback: true,
    stats: 'minimal'
  }
});
