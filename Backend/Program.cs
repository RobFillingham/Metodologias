using Backend.Extensions;
using Backend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Backend API",
        Description = "A clean architecture ASP.NET Core Web API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Your Team",
            Email = "contact@example.com"
        }
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Order tags/controllers alphabetically
    options.OrderActionsBy((apiDesc) => 
    {
        var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
        var actionName = apiDesc.ActionDescriptor.RouteValues["action"];
        var httpMethod = apiDesc.HttpMethod;
        
        // Define custom order for endpoints
        var order = controllerName switch
        {
            "Auth" => "1",
            "ParameterSets" => "2",
            "Languages" => "3",
            "Projects" => "4",
            "Estimations" => "5",
            "Functions" => "6",
            _ => "9"
        };
        
        // Order by: GroupNumber-ControllerName-HttpMethod-ActionName
        return $"{order}-{controllerName}-{httpMethod}-{actionName}";
    });

    // Enable XML comments (optional)
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // options.IncludeXmlComments(xmlPath);
});

// Add CORS
builder.Services.AddCorsPolicy();

// Add database context
builder.Services.AddDatabaseContext(builder.Configuration);

// Add JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add application services
builder.Services.AddApplicationServices();

// Add repositories
builder.Services.AddRepositories();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API V1");
        options.RoutePrefix = "swagger"; // Access at /swagger
    });
}

// Global exception handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
