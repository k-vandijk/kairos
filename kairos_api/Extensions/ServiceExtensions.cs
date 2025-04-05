using kairos_api.Repositories;
using kairos_api.Services.AuthService;
using kairos_api.Services.CapsuleService;
using kairos_api.Services.GeolocationService;
using kairos_api.Services.HashingService;
using kairos_api.Services.JwtTokenService;

namespace kairos_api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICapsuleService, CapsuleService>();
        services.AddScoped<IGeolocationService, GeolocationService>();
        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        return services;
    }
}
