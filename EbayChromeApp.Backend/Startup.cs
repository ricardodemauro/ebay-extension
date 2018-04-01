#define UseOptions // or NoOptions
using EbayChromeApp.Backend.Hubs;
using EbayChromeApp.Backend.Options;
using EbayChromeApp.Backend.Services;
using EbayChromeApp.Backend.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Net.WebSockets;

namespace EbayChromeApp.Backend

{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.Configure<EnvorinmentOptions>(c =>
            {
                c.RootDirectory = _env.ContentRootPath;
                c.AppDataDirectory = Path.Combine(_env.ContentRootPath, "App_Data");
                c.MaxMinutesInCache = Configuration.GetValue<int>("MaxMinutesInCache");
            });

            services.AddScoped<IStorage, FileStorage>();
            services.AddScoped<IEbayService, CachedEbayService>();

            services.Configure<EbayServiceOptions>(Configuration.GetSection("EbaySerivce"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                string httpsOnly = Configuration["HTTPS"];
                if (!string.IsNullOrEmpty(httpsOnly) && string.Equals(httpsOnly, "true", System.StringComparison.CurrentCultureIgnoreCase))
                {
                    var rOpts = new RewriteOptions()
                    .AddRedirectToHttps();

                    app.UseRewriter(rOpts);
                }
            }

#if NoOptions
            #region UseWebSockets
            app.UseWebSockets();
            #endregion
#endif
#if UseOptions
            #region UseWebSocketsOptions
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            #endregion
#endif
            #region AcceptWebSocket
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                        var ebayService = serviceProvider.GetService<IEbayService>();

                        var _hub = new EbayHub(context, webSocket, ebayService);
                        await _hub.ReceiveAsync();
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }

            });

            app.Use(async (context, next) =>
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("Its working!");

            });
            #endregion
        }
    }
}
