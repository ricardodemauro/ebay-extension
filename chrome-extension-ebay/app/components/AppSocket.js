const socketUri = 'ws://localhost:33427/ws'
//const socketUri = 'ws://rebaysearch.azurewebsites.net/ws'


const getAppSocket = (onOpen, onClose, onMessage, onError) => {
    let socket = new WebSocket(socketUri)
    socket.onclose = onClose
    socket.onopen = onOpen
    socket.onmessage = onMessage
    socket.onerror = onError
    return socket
}

const closeAppSocket = (socket) => {
    if(!socket || socket.readyState != WebSocket.OPEN) {
        console.info('socket not read to close')
    }
    else {
        socket.close(1000, 'Closing from client')
    }
}

const sendMessage = (socket, message) => {
    if(!socket || socket.readyState != WebSocket.OPEN) {
        console.info('socket not read to send messages')
    }
    else {
        const data = message
        socket.send(data)
    }
}

class AppSocket {
    constructor() {
        this.socket = getAppSocket(this.onOpen.bind(this), this.onClose.bind(this), this.onMessageReceive.bind(this), this.onError.bind(this))
    }

    sendMessage(data = {}) {
        if(!this.socket || this.socket.readyState != WebSocket.OPEN) {
            console.info('socket not open')
            return
        }
        const dataString = JSON.stringify(data)
        console.info(`data send ${dataString}`)
        this.socket.send(dataString)
    }

    setCallBack(callback) {
        this.callback = callback
    }

    onOpen(event) {
        console.info(`on socket opened`)
    }

    onClose(event) {
        console.info(`on socket closed`)
    }

    onError(event) {
        console.info(`on socket error`)
    }

    onMessageReceive(data) {
        //console.info(`on data receive ${data.data}`)
        const receivedData = JSON.parse(data.data)
        if(!this.callback) {
            console.info(`no callback registered`)
            return
        }
        this.callback(receivedData)
    }
}

export default AppSocket

