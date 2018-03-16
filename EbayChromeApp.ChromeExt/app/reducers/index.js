import { combineReducers } from 'redux'

import { slugReducer } from './apiReducer'
import { systemReducer } from './systemReducer'

const rootReducer = combineReducers({
    slugReducer,
    systemReducer
})

export default rootReducer