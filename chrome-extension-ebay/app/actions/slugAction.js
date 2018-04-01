import {
    FETCHED_KEYWORD,
    FETCHING_KEYWORD,
    FETCHED_PRODUCTIES,
    FETHING_PRODUCTIES,
    CLEAR_DATA } from '../actionTypes'

import {
    getStorage,
    setStorage
} from '../components/AppStorage'

import { beginFetch, endFetch, errorFetch, logFetch, addApiRequestCall, reachTheLimit } from './systemActions'

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

const getLastDate = (config) => {
    
    if(config.times >= 5) {
        let firstDate = 0
        let mostOld = Date.now()
        for(let i = 0; i < config.dates.length; i++) {
            const dDate = Date.parse(config.dates[i].date)
            if(dDate < mostOld) {
                firstDate = i
            }
        }
        return { found: true, date: config.dates[firstDate], index: firstDate }
    }
    return { found: false }
}

export const fetchSlug = (name, socket) => dispatch => {
    console.info(`begin fetch slug ${name}`)

    getStorage('TIMES')
        .then(data => {
            let config = { times: 0, dates: []}
            if(data !== undefined && data != null && typeof data === 'string') {
                config = JSON.parse(data)
            }
            else {
                const dataValue = JSON.parse(data.TIMES)
                config = Object.assign({}, config, dataValue)
            }
            let shouldContinue = false

            const now = new Date()

            const day = now.getDay()
            const month = now.getMonth()
            const year = now.getFullYear()

            let filteredDate = config.dates.filter(value => {
                const itemDate = new Date(value)
                return itemDate.getDay() == day && itemDate.getFullYear() == year && itemDate.getMonth() == month
            })

            shouldContinue = filteredDate.length < 5
            config.dates = filteredDate
            config.dates.push(now.toISOString())

            dispatch(addApiRequestCall(config.dates.length))

            if(shouldContinue) {
                const strData = JSON.stringify(config)
                setStorage({ TIMES: strData })
                    .then(() => {
                        socket.setCallBack(dispatch(onFetchedSlugData))
                        dispatch(beginFetch())
                        socket.sendMessage({ operation: 'complete', data: name })
                    })
            }
            else {
                dispatch(reachTheLimit(config.dates.length - 1))
            }
        })
        .catch((reason) => {
            console.info(`something went wront ${reason}`)
        })
}

const onFetchedSlugData = dispatch => data => {
    dispatch(logFetch(JSON.stringify(data, null, 2)))
    dispatch(retrivedDetailedAsset(data.data))
    dispatch(endFetch())
}