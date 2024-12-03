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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eCommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(Program).Assembly)
                .AddControllersAsServices();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eCommerce API", Version = "v1" });
            });

            // Configure MongoDB
            builder.Services.AddSingleton<MongoContext>();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Add logging
            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Add error handling middleware
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("404 error for path: {Path}", context.Request.Path);
                    }
                }
                catch (Exception ex)
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An unhandled exception occurred");
                    throw;
                }
            });

            app.UseRouting();

            // Enable CORS
            app.UseCors("AllowAll");

            // app.UseHttpsRedirection();
            app.UseAuthorization();

            // Add endpoint routing middleware
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", () => Results.Redirect("/swagger"));
            });

            // Add middleware to log all requests
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Request {Method} {Path}", context.Request.Method, context.Request.Path);
                await next();
                logger.LogInformation("Response {StatusCode} for {Method} {Path}", 
                    context.Response.StatusCode, context.Request.Method, context.Request.Path);
            });

            app.Run();
        }
    }
}