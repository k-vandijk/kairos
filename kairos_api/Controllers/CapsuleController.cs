using kairos_api.Services.GeolocationService;
using kairos_api.DTOs.TimecapsuleDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kairos_api.Repositories;
using kairos_api.Entities;

namespace kairos_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CapsuleController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeolocationService _geolocationService;
    public CapsuleController(IUnitOfWork unitOfWork, IGeolocationService geolocationService) : base(unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _geolocationService = geolocationService;
    }

    // This method returns all timecapsules created by the current user, but doesn't show the content of the timecapsules.
    [HttpGet("getall")]
    public async Task<IActionResult> GetTimecapsules()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var timecapsules = await _unitOfWork.Timecapsules.GetQueryable()
            .Where(tc => tc.UserId == currentUser.Id)
            .Select(tc => new TimecapsuleDTO
            {
                Id = tc.Id,
                UserId = tc.UserId,
                DateToOpen = tc.DateToOpen,
                Latitude = tc.Latitude ?? 0,
                Longitude = tc.Longitude ?? 0,
                CreatedAt = tc.CreatedAt,
            })
            .ToListAsync();

        return Ok(timecapsules);
    }

    // This method shows the content of a specific timecapsule, but only if it is in range, and the date to open has passed.
    [HttpGet("get/{timecapsuleId}")]
    public async Task<IActionResult> GetTimecapsule([FromBody] GetTimecapsuleDTO dto, [FromRoute] Guid timecapsuleId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var timecapsule = await _unitOfWork.Timecapsules.GetQueryable()
            .FirstOrDefaultAsync(tc => tc.Id == timecapsuleId && tc.UserId == currentUser.Id);
        if (timecapsule == null)
        {
            return NotFound();
        }

        // if the date to open is in the future, return bad request.
        if (timecapsule.DateToOpen > DateTime.Now)
        {
            return BadRequest("Timecapsule is not yet open.");
        }

        // if timecapsule contains lat and lon values, check if it is in range.
        if (timecapsule.Latitude != null && timecapsule.Longitude != null)
        {
            var distance = _geolocationService.CalculateDistance((double)timecapsule.Latitude, (double)timecapsule.Longitude, (double)dto.Latitude, (double)dto.Longitude);
            if (distance > 1000)
            {
                return BadRequest("Timecapsule is out of range.");
            }
        }

        return Ok(new TimecapsuleDTO()
        {
            Id = timecapsule.Id,
            UserId = timecapsule.UserId,
            Content = timecapsule.Content,
            DateToOpen = timecapsule.DateToOpen,
            Latitude = timecapsule.Latitude ?? 0,
            Longitude = timecapsule.Longitude ?? 0,
            CreatedAt = timecapsule.CreatedAt
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTimecapsule([FromBody] CreateTimecapsuleDTO dto)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var timecapsule = new Timecapsule
        {
            UserId = currentUser.Id,

            Content = dto.Content,
            DateToOpen = dto.DateToOpen,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        await _unitOfWork.Timecapsules.AddAsync(timecapsule);
        await _unitOfWork.CompleteAsync();

        return Ok("Timecapsule created successfully.");
    }
}
