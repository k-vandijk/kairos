using API.Services.HashingService;
using API.Services.JwtTokenService;
using Data;
using Data.DTOs.AccountDTOs;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly DataContext _context;
    private readonly IJwtTokenService _tokenService;
    private readonly IHashingService _hashingService;

    public AuthController(DataContext context, IJwtTokenService tokenService, IHashingService hashingService)
    {
        _context = context;
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

        // Check if a user with the provided email already exists
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
        if (existingUser != null)
        {
            return BadRequest("User with this email already exists.");
        }

        // Generate salt and hash the password using BCrypt
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string passwordHash = _hashingService.HashPassword(dto.Password, salt);

        var user = new ApplicationUser
        {
            Email = dto.Email.ToLower(),
            PasswordHash = passwordHash,
            PasswordSalt = salt
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        // Retrieve the user by email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        bool isPasswordValid = _hashingService.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
        {
            return Unauthorized("Invalid email or password.");
        }

        // Generate a JWT token
        var token = _tokenService.GenerateToken(user);
        return Ok(new { Token = token });
    }
}
