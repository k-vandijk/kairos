using kairos_api.DTOs.TimecapsuleDTOs;
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
    private readonly ICapsuleService _timecapsuleService;

    public CapsuleController(IUnitOfWork unitOfWork, ICapsuleService timecapsuleService) : base(unitOfWork)
    {
        _timecapsuleService = timecapsuleService;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetTimecapsules()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var timecapsules = await _timecapsuleService.GetTimecapsulesForUserAsync(currentUser);
        return Ok(timecapsules);
    }

    [HttpGet("get/{timecapsuleId}")]
    public async Task<IActionResult> GetTimecapsule([FromBody] GetTimecapsuleDTO dto, [FromRoute] Guid timecapsuleId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        try
        {
            var capsule = await _timecapsuleService.GetTimecapsuleForUserAsync(currentUser, timecapsuleId, dto);
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
    public async Task<IActionResult> CreateTimecapsule([FromBody] CreateTimecapsuleDTO dto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = await _timecapsuleService.CreateTimecapsuleForUserAsync(currentUser, dto);
        return Ok(result);
    }
}
