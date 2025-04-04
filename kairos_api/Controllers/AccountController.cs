using kairos_api.DTOs.AccountDTOs;
using kairos_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kairos_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountController(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        currentUser.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(currentUser);
        await _unitOfWork.CompleteAsync();

        return Ok("User updated successfully.");
    }
}
