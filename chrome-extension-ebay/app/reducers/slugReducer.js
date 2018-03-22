import { RETRIVED_SLUG, 
    SEARCH_SLUG,
    FETCH_DETAIL,
    FETCHED_DETAIL,
    FETCHING_DETAIL,
    NOT_FETCHED_DETAIL,
    CLEAR_DATA } from '../actionTypes'



export const slugReducer = (state = {}, action) => {
    const { type, slug, data } = action
    const { slugColl, assetColl, fetchingColl, doneAssetColl } = state
    switch(type) {
        case RETRIVED_SLUG:
            const nAssetColl = assetColl !== undefined ? assetColl : []
            const fetchedData = data.map((d, i) => {
                return { asset: d, detailed: false, status: NOT_FETCHED_DETAIL }
            })
            return {
                ...state,
                assetColl: nAssetColl.concat(fetchedData)
            }
        case SEARCH_SLUG: 
            const nSlugColl = slugColl ? slugColl : []
            return {
                ...state,
                slugColl: nSlugColl.concat({ slug: slug })
            }
        case FETCH_DETAIL: 
            const nAssetC = assetColl.map((asset, i) => {
                if(asset.asset == data)
                    return Object.assign({}, asset, { status: FETCHING_DETAIL })
                else
                    return asset
            })
            return {
                ...state,
                assetColl: nAssetC
            }
        case FETCHED_DETAIL:
            const ndoneAssetColl = doneAssetColl !== undefined ? doneAssetColl : []
            
            const nAssetCo = assetColl.map((asset, i) => {
                if(asset.asset == data.name)
                    return Object.assign({}, asset, { status: FETCHED_DETAIL })
                else
                    return asset
            })
            const doneAssetAux = assetColl.filter((asset, i) => {
                return asset.asset == data.name
            })
            const doneAsset = Object.assign({}, doneAssetAux[0], { totalEntries: data.totalEntries, status: FETCHED_DETAIL })
            return {
                ...state,
                doneAssetColl: ndoneAssetColl.concat(doneAsset),
                assetColl: nAssetCo
            }
        case CLEAR_DATA:
            return {
                doneAssetColl: [],
                assetColl: [],
                slugColl: []
            }
        default:
            return {
                ...state
            }
    }
}