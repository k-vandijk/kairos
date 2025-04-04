using kairos_api.DTOs.TimecapsuleDTOs;
using kairos_api.Entities;

namespace kairos_api.Services.CapsuleService;

public interface ICapsuleService
{
    Task<IEnumerable<TimecapsuleDTO>> GetTimecapsulesForUserAsync(User user);
    Task<TimecapsuleDTO> GetTimecapsuleForUserAsync(User user, Guid timecapsuleId, GetTimecapsuleDTO dto);
    Task<string> CreateTimecapsuleForUserAsync(User user, CreateTimecapsuleDTO dto);
}
