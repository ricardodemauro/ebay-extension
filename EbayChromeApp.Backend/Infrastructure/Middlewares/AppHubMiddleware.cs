using EbayChromeApp.Backend.Hubs;
using EbayChromeApp.Backend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using EbayChromeApp.Backend.Options;

namespace EbayChromeApp.Backend.Infrastructure.Middlewares
{
    public class AppHubMiddleware
    {
        private readonly RequestDelegate _next;

        public AppHubMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    var service = context.RequestServices.GetService<IEbayService>();
                    var options = context.RequestServices.GetService<IOptions<EnvorinmentOptions>>();
                    var _hub = new EbayHub(context, webSocket, service, options);
                    await _hub.ReceiveAsync();
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
