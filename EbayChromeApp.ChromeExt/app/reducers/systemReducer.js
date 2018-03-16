import { API_REQUEST_BEGIN, API_REQUEST_END, API_REQUEST_ERROR, API_REQUEST_LOG } from '../actionTypes'

const api = (state = {}, action) => {
    const { deep } = state
    const { json, error } = action
    switch (action.type) {
        case API_REQUEST_BEGIN:
            const nDeepBegin = deep === undefined ? 1 : deep + 1
            return {
                ...state,
                deep: nDeepBegin,
                isFetching: true,
            }
        case API_REQUEST_END:
            const nDeep = deep - 1
            return {
                ...state,
                isFetching: nDeep > 0,
                deep: nDeep
            }
        case API_REQUEST_ERROR:
            return {
                ...state,
                isFetching: false,
                json: '',
                error
            }
        case API_REQUEST_LOG:
            const d = new Date()
            const options = { hour: 'numeric', minute: 'numeric', second: 'numeric' };
            const strDate = new Intl.DateTimeFormat('en-US', options).format(d)
            return {
                ...state,
                json,
                date: strDate,
                error: ''
            }
        default:
            return state
    }
}

export const systemReducer = (state = {}, action) => {
    return api(state, action)
}