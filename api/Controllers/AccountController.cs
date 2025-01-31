using Data;
using Data.DTOs.AccountDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using System.Security.Claims;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController : BaseController
{
    private readonly DataContext _context;
    public AccountController(DataContext context) : base(context)
    {
        _context = context;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAccount()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        return Ok(currentUser);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDTO dto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        currentUser.FirstName = dto.FirstName;
        currentUser.LastName = dto.LastName;

        _context.Users.Update(currentUser);
        await _context.SaveChangesAsync();

        return Ok("User updated successfully.");
    }
}
