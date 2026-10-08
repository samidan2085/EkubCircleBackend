using EkubCircle.Application.Interfaces;
using EkubCircle.Domain.Entities;
using EkubCircle.Infrastructure.Persistence;
using EkubCircle.Infrastructure.Services; // <-- Make sure this points to Infrastructure.Services
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EkubCircle.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("DefaultConnection is missing.");
    }

    // 1. Register DbContext first
    services.AddDbContext<EkubDbContext>(options =>
        options.UseNpgsql(
            connectionString,
            npgsql => npgsql.MigrationsAssembly(typeof(EkubDbContext).Assembly.FullName)));

    services.AddScoped<IEkubDbContext>(sp =>
        sp.GetRequiredService<EkubDbContext>());

    // 2. Register Identity second (this provides UserManager)
    services
        .AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<EkubDbContext>()
        .AddDefaultTokenProviders();

    // 3. Register your custom services last
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<ICircleService, CircleService>();
    services.AddScoped<ILotteryService, LotteryService>();
    services.AddScoped<IPaymentService, PaymentService>();

    return services;
}
}