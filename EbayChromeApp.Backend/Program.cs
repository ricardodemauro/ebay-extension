using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EbayChromeApp.Backend
{
    public class Program
    {
        private static string APP_SECRETS_KEY = "ac7cb23c-787b-4575-af3f-aa4cbd1624ad";

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                         .AddEnvironmentVariables()
                         .AddCommandLine(args)
                         .AddUserSecrets(APP_SECRETS_KEY);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
