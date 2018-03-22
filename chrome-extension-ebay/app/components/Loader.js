import React from 'react'
import Fade from 'material-ui/transitions/Fade'
import { withStyles } from 'material-ui/styles'
import { CircularProgress } from 'material-ui/Progress'
import PropTypes from 'prop-types'

const styles = theme => ({
    root: {
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
    },
    button: {
      margin: theme.spacing.unit * 2,
    },
    placeholder: {
      height: 40,
    },
});

const Loader = (props) => {
    return (
        <div className={props.classes.placeholder}>
          <Fade in={props.loading}
            style={{ transitionDelay: props.loading ? '800ms' : '0ms' }} unmountOnExit>
            <CircularProgress />
          </Fade>
        </div>
    )
}

Loader.propTypes = {
    classes: PropTypes.object.isRequired,
    loading: PropTypes.bool.isRequired
}

export default withStyles(styles)(Loader)