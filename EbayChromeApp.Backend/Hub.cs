using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend
{
    public class Hub<TIn, TOut>
        where TIn : Message, new()
        where TOut : Message, new()
    {
        protected byte[] Buffer { get; set; } = new byte[1024 * 4];

        private readonly HttpContext _context;

        private readonly WebSocket _socket;

        private readonly Func<TIn, CancellationToken, Task<TOut>> _action;

        public Hub(HttpContext context, WebSocket webSocket)
            : this(context, webSocket, Hub<TIn, TOut>.Echo)
        {

        }

        public Hub(HttpContext context, WebSocket webSocket, Func<TIn, CancellationToken, Task<TOut>> action)
        {
            _context = context;
            _socket = webSocket;
            _action = action;
        }

        public Task ReceiveAsync()
        {
            return ReceiveAsync(CancellationToken.None);
        }

        protected async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            WebSocketReceiveResult result = await _socket.ReceiveAsync(new ArraySegment<byte>(Buffer), cancellationToken);
            while (!result.CloseStatus.HasValue)
            {
                string message = Encoding.Default.GetString(Buffer);
                TIn data = JsonConvert.DeserializeObject<TIn>(message);
                TOut dataResult = await _action(data, cancellationToken);
                string messageOut = JsonConvert.SerializeObject(dataResult);

                Buffer = Encoding.UTF8.GetBytes(messageOut);

                await _socket.SendAsync(new ArraySegment<byte>(Buffer, 0, Buffer.Length), result.MessageType, result.EndOfMessage, cancellationToken);

                result = await _socket.ReceiveAsync(new ArraySegment<byte>(Buffer), cancellationToken);
            }
            await CloseAsync(result, cancellationToken);
        }

        public static Task<TOut> Echo(TIn message, CancellationToken cancellationToken)
        {
            if (message is Message)
            {
                Message msgString = message;
                msgString.Data = msgString.Data + " hello";

                return Task.FromResult(msgString as TOut);
            }
            return Task.FromResult(message as TOut);
        }

        protected Task CloseAsync(WebSocketReceiveResult result, CancellationToken cancellationToken)
        {
            return _socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, cancellationToken);
        }

        public virtual async Task<WebSocketReceiveResult> ReceiveMessageAsync<T>(T message, CancellationToken cancellationToken)
        {
            var result = await _socket.ReceiveAsync(new ArraySegment<byte>(Buffer), cancellationToken);
            return result;
        }
    }
}
