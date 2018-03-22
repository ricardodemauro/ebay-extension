const path = require('path')
const webpack = require('webpack')
const CleanWebpackPlugin = require('clean-webpack-plugin')
const CopyWebpackPlugin = require('copy-webpack-plugin')
const copyOptions = { debug: 'info' }

module.exports = {
  entry: ['babel-polyfill', path.join(__dirname, 'app', 'index.js')],
  output: {
    filename: 'bundle.js',
    path: path.resolve(__dirname, 'dist/app')
  },
  module: {
    rules: [{
      test: /.jsx?$/,
      include: [
        path.resolve(__dirname, 'app')
      ],
      exclude: [
        path.resolve(__dirname, 'node_modules'),
        path.resolve(__dirname, 'bower_components')
      ],
      loader: 'babel-loader'
    }]
  },
  plugins: [
    new webpack.DefinePlugin({
      'process.env.NODE_ENV': JSON.stringify('production')
    }),
    new webpack.EnvironmentPlugin(['NODE_ENV']),
    new CleanWebpackPlugin(['app'], {
      root: path.resolve(__dirname, 'dist'),
      verbrose: true,
    }),
    new webpack.optimize.UglifyJsPlugin(),
    new CopyWebpackPlugin([
      { from: 'app/*.html', to: '../' },
      { from: 'images/*.*', to: '../app' },
      { from: 'app/*.json', to: '../' }
    ], copyOptions)
  ],
  resolve: {
    extensions: ['.json', '.js', '.jsx', '.css']
  },
  devtool: 'source-map',
  devServer: {
    contentBase: path.join(__dirname, '/dist/app/'),
    compress: true,
    port: 9000,
    host: '0.0.0.0',
  }
};