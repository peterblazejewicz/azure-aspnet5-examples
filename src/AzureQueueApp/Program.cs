using System;
using AzureQueueApp.Models;
using Fclp;
using GenFu;
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
                    RunApplication(p.Object);
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

        public static void RunApplication(ApplicationOptions options)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("Configs/appsettings.json");
            builder.AddUserSecrets();
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            app = new Application(new AzureStorageOptions
            {
                AccountName = Configuration.GetSection("Azure:Storage")["AccountName"],
                AccountKey = Configuration.GetSection("Azure:Storage")["AccountKey"],
                QueueName = Configuration.GetSection("Azure:Storage")["QueueName"]
            });
            // GenFu configuration specific for our example
            A.Configure<TicketRequest>()
                .Fill(t => t.OrderDate)
                .AsFutureDate();
            A.Configure<TicketRequest>()
                .Fill(t => t.NumberOfTickets)
                .WithinRange(1, 10);
            A.Configure<TicketRequest>()
                .Fill(t => t.Email)
                .AsEmailAddressForDomain("example.com");
            // execute
            switch (options.Operation)
            {
                case Operation.ChangeMessage:
                    app.ChangeMessage();
                    break;
                case Operation.InsertMessage:
                    app.InsertMessage();
                    break;
                case Operation.PeekMessage:
                    app.PeekMessage();
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
--operation InsertMessage		inserts single TicketRequest into queue
--operation PeekMessage			peeks a single TicketRequest from queue";
    }
}
