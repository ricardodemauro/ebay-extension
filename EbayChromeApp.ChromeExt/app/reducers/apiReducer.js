import { FETCHED_KEYWORD,
    FETCHED_PRODUCTIES,
    FETCHING_KEYWORD,
    FETHING_PRODUCTIES,
    CLEAR_DATA } from '../actionTypes'

export const slugReducer = (state = {}, action) => {
    const { type, data } = action
    const { slugColl, assetColl } = state
    switch(type) {
        case FETCHED_KEYWORD:
            return {
                ...state,
                slugColl: data,
                status: FETCHED_KEYWORD
            }
        case FETCHING_KEYWORD: {
            return {
                ...state,
                status: FETCHING_KEYWORD
            }
        }
        case FETHING_PRODUCTIES: {
            return {
                ...state,
                status: FETHING_PRODUCTIES
            }
        }
        case FETCHED_PRODUCTIES:
            const nassetColl = assetColl !== undefined ? assetColl : []
            const fassetColl = [...nassetColl, ...data]
            return {
                ...state,
                assetColl: fassetColl,
                status: FETCHED_PRODUCTIES
            }
        
        case CLEAR_DATA:
            return {
                slugColl: [],
                assetColl: []
            }
        default:
            return state
    }
}