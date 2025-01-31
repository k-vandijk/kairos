using Data;
using Data.DTOs.TimecapsuleDTOs;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TimecapsuleController : BaseController
{
    private readonly DataContext _context;
    public TimecapsuleController(DataContext context) : base(context)
    {
        _context = context;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetTimecapsules()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var timecapsules = await _context.Timecapsules
            .Where(tc => tc.UserId == currentUser.Id)
            .Select(tc => new TimecapsuleDTO
            {
                Id = tc.Id,
                UserId = tc.UserId,
                Content = tc.Content,
                DateToOpen = tc.DateToOpen,
                Latitude = tc.Latitude ?? 0,
                Longitude = tc.Longitude ?? 0,
                CreatedAt = tc.CreatedAt,
                UpdatedAt = tc.UpdatedAt
            })
            .ToListAsync();

        return Ok(timecapsules);
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

        _context.Timecapsules.Add(timecapsule);
        await _context.SaveChangesAsync();
        return Ok("Timecapsule created successfully.");
    }
}
