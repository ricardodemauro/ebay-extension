const connection = new signalR.HubConnection('/chat');

connection.on('Send', (message) => {
    const encodedMsg = message;
    const listItem = document.createElement('li');
    listItem.innerHTML = encodedMsg;
    document.getElementById('messages').appendChild(listItem);
});

document.getElementById('send').addEventListener('click', event => {
    const msg = document.getElementById('message').value;
    const usr = document.getElementById('user').value;

    connection.invoke('SendMessage', usr, msg).catch(err => showErr(err));
    event.preventDefault();
});

function showErr(msg) {
    const listItem = document.createElement('li');
    listItem.setAttribute('style', 'color: red');
    listItem.innerText = msg.toString();
    document.getElementById('messages').appendChild(listItem);
}

connection.start().catch(err => showErr(err));