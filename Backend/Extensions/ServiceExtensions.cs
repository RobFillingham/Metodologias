using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Backend.Data.Context;
using Backend.Repositories.Implementations;
using Backend.Repositories.Implementations.CocomoThree;
using Backend.Repositories.Implementations.CocomoOne;
using Backend.Repositories.Implementations.CocomoTwoStageOne;
using Backend.Repositories.Implementations.CocomoIIStage3;
using Backend.Repositories.Interfaces;
using Backend.Repositories.Interfaces.CocomoThree;
using Backend.Repositories.Interfaces.CocomoOne;
using Backend.Repositories.Interfaces.CocomoTwoStageOne;
using Backend.Repositories.Interfaces.CocomoIIStage3;
using Backend.Services.Implementations;
using Backend.Services.Implementations.CocomoThree;
using Backend.Services.Implementations.CocomoOne;
using Backend.Services.Implementations.CocomoTwoStageOne;
using Backend.Services.Implementations.CocomoIIStage3;
using Backend.Services.Interfaces;
using Backend.Services.Interfaces.CocomoThree;
using Backend.Services.Interfaces.CocomoOne;
using Backend.Services.Interfaces.CocomoTwoStageOne;
using Backend.Services.Interfaces.CocomoIIStage3;

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
        // Register Auth services
        services.AddScoped<IAuthService, AuthService>();
        
        // Register COCOMO 3 services
        services.AddScoped<Backend.Services.Interfaces.CocomoThree.IParameterSetService, Backend.Services.Implementations.CocomoThree.ParameterSetService>();
        services.AddScoped<Backend.Services.Interfaces.CocomoThree.ICocomoCalculationService, Backend.Services.Implementations.CocomoThree.CocomoCalculationService>();
        services.AddScoped<Backend.Services.Interfaces.CocomoThree.ILanguageService, Backend.Services.Implementations.CocomoThree.LanguageService>();
        services.AddScoped<Backend.Services.Interfaces.CocomoThree.IProjectService, Backend.Services.Implementations.CocomoThree.ProjectService>();
        services.AddScoped<Backend.Services.Interfaces.CocomoThree.IEstimationService, Backend.Services.Implementations.CocomoThree.EstimationService>();
        services.AddScoped<Backend.Services.Interfaces.CocomoThree.IEstimationFunctionService, Backend.Services.Implementations.CocomoThree.EstimationFunctionService>();
        
        // Register COCOMO 1 services
        services.AddScoped<ICocomo1EstimationService, Cocomo1EstimationService>();
        
        // Register COCOMO 2 Stage 1 services
        services.AddScoped<ICocomoCalculationCocomo2Stage1Service, CocomoCalculationCocomo2Stage1Service>();
        services.AddScoped<IParameterSetCocomo2Stage1Service, ParameterSetCocomo2Stage1Service>();
        services.AddScoped<IEstimationCocomo2Stage1Service, EstimationCocomo2Stage1Service>();
        services.AddScoped<IComponentCocomo2Stage1Service, ComponentCocomo2Stage1Service>();
        
        // Register COCOMO II Stage 3 services
        services.AddScoped<Backend.Services.Interfaces.CocomoIIStage3.IParameterSetService, Backend.Services.Implementations.CocomoIIStage3.ParameterSetService>();
        services.AddScoped<Backend.Services.Interfaces.CocomoIIStage3.ICocomoCalculationService, Backend.Services.Implementations.CocomoIIStage3.CocomoCalculationService>();
        services.AddScoped<Backend.Services.Interfaces.CocomoIIStage3.ILanguageService, Backend.Services.Implementations.CocomoIIStage3.LanguageService>();
        // Note: CocomoIIStage3 uses the CocomoThree Projects endpoint (api/Projects) instead of having its own
        services.AddScoped<Backend.Services.Interfaces.CocomoIIStage3.IEstimationService, Backend.Services.Implementations.CocomoIIStage3.EstimationService>();
        services.AddScoped<Backend.Services.Interfaces.CocomoIIStage3.IEstimationFunctionService, Backend.Services.Implementations.CocomoIIStage3.EstimationFunctionService>();
        
        // Register KLOC services
        services.AddScoped<IKlocEstimationService, KlocEstimationService>();
        
        // Register Function Point services
        services.AddScoped<IFunctionPointEstimationService, FunctionPointEstimationService>();
        
        // Register Use Case Point services
        services.AddScoped<IUseCasePointEstimationService, UseCasePointEstimationService>();
        
        return services;
    }

    /// <summary>
    /// Register all repositories
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register COCOMO 3 repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoThree.IParameterSetRepository, Backend.Repositories.Implementations.CocomoThree.ParameterSetRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoThree.ILanguageRepository, Backend.Repositories.Implementations.CocomoThree.LanguageRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoThree.IProjectRepository, Backend.Repositories.Implementations.CocomoThree.ProjectRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoThree.IEstimationRepository, Backend.Repositories.Implementations.CocomoThree.EstimationRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoThree.IEstimationFunctionRepository, Backend.Repositories.Implementations.CocomoThree.EstimationFunctionRepository>();
        
        // Register COCOMO 1 repositories
        services.AddScoped<ICocomo1EstimationRepository, Cocomo1EstimationRepository>();
        
        // Register COCOMO 2 Stage 1 repositories
        services.AddScoped<IParameterSetCocomo2Stage1Repository, ParameterSetCocomo2Stage1Repository>();
        services.AddScoped<IEstimationCocomo2Stage1Repository, EstimationCocomo2Stage1Repository>();
        services.AddScoped<IComponentCocomo2Stage1Repository, ComponentCocomo2Stage1Repository>();
        
        // Register COCOMO II Stage 3 repositories
        services.AddScoped<Backend.Repositories.Interfaces.CocomoIIStage3.IParameterSetRepository, Backend.Repositories.Implementations.CocomoIIStage3.ParameterSetRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoIIStage3.ILanguageRepository, Backend.Repositories.Implementations.CocomoIIStage3.LanguageRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoIIStage3.IProjectRepository, Backend.Repositories.Implementations.CocomoIIStage3.ProjectRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoIIStage3.IEstimationRepository, Backend.Repositories.Implementations.CocomoIIStage3.EstimationRepository>();
        services.AddScoped<Backend.Repositories.Interfaces.CocomoIIStage3.IEstimationFunctionRepository, Backend.Repositories.Implementations.CocomoIIStage3.EstimationFunctionRepository>();
        
        // Register KLOC repositories
        services.AddScoped<IKlocEstimationRepository, KlocEstimationRepository>();
        
        // Register Function Point repositories
        services.AddScoped<IFunctionPointEstimationRepository, FunctionPointEstimationRepository>();
        
        // Register Use Case Point repositories
        services.AddScoped<IUseCasePointEstimationRepository, UseCasePointEstimationRepository>();
        
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
