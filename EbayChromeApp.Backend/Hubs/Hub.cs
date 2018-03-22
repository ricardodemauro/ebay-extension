using EbayChromeApp.Backend.Messages;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EbayChromeApp.Backend.Hubs
{
    public class Hub<TIn, TOut>
        where TIn : Message, new()
        where TOut : Message, new()
    {
        protected byte[] Buffer { get; set; } = new byte[1024 * 4];

        private readonly HttpContext _context;

        private readonly WebSocket _socket;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public Hub(HttpContext context, WebSocket webSocket)
        {
            _context = context;
            _socket = webSocket;
            _jsonSerializerSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
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
                int index = message.Length;
                int deep = 0;
                for (int i = 0; i < message.Length; i++)
                {
                    char @char = message[i];
                    if (@char == '{')
                    {
                        deep++;
                    }
                    else if (@char == '}')
                    {
                        deep--;

                        if (deep == 0)
                        {
                            index = i + 1;
                            break;
                        }
                    }
                }
                message = message.Substring(0, index);
                string messageOut = string.Empty;
                try
                {
                    TIn data = JsonConvert.DeserializeObject<TIn>(message);
                    TOut dataResult = await OnGetMessage(data, cancellationToken);
                    messageOut = JsonConvert.SerializeObject(dataResult, _jsonSerializerSettings);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);

                    var errorMsg = new ErrorMessage(ex);
                    messageOut = JsonConvert.SerializeObject(errorMsg, _jsonSerializerSettings);
                }

                Buffer = Encoding.UTF8.GetBytes(messageOut);

                await _socket.SendAsync(new ArraySegment<byte>(Buffer, 0, Buffer.Length), result.MessageType, result.EndOfMessage, cancellationToken);

                result = await _socket.ReceiveAsync(new ArraySegment<byte>(Buffer), cancellationToken);
            }
            await CloseAsync(result, cancellationToken);
        }

        public virtual Task<TOut> OnGetMessage(TIn message, CancellationToken cancellationToken)
        {
            if (message is Message<string>)
            {
                Message<string> msgString = message as Message<string>;
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
