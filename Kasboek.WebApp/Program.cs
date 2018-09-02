using System;
using Kasboek.WebApp.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kasboek.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            InitializeDb(host);
            host.Run();
        }

        public static void InitializeDb(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    using (var context = services.GetRequiredService<KasboekDbContext>())
                    {
                        KasboekInitializer.Initialize(context);
                    }
                    logger.LogInformation("Initialiseren van de database is afgerond.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Er is een fout opgetreden bij het initialiseren van de database.");
                }
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
