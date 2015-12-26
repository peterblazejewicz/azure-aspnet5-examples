using System;
using System.Threading.Tasks;
using AzureQueueApp.Models;
using Fclp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace AzureQueueApp
{
    public class Program
    {
        public Program()
        {
            env = PlatformServices.Default.Application;
        }
        public static void Main(string[] args)
        {
            Logger.Get().LogInformation("starting");
            try
            {
                var p = new FluentCommandLineParser<ApplicationOptions>();
                p.Setup<Operation>(options => options.Operation)
                    .As('o', "operation")
                    .Required();
                p.SetupHelp("?", "help")
                    .Callback(text => Console.WriteLine(HELP));
                var results = p.Parse(args);
                if (results.HasErrors == false)
                {
                    RunApplication(p.Object).Wait();
                }
                else
                {
                    p.HelpOption.ShowHelp(p.Options);
                }
                Console.WriteLine("Press a key to exit ...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.Get().LogError($"Error: {ex.Message}");
                Logger.Get().LogInformation(ex.StackTrace);
                Environment.Exit(1);
            }

        }

        public static async Task RunApplication(ApplicationOptions options)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("Configs/appsettings.json");
            builder.AddUserSecrets();
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            app = await Application.CreateAsync(new AzureStorageOptions
            {
                AccountName = Configuration.GetSection("Azure:Storage")["AccountName"],
                AccountKey = Configuration.GetSection("Azure:Storage")["AccountKey"],
                QueueName = Configuration.GetSection("Azure:Storage")["QueueName"]
            });
            // execute
            switch (options.Operation)
            {
                case Operation.ChangeMessage:
                    await app.ChangeMessage();
                    break;
                case Operation.InsertMessage:
                    await app.InsertMessage();
                    break;
                case Operation.GetLength:
                    await app.GetLength();
                    break;
                case Operation.PeekMessage:
                    await app.PeekMessage();
                    break;
                case Operation.RemoveMessage:
                    await app.RemoveMessage();
                    break;
                case Operation.Unknown:
                default:
                    Logger.Get().LogCritical("Unknown option used");
                    break;
            }
        }
        private static IConfiguration Configuration { get; set; }
        private static IApplicationEnvironment env { get; set; }
        private static IApplication app;
        private const string HELP = @"Azure Queue Storage example application.
Author: @peterblazejewicz
Options:
--operation ChangeMessage		changes a content of single TicketRequest from queue
--operation GetLength			checks length of the queue
--operation InsertMessage		inserts single TicketRequest into queue
--operation PeekMessage			peeks a single TicketRequest from queue
--operation RemoveMessage		gets a message and deletes it from queue";
    }
}
