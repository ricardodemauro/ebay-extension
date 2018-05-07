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
using EbayChromeApp.Backend.Infrastructure.Middlewares;

namespace EbayChromeApp.Backend.Infrastructure
{
    public static class AppHubMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppHub(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AppHubMiddleware>();
        }
    }
}
