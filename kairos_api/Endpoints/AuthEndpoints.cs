using Carter;
using kairos_api.DTOs.AuthDTOs;
using kairos_api.Services.AuthService;

namespace kairos_api.Endpoints;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", Register);

        app.MapPost("/api/auth/login", Login);
    }

    public static async Task<IResult> Register(IAuthService authService, RegisterDTO dto)
    {
        try
        {
            var token = await authService.RegisterAsync(dto);
            return Results.Ok(token);
        }
        catch (ArgumentException ex)
        {
            return Results.Unauthorized();
        }
    }

    public static async Task<IResult> Login(IAuthService authService, LoginDTO dto)
    {
        try
        {
            var token = await authService.LoginAsync(dto);
            return Results.Ok(new { Token = token });
        }
        catch (ArgumentException ex)
        {
            return Results.Unauthorized();
        }
    }
}
