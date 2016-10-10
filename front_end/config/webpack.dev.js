var webpackMerge = require('webpack-merge');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var commonConfig = require('./webpack.common.js');
var webpack = require('webpack');
var helpers = require('./helpers');
const ENV = 'development';

module.exports = webpackMerge(commonConfig, {
  devtool: 'cheap-module-eval-source-map',

  output: {
    path: helpers.root('dist'),
    publicPath: 'http://localhost:8080/',
    filename: '[name].js',
    chunkFilename: '[id].chunk.js'
  },

  plugins: [
    new ExtractTextPlugin('[name].css'),
    // new webpack.DefinePlugin({
    //  'CRDS_API_ENDPOINT': "02938902384",
    //  'TESTYBOB': 'ewrlkewrew',
    //  'process.env': {
    //    'CRDS_API_ENDPOINT': "23423423234",
    //    'TESTYBOB': 'alkjlksdjflksd',
    //    'ENV': 'development',
    //    'NODE_ENV': 'development'
    //    }
    //  })
  ],

  devServer: {
    historyApiFallback: true,
    stats: 'minimal'
  }
});
