using Ncs.Cqrs.Api.Helpers;
using Ncs.Cqrs.Application.Common.Converters;
using Ncs.Cqrs.Application.Features.Reservations.Validators;
using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Features.Users.Mappings;
using Ncs.Cqrs.Application.Features.Users.Validators;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Application.Services;
using Ncs.Cqrs.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// ✅ Register HttpContextAccessor (Required for accessing HttpContext)
builder.Services.AddHttpContextAccessor();

// 🔹 Load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 🔹 Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 🔹 Setup Dependency Injection

// ✅ AutoMapper
var profileAssemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(UsersProfile).Assembly
};
builder.Services.AddAutoMapper(profileAssemblies);

// ✅ FluentValidation
var fluentAssemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(UserLoginCommandValidator).Assembly
};
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
var excludedValidators = new[]
{
    typeof(CreateReservationGuestsValidator),
    typeof(UpdateReservationGuestsValidator)
};

builder.Services.AddValidatorsFromAssemblies(fluentAssemblies,
    includeInternalTypes: false,
    filter: result => !excludedValidators.Contains(result.ValidatorType));



// ✅ MediatR (CQRS)
var assemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(UserLoginCommand).Assembly
};
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

// ✅ Database & Repository Layer
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMasterRepository, MasterRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuSchedulesRepository, MenuSchedulesRepository>();
builder.Services.AddScoped<IReservationsRepository, ReservationsRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

// ✅ Controllers & JSON Serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // ✅ Keeps PascalCase
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Optional
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter("yyyy-MM-dd"));
    });

// ✅ API Behavior
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// 🔹 Configure Authentication & Authorization

// ✅ Load JWT settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("JWT Secret Key is missing from appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// 🔹 Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),      // /api/v1/orders
        new QueryStringApiVersionReader("api-version"),  // ?api-version=1.0
        new HeaderApiVersionReader("X-API-Version")  // Header: X-API-Version: 1.0
    );
});

// 🔹 Configure API Explorer for Swagger
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// 🔹 Configure Swagger with Bearer Authentication
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"Ncs.Cqrs API {description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
            Description = "API documentation for Canteen Application",
            Contact = new OpenApiContact
            {
                Name = "NCS Support",
                Email = "ncs-support@gmail.com"
            }
        });
    }

    // ✅ Enable Bearer Token in Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer' followed by your JWT token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme, Array.Empty<string>()
        }
    });

    // ✅ Additional Swagger Config
    c.EnableAnnotations(); // Enable Swagger annotations
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    c.SchemaFilter<GenericResponseSchemaFilter>();
    c.OperationFilter<GenericResponseSchemaFilter>();
    c.OperationFilter<AddResponseHeadersFilter>();

    // ✅ Order API Endpoints
    c.OrderActionsBy(apiDesc =>
    {
        var order = new Dictionary<string, int>
        {
            { "GET", 1 },
            { "POST", 2 },  // CREATE
            { "PUT", 3 },   // UPDATE
            { "PATCH", 3 }, // UPDATE (alternative)
            { "DELETE", 4 }
        };

        var httpMethod = apiDesc.HttpMethod?.ToUpper() ?? "GET";
        return $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{order.GetValueOrDefault(httpMethod, 99)}";
    });
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<ChangePasswordDtoExample>();
// 🔹 Configure CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins(
            "https://localhost:5001",
            "http://localhost:5000"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});



// 🔹 Build Application
var app = builder.Build();

// ✅ Middleware Pipeline Configuration
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging(); // Log requests

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        });
}

app.MapControllers();
string pathUpload = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
if (!Directory.Exists(pathUpload))
{
    Directory.CreateDirectory(pathUpload);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(pathUpload),
    RequestPath = "/uploads"
});
app.Run();
