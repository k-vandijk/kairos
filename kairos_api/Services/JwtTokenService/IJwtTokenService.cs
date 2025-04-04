using kairos_api.Entities;

namespace kairos_api.Services.JwtTokenService;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}