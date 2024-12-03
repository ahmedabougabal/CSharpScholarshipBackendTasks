using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDB.Driver;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;

namespace eCommerce
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eCommerce API", Version = "v1" });
            });

            builder.Services.AddIdentity<AppUser, MongoIdentityRole<string>>()
                .AddMongoDbStores<AppUser, MongoIdentityRole<string>, string>(
                    builder.Configuration["MongoConnection"],
                    "eCommerce"
                );
            builder.Services.AddScoped<MongoContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "eCommerce API v1"));
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}