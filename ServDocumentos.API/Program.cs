using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ServDocumentos.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
              .UseStartup<Startup>()
              .ConfigureAppConfiguration((builderContext, config) =>
              {
                  config.AddJsonFile("serilog-config.json");
              })
              .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                  .ReadFrom.Configuration(hostingContext.Configuration)
              );

            Serilog.Debugging.SelfLog.Enable(msg => File.AppendAllText("milog.txt", msg));

            return builder;
        }
    }

}
