export const getStorage = (key) => {
    //return getFromLocalStorage(key)
    return getFromChromeStorage(key)
}

export const setStorage = (data) => {
    //return setLocalStorage(data)
    return setChromeStorage(data)
}

const setLocalStorage = (data) => {
    return new Promise((resolve, reject) => {
        if(window['localStorage']) {
            let storage = window.localStorage
            for(let p in data) {
                storage.setItem(p, data[p])
            }
            
            resolve()
        }
        else {
            reject('storage not available')
        }
    })
}

const setChromeStorage = (data) => {
    return new Promise((resolve, reject) => {
        chrome.storage.local.set(data, resolve)
    })
}

const getFromLocalStorage = (key, callback) => {
    return new Promise((resolve, reject) => {
        if(window['localStorage']) {
            let storage = window.localStorage
            let item = storage.getItem(key)
            resolve(item)
        }
        else {
            reject('storage not available')
        }
    })
}

const getFromChromeStorage = (key, callback) => {
    return new Promise((resolve, reject) => {
        chrome.storage.local.get(key, resolve)
    })
}