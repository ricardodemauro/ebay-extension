import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { withStyles } from 'material-ui/styles'
import Reboot from 'material-ui/Reboot'
import Paper from 'material-ui/Paper'
import SimpleAppBar from '../components/SimpleAppBar'
import SearchApp from './SearchApp'
import Loader from '../components/Loader'
import { InputLabel } from 'material-ui/Input'

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
        height: 250,
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
        reqToday: PropTypes.number.isRequired,
        reachedLimit: PropTypes.bool.isRequired,
        hasError: PropTypes.bool.isRequired,
        socket: PropTypes.object.isRequired
    }
    constructor(props) {
        super(props)
        this.state = {}
    }

    render() {
        const { classes, isFetching, error, json, hasError, socket, reqToday, reachedLimit } = this.props
        const statusTxt = isFetching ? 'Fetching' : ''
        const labelVisisble = reachedLimit ? { display: 'block' } : { display: 'none' }

        return (
            <div>
                <Reboot />
                <SimpleAppBar title="Ebay App" />
                <div className={classes.content}>
                    <Paper className={classes.root}>
                        <Loader loading={isFetching}/>
                        <SearchApp socket={socket} />
                        <div style={labelVisisble}>
                            <InputLabel>You reached the limit of today ({reqToday} requests).</InputLabel>
                        </div>
                        <div style={labelVisisble}>
                            <InputLabel>Try again tomorrow.</InputLabel>
                        </div>
                        
                    </Paper>
                </div>
            </div>
        )
    }
}

const mapStateToProps = state => {
    const { systemReducer } = state
    const { isFetching, json, error, times, reachedLimit } = systemReducer

    return {
        isFetching: isFetching !== undefined ? isFetching : false,
        json: json ? json : '',
        error: error ? error.message : '',
        hasError: error !== undefined,
        reqToday: times !== undefined ? times : 0,
        reachedLimit: reachedLimit !== undefined ? reachedLimit : false
    }
}

//export default withStyles(styles)(App)
export default withStyles(styles)(connect(mapStateToProps)(App))