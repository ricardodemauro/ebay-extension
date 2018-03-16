import {
    FETCHED_KEYWORD,
    FETCHING_KEYWORD,
    FETCHED_PRODUCTIES,
    FETHING_PRODUCTIES,
    CLEAR_DATA } from '../actionTypes'
import { beginFetch, endFetch, errorFetch, logFetch } from './systemActions'
const API_CODE = 'Eug0rYa1bHQMMmoOAQ1mKZhWdwVxnk/NuBmDWszN0aaH63OhNdmwpg=='
const API_PRD_CODE = 'nMKps7SU90VN16pg4KmGyZ77IwXtlIUR06w1guRYkskLRhUp0Ebhqg=='
export const getBaseUri = () => {
    return window.location.protocol + '\\\\' + window.location.host + '\\'
}

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

export const fetchDetailAsset = slugCollection => dispatch => {
    if(slugCollection.length > 0) {
        let productColl = []
        for(let i = 0; i < slugCollection.length; i++) {
            const slug = slugCollection[i]
            for(let j = 0; j < slug.body.length; j++) {
                const prd = slug.body[j]
                productColl.push(prd)
            }
        }
        fetchFixedSize(productColl, dispatch)
    }
}

async function fetchFixedSize (productColl, dispatch) {
    for(let k = 0; k < productColl.length; k++) {
        console.info(`fetching length ${productColl.length} index ${k}`)
        dispatch(beginFetch())
        //dispatch(fetchingProducties())
        try {
            const response = await fetchProducties2([productColl[k]])
            const json = await response.json()
            dispatch(logFetch(JSON.stringify(json, null, 2)))
            dispatch(retrivedDetailedAsset(json))
        }
        catch(err) {
            dispatch(errorFetch(err))
        }
        dispatch(endFetch())
    }

    console.info('exiting fetchfixedsize')

}

export const fetchProducties2 = productColl => {
    const uri = `https://ebayappextension.azurewebsites.net/api/productjs?code=${API_PRD_CODE}`
    const payload = { producties: productColl }
    let data = new FormData()
    data.append('json', JSON.stringify(payload))
    const opts = {
        method: 'POST',
        headers: { 
            'Access-Control-Allow-Origin': '*', 
            'Content-Type': 'application/json' 
        },
        mode: 'cors',
        cache: 'default',
        body: JSON.stringify(payload)
    } 
    return fetch(uri, opts)
}

export const fetchProducties = productColl => dispatch => {
    const uri = `https://ebayappextension.azurewebsites.net/api/productjs?code=${API_PRD_CODE}`
    const payload = { producties: productColl }
    let data = new FormData()
    data.append('json', JSON.stringify(payload))
    const opts = {
        method: 'POST',
        headers: { 
            'Access-Control-Allow-Origin': '*', 
            'Content-Type': 'application/json' 
        },
        mode: 'cors',
        cache: 'default',
        body: JSON.stringify(payload)
    } 
    dispatch(beginFetch())
    dispatch(fetchingProducties())
    return fetch(uri, opts)
        .then(response => response.json())
        .then(json => {
            dispatch(logFetch(JSON.stringify(json, null, 2)))
            dispatch(retrivedDetailedAsset(json))
            dispatch(endFetch())
        })
        .catch(err => {
            dispatch(errorFetch(err))
            dispatch(endFetch())
        })
}

export const fetchSlug = name => dispatch => {
    const uri = `https://ebayappextension.azurewebsites.net/api/slugjs?code=${API_CODE}&name=${name}`
    
    const opts = {
        method: 'GET',
        headers: { 'Access-Control-Allow-Origin': '*' },
        mode: 'cors',
        cache: 'default'
    }
    dispatch(beginFetch())
    dispatch(fetchingSlug())
    return fetch(uri, opts)
        .then(response => response.json())
        .then(json => {
            dispatch(logFetch(JSON.stringify(json, null, 2)))
            dispatch(retrivedSlug(json))
            dispatch(endFetch())
            dispatch(fetchDetailAsset(json))
        })
        .catch(err => {
            dispatch(errorFetch(err))
            dispatch(endFetch())
        })
}

