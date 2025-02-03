using AvroBlogExample;
using EventBus.Sdk;
using EventBus.Sdk.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    //.ConfigureAppConfiguration((context, config) =>
    //{
    //    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    //})
    .ConfigureServices((context, services) =>
    {

        services.AddSingleton<Demo>();

        var evbConfig = context.Configuration.GetSection("EvbConfig").Get<EventBusConfig>();
        services.AddEventBus(evbConfig, evb =>
            evb
                .WithKafkaProducer()
                .WithKafkaConsumer<MyCloudEventHandler>()
        );


    })
    .Build();


var t = host.RunAsync();

var my = host.Services.GetRequiredService<Demo>();
await my.Publish();

await t;