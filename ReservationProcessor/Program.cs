using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMqUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var rabbitConfig = hostContext.Configuration.GetSection("Rabbit");
                    services.Configure<RabbitOptions>(rabbitConfig);

                    //services.AddHttpClient(); // anything can put HttpClient on the constructor.
                    //services.AddHttpClient("tacos");
                    services.AddHttpClient<ReservationHttpService>();

                    services.AddHostedService<ReservationListener>();
                });
    }
}
