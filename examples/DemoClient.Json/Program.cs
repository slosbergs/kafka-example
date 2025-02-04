using DemoClient.Json;
using EventBus.Sdk;
using EventBus.Sdk.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var host = Host.CreateDefaultBuilder(args)
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

var foo = new FooClass() { Name = "Frank Drebin", FavoriteColor = "RED" };
await my.Publish(foo);

await t;