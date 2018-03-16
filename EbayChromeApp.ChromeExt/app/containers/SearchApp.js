import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { fetchSlug, fetchDetailAsset, clearData } from '../actions/slugAction'

import { withStyles } from 'material-ui/styles'
import Button from 'material-ui/Button'
import { FormControl, FormHelperText } from 'material-ui/Form'
import Input, { InputLabel } from 'material-ui/Input'
import { FETCHED_KEYWORD } from '../actionTypes'

const styles = theme => ({
    container: {
      display: 'flex',
      flexWrap: 'wrap',
    },
    button: {
        margin: theme.spacing.unit,
    },
    formControl: {
      margin: theme.spacing.unit,
    },
});

class SearchApp extends Component {
    static propTypes = {
        dispatch: PropTypes.func.isRequired,
        slugColl: PropTypes.array.isRequired,
        assetColl: PropTypes.array.isRequired
    }
    constructor(props) {
        super(props)
        this.state = { keyword: '' }
    }

    

    onSearchClick() {
        const { keyword } = this.state
        const { dispatch } = this.props

        console.info(`on search click ${keyword}`) 

        dispatch(clearData())
        dispatch(fetchSlug(keyword))
        this.setState({ keyword: keyword })
    }

    onChangeTextBox(event) {
        this.setState( { keyword: event.target.value })
    }

    onDownload() {
        console.info('Download activated')
        const { assetColl } = this.props

        let rows = [['name', 'total']]
        
        for(let i = 0; i < assetColl.length; i++) {
            const asset = assetColl[i]
            if(asset.name) {
                rows.push([asset.name, asset.totalEntries])
            }
        }

        let csvContent = "";
        rows.forEach(function(rowArray){
            let row = rowArray.join(",");
            csvContent += row + "\r\n";
         }); 
        
        const encodedUri = csvContent;
        //let link = document.createElement("a");
        //link.setAttribute("href", encodedUri);
        //link.setAttribute("download", "data.csv");
        //link.setAttribute('id', 'tmpLink');
        //document.body.appendChild(link); // Required for FF
        //link.click(); // This will download the data file named "my_data.csv".
        //chrome.tabs.create({url: encodedUri})
        const doc = URL.createObjectURL( new Blob([encodedUri], {type: 'application/octet-binary'}) );
        chrome.downloads.download({ url: doc, filename: 'data.csv', conflictAction: 'overwrite', saveAs: true })
    }

    render() {
        const { classes, isFetching, assetColl } = this.props
        const downloadAvailable = assetColl.length > 0 && !isFetching

        const { keyword } = this.state
        return (
            <div>
                <div className={classes.container}>
                    <FormControl className={classes.formControl}>
                        <InputLabel htmlFor="keyword">keyword</InputLabel>
                        <Input id="keyword" disabled={isFetching} value={keyword} onChange={this.onChangeTextBox.bind(this)} />
                    </FormControl>
                    
                    <Button className={classes.button} disabled={isFetching} variant="raised" size="small" color="primary" onClick={this.onSearchClick.bind(this)}>Search</Button>
                    <Button className={classes.button} disabled={!downloadAvailable} variant="raised" size="small" onClick={this.onDownload.bind(this)}>Download</Button>
                    
                </div>
            </div>
        )
    }
}

const mapStateToProps = state => {
    const { slugReducer, systemReducer } = state
    const { slugColl, assetColl, status } = slugReducer
    const { isFetching } = systemReducer
    return {
        slugColl: slugColl !== undefined ? slugColl : [],
        isFetching: isFetching !== undefined ? isFetching : false,
        assetColl: assetColl !== undefined ? assetColl : [],
        status: status !== undefined ? status : ''
    }
}

export default withStyles(styles)(connect(mapStateToProps)(SearchApp))