using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var configuration = builder.Configuration;

// Retrieve connection string from appsettings.json
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing or empty.");
}

// Register ConnectionFactory with DI
builder.Services.AddSingleton(new ConnectionFactory(connectionString));

// Register repositories with DI using the connection string
builder.Services.AddScoped<ITaskRepository>(provider => new TaskRepository(connectionString));
builder.Services.AddScoped<IUserRepository>(provider => new UserRepository(connectionString));

// Register application services
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<UserService>();

// Load JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var securityKeyConf = jwtSettings["SecretKey"];
var issuerConf = jwtSettings["Issuer"];
var audienceConf = jwtSettings["Audience"];
if (string.IsNullOrEmpty(issuerConf))
{
    throw new InvalidOperationException("JWT Issuer is missing or empty.");
}
if (string.IsNullOrEmpty(audienceConf))
{
    throw new InvalidOperationException("JWT Audience is missing or empty.");
}
if (string.IsNullOrEmpty(securityKeyConf))
{
    throw new InvalidOperationException("JWT SecretKey is missing or empty.");
}

// Configure authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuerConf,
            ValidAudience = audienceConf,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyConf))
        };
    });


builder.Services.AddControllers();

// Configure OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Manager API",
        Version = "v1",
        Description = "API documentation for Task Manager"
    });
    // Add JWT Bearer support in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and the JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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


var app = builder.Build();

// Log the connection string (optional)
Console.WriteLine($"Database Connection: {connectionString}");

var connectionFactory = app.Services.GetRequiredService<ConnectionFactory>();
await DatabaseInitializer.InitializeAsync(connectionFactory);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API v1");
    });

    // Redirect root URL ("/") to Swagger
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger/index.html");
            return;
        }
        await next();
    });
}

app.UseHttpsRedirection();
app.Run();
