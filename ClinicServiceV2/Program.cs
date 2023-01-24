using ClinicService.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicServiceV2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddGrpc();

            builder.Services.AddDbContext<ClinicServiceDbContext>(options =>
            {
                options.UseMySql(builder.Configuration["Settings:DatabaseOptions:ConnectionString"], new MySqlServerVersion(new Version(8, 0)));
            });


            // Add services to the container.
            builder.Services.AddAuthorization();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            

            app.Run();
        }
    }
}