using kairos_api.DTOs.CapsuleDTOs;
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

    public async Task<IEnumerable<CapsuleDTO>> GetCapsulesForUserAsync(User user)
    {
        return await _unitOfWork.Capsules.GetQueryable()
            .Where(tc => tc.UserId == user.Id)
            .Select(tc => new CapsuleDTO
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

    public async Task<CapsuleDTO> GetCapsuleForUserAsync(User user, Guid capsuleId, GetCapsuleDTO dto)
    {
        var capsule = await _unitOfWork.Capsules.GetQueryable()
            .FirstOrDefaultAsync(tc => tc.Id == capsuleId && tc.UserId == user.Id);

        if (capsule == null)
        {
            throw new KeyNotFoundException("Capsule not found.");
        }

        // If the capsules open date is in the future, throw an exception.
        if (capsule.DateToOpen > DateTime.Now)
        {
            throw new InvalidOperationException("Capsule is not yet open.");
        }

        // Check geolocation if available.
        if (capsule.Latitude != null && capsule.Longitude != null)
        {
            var distance = _geolocationService.CalculateDistance(
                (double)capsule.Latitude,
                (double)capsule.Longitude,
                (double)dto.Latitude,
                (double)dto.Longitude);

            if (distance > 1000)
            {
                throw new InvalidOperationException("Capsule is out of range.");
            }
        }

        return new CapsuleDTO
        {
            Id = capsule.Id,
            UserId = capsule.UserId,
            Content = capsule.Content,
            DateToOpen = capsule.DateToOpen,
            Latitude = capsule.Latitude ?? 0,
            Longitude = capsule.Longitude ?? 0,
            CreatedAt = capsule.CreatedAt
        };
    }

    public async Task<string> CreateCapsuleForUserAsync(User user, CreateCapsuleDTO dto)
    {
        var capsule = new Capsule
        {
            UserId = user.Id,
            Content = dto.Content,
            DateToOpen = dto.DateToOpen.ToUniversalTime(),
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        await _unitOfWork.Capsules.AddAsync(capsule);
        await _unitOfWork.CompleteAsync();

        return "Capsule created successfully.";
    }
}
