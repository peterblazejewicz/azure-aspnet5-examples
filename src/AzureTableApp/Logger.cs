using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureTableApp
{
    public class Logger
    {
        public static ILogger Get()
        {
            if (Logger._logger == null)
            {
                var factory = new LoggerFactory();
                Logger._logger = factory.CreateLogger<Program>();
                var loggingConfiguration = new ConfigurationBuilder()
                    .AddJsonFile("Configs/logging.json")
                    .Build();
                factory.AddConsole(loggingConfiguration);
            }
            return Logger._logger;
        }

        private static ILogger _logger = null;
    }
}
