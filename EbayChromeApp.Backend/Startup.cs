using EbayChromeApp.Backend.Data;
using EbayChromeApp.Backend.Infrastructure;
using EbayChromeApp.Backend.Options;
using EbayChromeApp.Backend.Services;
using EbayChromeApp.Backend.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

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
                c.MaxCallTimes = Configuration.GetValue<int>("MaxCallTimes");
            });

            services.AddScoped<IStorage, FileStorage>();
            services.AddScoped<IEbayService, CachedEbayService>();

            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseSqlite("Data Source=ebay.db");
            });

            services.Configure<EbayServiceOptions>(Configuration.GetSection("EbayService"));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            app.UseAppHub();

            app.UseMvcWithDefaultRoute();
        }
    }
}
