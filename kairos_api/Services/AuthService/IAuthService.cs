using kairos_api.DTOs.AuthDTOs;

namespace kairos_api.Services.AuthService;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDTO dto);
    Task<string> LoginAsync(LoginDTO dto);
}
