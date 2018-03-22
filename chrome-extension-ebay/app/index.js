import React from 'react'
import { render } from 'react-dom'
import App from './containers/app'

import { createStore, combineReducers, applyMiddleware } from 'redux'
import { Provider } from 'react-redux'
import { createLogger } from 'redux-logger'
import thunkMiddleware from 'redux-thunk'

import rootReducer from './reducers/index'
import AppSocket from './components/AppSocket'

const appSoc = new AppSocket()

const store = createStore(
  rootReducer,
  applyMiddleware(thunkMiddleware, createLogger())
)

document.addEventListener('DOMContentLoaded', () => {
  render(
    <Provider store={store}>
      <App socket={appSoc} />
    </Provider>, 
    document.getElementById('main'))
});
