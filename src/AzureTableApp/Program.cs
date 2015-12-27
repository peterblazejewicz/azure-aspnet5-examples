using System;
using System.Threading.Tasks;
using AzureTableApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
			RunApplication(args).Wait();	
        }
		public static async Task RunApplication(string[] args)
		{
			try {
				// configuration
				var builder = new ConfigurationBuilder()
					.AddJsonFile("Configs/appsettings.json")
					.AddUserSecrets()
					.AddEnvironmentVariables();
				Configuration = builder.Build();
				// options 
				ConfigurationBinder.Bind(Configuration.GetSection("Azure:Storage"), options);
				// appliation
				var app = await Application.CreateAsync(options);
				Console.WriteLine("Press any key to exit ...");
            	Console.Read();
				Environment.Exit(0);			
			} catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				Environment.Exit(-1);
			}
		}
		private static AzureStorageOptions options { get; set; } = new AzureStorageOptions();
		private static IConfiguration Configuration { get; set; }
    }
}
