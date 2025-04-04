using kairos_api.Services.HashingService;
using kairos_api.Services.JwtTokenService;
using kairos_api.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Mvc;
using kairos_api.Repositories;
using kairos_api.Entities;

namespace kairos_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _tokenService;
    private readonly IHashingService _hashingService;

    public AuthController(IUnitOfWork unitOfWork, IJwtTokenService tokenService, IHashingService hashingService) : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _hashingService = hashingService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            return BadRequest("Passwords do not match.");
        }

        var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email.ToLower());
        if (existingUser != null)
        {
            return BadRequest("User with this email already exists.");
        }

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

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email.ToLower());
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        bool isPasswordValid = _hashingService.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new { Token = token });
    }
}
