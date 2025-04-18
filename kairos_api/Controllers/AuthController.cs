﻿using kairos_api.DTOs.AuthDTOs;
using kairos_api.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace kairos_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) : base(null)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        try
        {
            var message = await _authService.RegisterAsync(dto);
            return Ok(message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(new { Token = token });
        }
        catch (ArgumentException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
