using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PgpEncrypt.File.Extensions;
using Serilog;

namespace Habitat.BackInStock.Function
{
    public class Program
    {
        public static void Main()
        {
            // Initialize serilog logger
            Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                 .MinimumLevel.Debug()
                 .Enrich.FromLogContext()
                 .CreateLogger();

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    // Add logging
                    services.AddSingleton(LoggerFactory.Create(builder =>
                    {
                        builder
                            .AddSerilog(dispose: true);
                    }));
                    // Add file service
                    services.AddSourceBlobStorageFileService("https://bitscry.blob.core.windows.net", "6425e6e5-c928-402d-83f1-bd0eb11bbd56");
                    services.AddDestinationBlobStorageFileService("https://bitscry.blob.core.windows.net", "6425e6e5-c928-402d-83f1-bd0eb11bbd56");
                })
                .Build();

            host.Run();
        }
    }
}