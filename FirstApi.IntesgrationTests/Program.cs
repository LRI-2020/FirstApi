// See https://aka.ms/new-console-template for more information

using FirstApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var configBuilder = new ConfigurationBuilder();
configBuilder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
IConfiguration config = configBuilder.Build();
var connectionString = config["ConnectionStrings:FirstApiIntegrationTests"];
HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
var host = hostBuilder.Build();
await host.RunAsync();