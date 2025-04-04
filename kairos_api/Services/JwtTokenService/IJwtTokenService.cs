using kairos_api.Models;

namespace kairos_api.Services.JwtTokenService;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}