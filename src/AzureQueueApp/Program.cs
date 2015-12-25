using System;
using Fclp;
using Microsoft.Extensions.Configuration;
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
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Error: {0}", ex.Message);
                Console.ResetColor();
                Environment.Exit(1);
            }

        }

        public static void RunApplication(ApplicationOptions options)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            builder.AddUserSecrets();
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            app = new Application(new AzureStorageOptions
            {
                AccountName = Configuration.GetSection("Azure:Storage")["AccountName"],
                AccountKey = Configuration.GetSection("Azure:Storage")["AccountKey"],
                QueueName = Configuration.GetSection("Azure:Storage")["QueueName"]
            });
            switch (options.Operation)
            {
                case Operation.InsertMessage:
                    app.InsertMessage();
                    break;
                case Operation.PeekMessage:
                    app.PeekMessage();
                    break;
                case Operation.Unknown:
                default:
                    Console.WriteLine("Unknown");
                    break;
            }
        }
        private static IConfiguration Configuration { get; set; }
        private static IApplicationEnvironment env { get; set; }
        private static IApplication app;
        private const string HELP = @"Azure Queue Storage example application.
Author: @peterblazejewicz
Options:
--operation InsertMessage		inserts single TicketRequest into queue
--operation PeekMessage			peeks a single TicketRequest from queue
--operation PeekMessages		peeks a list of TicketRequests from queue
--operation GetMessage			gets a single TicketRequest from queue
--operation GetMessages			gets a set of TicketRequests from queue";
    }
}
