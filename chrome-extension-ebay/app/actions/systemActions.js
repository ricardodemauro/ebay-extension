import { API_REQUEST_BEGIN,
    API_REQUEST_END,
    API_REQUEST_ERROR,
    API_REQUEST_LOG } from '../actionTypes'

export const beginFetch = () => {
    return {
        type: API_REQUEST_BEGIN
    }
}

export const endFetch = () => {
    return {
        type: API_REQUEST_END
    }
}

export const errorFetch = (error) => {
    return {
        type: API_REQUEST_ERROR,
        error
    }
}

export const logFetch = (json) => {
    return {
        type: API_REQUEST_LOG,
        json
    }
}