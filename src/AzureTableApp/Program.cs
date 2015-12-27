using System;
using System.Threading.Tasks;
using AzureTableApp.Models;
using Microsoft.Extensions.Configuration;
using Fclp;
using Microsoft.Extensions.Logging;

namespace AzureTableApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.LogInformation("Starting application ...");
            var p = new FluentCommandLineParser<ApplicationOptions>();
            p.Setup<Operation>(options => options.Operation)
                    .As('o', "operation")
                    .Required();
            p.SetupHelp("?", "help")
                .Callback(text => Console.WriteLine(HELP_BANNER));
            var results = p.Parse(args);
            if (results.HasErrors == false)
            {
                RunApplication(p.Object.Operation).Wait();
            }
            else
            {
                Log.LogWarning("No valid options for appliation found");
                p.HelpOption.ShowHelp(p.Options);
            }
        }
        public static async Task RunApplication(Operation operation)
        {
            try
            {
                // configuration
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("Configs/appsettings.json")
                    .AddUserSecrets()
                    .AddEnvironmentVariables();
                Configuration = builder.Build();
                // options
                ConfigurationBinder.Bind(Configuration.GetSection("Azure:Storage"), options);
                // application
                var app = await Application.CreateAsync(options);
                switch (operation)
                {
                    default:
                        Console.WriteLine(HELP_BANNER);
                        break;

                }
                Console.WriteLine("Press any key to exit ...");
                Console.Read();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Log.LogInformation(ex.StackTrace);
                Environment.Exit(-1);
            }
        }
        private static AzureStorageOptions options { get; set; } = new AzureStorageOptions();
        private static IConfiguration Configuration { get; set; }
        private static ILogger Log { get; } = Logger.Get();
        private const string HELP_BANNER = @"Azure Storage Table example application
@author: @peterblazejewicz

Options:
-o/--operation AddEntity		Adds Customer entity to storage table
-h/--help				Shows usage information";

    }
}
