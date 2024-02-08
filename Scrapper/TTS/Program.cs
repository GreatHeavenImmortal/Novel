// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Xml.Serialization;
using TTS;
using TTS.Interfaces;
using TTS.Models;

NovelbinnetOption? options = new NovelbinnetOption();
List<ServiceDescriptor> items = new List<ServiceDescriptor>();

WebsiteCatalog.Catalog.Add("novelbin.net",typeof(NovelbinnetOption));

IHost host = Host.CreateDefaultBuilder()
    .ConfigureHostConfiguration(ctx =>
    {

    })
    .ConfigureLogging(log =>
    {

    })
    .ConfigureAppConfiguration((ctx, configuration) =>
    {
        configuration
            .AddJsonFile("Settings/ParserConfig.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((ctx, services) =>
    {
        //options configurations
        var configurationRoot = ctx.Configuration;
        services.Configure<NovelbinnetOption>(configurationRoot.GetSection(NovelbinnetOption.SectionName));

        services.AddSingleton<Main>();
        services.AddHttpClient("defaultClient", client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X x.y; rv:42.0)");
        });
        services.AddTransient<ConfigurationParserService>();
        services.AddSingleton<TextFunctionsService>();
        services.AddTransient<IParserService, ParserService>();
    })
    .Build();

Main main = host.Services.GetRequiredService<Main>();
main.StartAsync(args);
await host.RunAsync();
public static class WebsiteCatalog
{
    public static Dictionary<string, Type> Catalog = new Dictionary<string, Type>();
}