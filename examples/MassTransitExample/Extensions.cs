//using Serilog;
//using Serilog.Events;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Options;

//namespace MassTransitExample;

//public static class Extensions
//{
//    public static T ConfigureSerilog<T>(this T builder) where T : IHostBuilder
//    {
//        Log.Logger = new LoggerConfiguration()
//            .MinimumLevel.Information()
//            .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
//            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
//            .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Information)
//            .Enrich.FromLogContext()
//            .WriteTo.Console()
//            .CreateLogger();

//        builder.UseSerilog();

//        return builder;
//    }
//}
