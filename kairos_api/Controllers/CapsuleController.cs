using kairos_api.DTOs.CapsuleDTOs;
using kairos_api.Repositories;
using kairos_api.Services.CapsuleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kairos_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CapsuleController : BaseController
{
    private readonly ICapsuleService _capsuleService;

    public CapsuleController(IUnitOfWork unitOfWork, ICapsuleService capsuleService) : base(unitOfWork)
    {
        _capsuleService = capsuleService;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetCapsules()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var capsules = await _capsuleService.GetCapsulesForUserAsync(currentUser);
        return Ok(capsules);
    }

    [HttpGet("get/{capsuleId}")]
    public async Task<IActionResult> GetCapsule([FromBody] GetCapsuleDTO dto, [FromRoute] Guid capsuleId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        try
        {
            var capsule = await _capsuleService.GetCapsuleForUserAsync(currentUser, capsuleId, dto);
            return Ok(capsule);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCapsule([FromBody] CreateCapsuleDTO dto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = await _capsuleService.CreateCapsuleForUserAsync(currentUser, dto);
        return Ok(result);
    }
}
