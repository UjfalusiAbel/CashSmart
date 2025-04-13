
using System.Text;
using System.Text.Json.Serialization;
using CashSmart.Application.Services;
using CashSmart.Core.Models;
using CashSmart.Core.Persistence;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MediatR;
using CashSmart.Application.Models.UserManagement;
using CashSmart.Application.Models.InvestmentManagement;

public class CashSmartApp()
{
    public static void Main(string[] args)
    {
        try
        {
            Env.Load("../.env");

            var builder = WebApplication.CreateBuilder(args);

            var jwtSettings = new
            {
                Secret = Environment.GetEnvironmentVariable("Jwt_Secret"),
                Issuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
                Audience = Environment.GetEnvironmentVariable("Jwt_Audience")
            };

            Console.WriteLine("JWT Secret: " + jwtSettings.Secret);
            Console.WriteLine("JWT Issuer: " + jwtSettings.Issuer);
            Console.WriteLine("JWT Audience: " + jwtSettings.Audience);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("LocalHost", builder =>
                    builder.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
                options.AddPolicy("Prod", builder =>
                    builder.WithOrigins("http://meloptica.stream", "https://meloptica.stream")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                );
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });

            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CacheMe API", Version = "v1" });
                    c.UseInlineDefinitionsForEnums();
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter 'Bearer' [space] and then your token in the text input below."
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
                Array.Empty<string>()
            }
                    });
                });

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__WebApiDatabase");
            Console.WriteLine(connectionString);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in environment variables.");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<JwtTokenService>();

            // MediatR
            builder.Services.AddMediatR(typeof(GetUserByEmail.Handler).Assembly);
            builder.Services.AddMediatR(typeof(LoginUser.Handler).Assembly);
            builder.Services.AddMediatR(typeof(RegisterUser.Handler).Assembly);
            builder.Services.AddMediatR(typeof(AddInvestment.AddInvestmentRequestHandler).Assembly);
            builder.Services.AddMediatR(typeof(ListInvestmentsByUser.ListInvestmentsByUserRequestHandler).Assembly);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            if (app.Environment.IsProduction())
            {
                Console.WriteLine("Production cors.");
                app.UseCors("Prod");
            }
            else
            {
                Console.WriteLine("Dev cors.");
                app.UseCors("LocalHost");
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<CashSmartApp>>();
                logger.LogError(ex, "Error during migration");
            }


            app.Run();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

