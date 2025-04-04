using kairos_api.DTOs.AuthDTOs;
using kairos_api.Entities;
using kairos_api.Repositories;
using kairos_api.Services.HashingService;
using kairos_api.Services.JwtTokenService;

namespace kairos_api.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _tokenService;
    private readonly IHashingService _hashingService;

    public AuthService(IUnitOfWork unitOfWork, IJwtTokenService tokenService, IHashingService hashingService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _hashingService = hashingService;
    }

    public async Task<string> RegisterAsync(RegisterDTO dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new ArgumentException("Passwords do not match.");

        var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email.ToLower());
        if (existingUser != null)
            throw new ArgumentException("User with this email already exists.");

        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string passwordHash = _hashingService.HashPassword(dto.Password, salt);

        var user = new User
        {
            Email = dto.Email.ToLower(),
            PasswordHash = passwordHash,
            PasswordSalt = salt
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return "User registered successfully.";
    }

    public async Task<string> LoginAsync(LoginDTO dto)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email.ToLower());
        if (user == null)
            throw new ArgumentException("Invalid email or password.");

        bool isPasswordValid = _hashingService.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
            throw new ArgumentException("Invalid email or password.");

        var token = _tokenService.GenerateToken(user);
        return token;
    }
}
