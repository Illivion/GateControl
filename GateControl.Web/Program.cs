using System.Net;
using System.Security.Authentication;
using GateControl.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GateControl.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseKestrel(o =>
                            {
                                o.Listen(IPAddress.Any, Settings.HttpPort,
                                    lo =>
                                    {
                                        lo.Protocols = HttpProtocols.Http2;
                                        lo.UseHttps("server2.pfx", "illivionaiomo", o =>
                                        {
                                            o.AllowAnyClientCertificate();
                                            
                                        });
                                    });
                                o.Listen(IPAddress.Any, 20090);
                            });
                        webBuilder.UseStartup<Startup>();
                    }
                );
    }
}
