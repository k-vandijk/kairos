using kairos_api.DTOs.TimecapsuleDTOs;
using kairos_api.Entities;
using kairos_api.Repositories;
using kairos_api.Services.GeolocationService;
using Microsoft.EntityFrameworkCore;

namespace kairos_api.Services.CapsuleService;

public class CapsuleService : ICapsuleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeolocationService _geolocationService;

    public CapsuleService(IUnitOfWork unitOfWork, IGeolocationService geolocationService)
    {
        _unitOfWork = unitOfWork;
        _geolocationService = geolocationService;
    }

    public async Task<IEnumerable<TimecapsuleDTO>> GetTimecapsulesForUserAsync(User user)
    {
        return await _unitOfWork.Timecapsules.GetQueryable()
            .Where(tc => tc.UserId == user.Id)
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
    }

    public async Task<TimecapsuleDTO> GetTimecapsuleForUserAsync(User user, Guid timecapsuleId, GetTimecapsuleDTO dto)
    {
        var timecapsule = await _unitOfWork.Timecapsules.GetQueryable()
            .FirstOrDefaultAsync(tc => tc.Id == timecapsuleId && tc.UserId == user.Id);

        if (timecapsule == null)
        {
            throw new KeyNotFoundException("Timecapsule not found.");
        }

        // If the timecapsule's open date is in the future, throw an exception.
        if (timecapsule.DateToOpen > DateTime.Now)
        {
            throw new InvalidOperationException("Timecapsule is not yet open.");
        }

        // Check geolocation if available.
        if (timecapsule.Latitude != null && timecapsule.Longitude != null)
        {
            var distance = _geolocationService.CalculateDistance(
                (double)timecapsule.Latitude,
                (double)timecapsule.Longitude,
                (double)dto.Latitude,
                (double)dto.Longitude);

            if (distance > 1000)
            {
                throw new InvalidOperationException("Timecapsule is out of range.");
            }
        }

        return new TimecapsuleDTO
        {
            Id = timecapsule.Id,
            UserId = timecapsule.UserId,
            Content = timecapsule.Content,
            DateToOpen = timecapsule.DateToOpen,
            Latitude = timecapsule.Latitude ?? 0,
            Longitude = timecapsule.Longitude ?? 0,
            CreatedAt = timecapsule.CreatedAt
        };
    }

    public async Task<string> CreateTimecapsuleForUserAsync(User user, CreateTimecapsuleDTO dto)
    {
        var timecapsule = new Timecapsule
        {
            UserId = user.Id,
            Content = dto.Content,
            DateToOpen = dto.DateToOpen,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        await _unitOfWork.Timecapsules.AddAsync(timecapsule);
        await _unitOfWork.CompleteAsync();

        return "Timecapsule created successfully.";
    }
}
