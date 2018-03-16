import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { withStyles } from 'material-ui/styles'
import Reboot from 'material-ui/Reboot'
import Paper from 'material-ui/Paper'
import SimpleAppBar from '../components/SimpleAppBar'
import SearchApp from './SearchApp'
import Loader from '../components/Loader'

const styles = theme => ({
    root: theme.mixins.gutters({
        width: 320,
        height: 250,
        display: 'flex',
        alignItems: 'center',
        flexDirection: 'column' 
    }),
    second: theme.mixins.gutters({
        width: 320,
        marginTop: 10
    }),
    content: theme.mixins.gutters({
        width: 320,
        height: 400,
        marginLeft: 0,
        marginRight: 0,
        paddingLeft: 0,
        paddingRight: 0
    }),
    json: {
        whiteSpace: 'pre-wrap'
    }
});

class App extends Component {
    static propTypes = {
        dispatch: PropTypes.func.isRequired,
        isFetching: PropTypes.bool.isRequired,
        status: PropTypes.string,
        json: PropTypes.string,
        error: PropTypes.string,
        date: PropTypes.string,
        hasError: PropTypes.bool.isRequired
    }
    constructor(props) {
        super(props)
        this.state = {}
    }

    render() {
        const { classes, isFetching, error, json, hasError } = this.props
        const statusTxt = isFetching ? 'Fetching' : ''

        return (
            <div>
                <Reboot />
                <SimpleAppBar title="Ebay App" />
                <div className={classes.content}>
                    <Paper className={classes.root}>
                        <Loader loading={isFetching}/>
                        <SearchApp />
                    </Paper>
                    <Paper className={classes.second}>
                        <div>
                            <div>error msg: '{error}'</div>
                        </div>
                        <div>
                            <div className={classes.json}>api data: {json}</div>
                        </div>
                    </Paper>
                    
                </div>
            </div>
        )
    }
}

const mapStateToProps = state => {
    const { systemReducer } = state
    const { isFetching, json, error } = systemReducer

    return {
        isFetching: isFetching !== undefined ? isFetching : false,
        json: json ? json : '',
        error: error ? error.message : '',
        hasError: error !== undefined
    }
}

//export default withStyles(styles)(App)
export default withStyles(styles)(connect(mapStateToProps)(App))