using Data.Models;

namespace API.Services.JwtTokenService;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}