using ClinicService.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ClinicServiceV2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Configuration Kestrel
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5100, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
                options.Listen(IPAddress.Any, 5101, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            });

            // Configure gRPC
            builder.Services.AddGrpc().AddJsonTranscoding();


            // DBContext
            builder.Services.AddDbContext<ClinicServiceDbContext>(options =>
            {
                options.UseMySql(builder.Configuration["Settings:DatabaseOptions:ConnectionString"], new MySqlServerVersion(new Version(8, 0)));
            });


            // Add services to the container.
            builder.Services.AddAuthorization();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

            app.MapGrpcService<ClinicServiceV2.Services.ClinicService>().EnableGrpcWeb();
            app.Map("/", () => "Communication with gRPC endpoints must be through a gRPC client.");

            app.Run();
        }
    }
}