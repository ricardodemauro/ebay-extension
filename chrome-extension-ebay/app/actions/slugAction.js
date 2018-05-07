import {
    FETCHED_KEYWORD,
    FETCHING_KEYWORD,
    FETCHED_PRODUCTIES,
    FETHING_PRODUCTIES,
    CLEAR_DATA } from '../actionTypes'

import { beginFetch, endFetch, logFetch, reachTheLimit } from './systemActions'

export const fetchingSlug = () => {
    return {
        type: FETCHING_KEYWORD
    }
}

export const fetchingProducties = () => {
    return {
        type: FETHING_PRODUCTIES
    }
}

export const retrivedSlug = (data) => {
    return {
        type: FETCHED_KEYWORD,
        data
    }
}

export const retrivedDetailedAsset = (data) => {
    return {
        type: FETCHED_PRODUCTIES,
        data
    }
}

export const clearData = () => {
    return {
        type: CLEAR_DATA
    }
}

export const fetchSlug = (name, socket) => dispatch => {
    socket.setCallBack(dispatch(onFetchedSlugData))
    dispatch(beginFetch())
    socket.sendMessage({ operation: 'complete', data: name })
}

const onFetchedSlugData = dispatch => data => {
    if(data.code == 401) {
        dispatch(reachTheLimit(data.times))
    }
    else {
        dispatch(logFetch(JSON.stringify(data, null, 2)))
        dispatch(retrivedDetailedAsset(data.data))
    }
    
    dispatch(endFetch())
}