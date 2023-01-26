using ClinicService.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Net;

#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
namespace ClinicServiceV2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Configure Kestrel
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

            // Configure Swagger
            builder.Services.AddGrpcSwagger();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "ClinicServiceV2.xml");
                c.IncludeXmlComments(filePath);
                c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
            });

            // Add services to the container.
            builder.Services.AddAuthorization();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }

            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.MapGrpcService<ClinicServiceV2.Services.ClinicService>().EnableGrpcWeb();
            app.Map("/", () => "Communication with gRPC endpoints must be through a gRPC client.");

            app.Run();
        }
    }
}
