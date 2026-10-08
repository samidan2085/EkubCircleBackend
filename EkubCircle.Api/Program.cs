using System.Text;
using EkubCircle.Domain.Entities;
using EkubCircle.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. Infrastructure
// ============================================================
builder.Services.AddInfrastructureServices(builder.Configuration);

// ============================================================
// 2. Controllers
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ============================================================
// 3. OpenAPI + JWT Bearer
// ============================================================
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??=
            new Microsoft.OpenApi.OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<
                string,
                Microsoft.OpenApi.IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] =
            new Microsoft.OpenApi.OpenApiSecurityScheme
            {
                Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.ParameterLocation.Header,
                Description = "Enter your JWT token"
            };

        return Task.CompletedTask;
    });
    // --------------------------------------------------------
    // Add Bearer requirement to API operations
    // -------------------------------------------------------
    options.AddOperationTransformer(
        (operation, context, cancellationToken) =>
        {
            operation.Security ??= [];

            operation.Security.Add(
                new Microsoft.OpenApi.OpenApiSecurityRequirement
                {
                    [
                        new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                            "Bearer",
                            context.Document)
                    ] = []
                });

            return Task.CompletedTask;
        });
});

// ============================================================
// 4. JWT Configuration
// ============================================================
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Jwt:Key is missing from configuration.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"];

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "Jwt:Issuer is missing from configuration.");
}

var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Jwt:Audience is missing from configuration.");
}

// ============================================================
// 5. JWT Authentication
// ============================================================
builder.Services
    .AddAuthentication(options =>
    {
        // IMPORTANT:
        // Use JWT instead of Identity cookies for API requests.
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                // Allow small time difference between
                // client and server.
                ClockSkew = TimeSpan.FromMinutes(1)
            };
    });

// ============================================================
// 6. Authorization
// ============================================================
builder.Services.AddAuthorization();

// ============================================================
// 7. CORS
// ============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================================================
// Build Application
// ============================================================
var app = builder.Build();

// ============================================================
// 8. Development Tools
// ============================================================
if (app.Environment.IsDevelopment())
{
    // OpenAPI JSON
    app.MapOpenApi();

    // Scalar API documentation
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Ekub Circle API")
            .WithTheme(ScalarTheme.Purple);
    });
}

// ============================================================
// 9. Middleware Pipeline
// ============================================================

// HTTPS
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAngular");

// IMPORTANT:
// Authentication MUST come before Authorization.
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// ============================================================
// 10. Seed Roles
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new[]
        {
            "Organizer",
            "Member"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result =
                    await roleManager.CreateAsync(
                        new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(
                            x => x.Description));

                    throw new InvalidOperationException(
                        $"Failed to create role '{role}': {errors}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        var logger =
            services.GetRequiredService<ILogger<Program>>();

        logger.LogError(
            ex,
            "An error occurred while seeding roles.");
    }
}

// ============================================================
// 11. Start Application
// ============================================================
app.Run();