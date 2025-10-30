using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Backend.Data.Context;
using Backend.Repositories.Implementations;
using Backend.Repositories.Implementations.CocomoThree;
using Backend.Repositories.Interfaces;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Services.Implementations;
using Backend.Services.Implementations.CocomoThree;
using Backend.Services.Interfaces;
using Backend.Services.Interfaces.CocomoThree;

namespace Backend.Extensions;

/// <summary>
/// Extension methods for configuring services
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Register all application services
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services here
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IParameterSetService, ParameterSetService>();
        services.AddScoped<ICocomoCalculationService, CocomoCalculationService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IEstimationService, EstimationService>();
        services.AddScoped<IEstimationFunctionService, EstimationFunctionService>();
        
        return services;
    }

    /// <summary>
    /// Register all repositories
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register repositories here
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IParameterSetRepository, ParameterSetRepository>();
        services.AddScoped<ILanguageRepository, LanguageRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IEstimationRepository, EstimationRepository>();
        services.AddScoped<IEstimationFunctionRepository, EstimationFunctionRepository>();
        
        return services;
    }

    /// <summary>
    /// Configure database context
    /// </summary>
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        return services;
    }

    /// <summary>
    /// Configure JWT authentication
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

        services.AddAuthentication(options =>
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
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                NameClaimType = "sub" // Map the 'sub' claim to User.Identity.Name
            };
            
            // Disable claim type mapping to preserve original claim types
            options.MapInboundClaims = false;
        });

        return services;
    }

    /// <summary>
    /// Configure CORS policy
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", builder =>
            {
                builder.WithOrigins("http://localhost:4200") // Angular default port
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        return services;
    }
}
