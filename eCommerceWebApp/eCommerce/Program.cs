using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AspNetCore.Identity.MongoDbCore.Models;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDB.Driver;

namespace eCommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eCommerce API", Version = "v1" });
                
                // Configure Swagger to use JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Configure PaymentSettings
            builder.Services.Configure<PaymentSettings>(
                builder.Configuration.GetSection("PaymentSettings"));

            // Register HttpClient
            builder.Services.AddHttpClient();

            // Register PaymentService
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            // Configure MongoDB for Identity
            var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
            {
                MongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = builder.Configuration.GetConnectionString("MongoConnection") ?? 
                        throw new InvalidOperationException("MongoDB connection string not found"),
                    DatabaseName = "eCommerceIdentity"
                },
                IdentityOptionsAction = options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                }
            };

            // Configure MongoDB Context
            builder.Services.AddSingleton<MongoContext>();

            // Configure Identity
            builder.Services.AddIdentity<AppUser, MongoIdentityRole<string>>()
                .AddMongoDbStores<AppUser, MongoIdentityRole<string>, string>(
                    mongoDbIdentityConfig.MongoDbSettings.ConnectionString,
                    mongoDbIdentityConfig.MongoDbSettings.DatabaseName
                )
                .AddDefaultTokenProviders();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtIssuer"] ?? throw new InvalidOperationException("JWT issuer not configured"),
                    ValidAudience = builder.Configuration["JwtIssuer"] ?? throw new InvalidOperationException("JWT issuer not configured"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"] ?? throw new InvalidOperationException("JWT key not configured")))
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}