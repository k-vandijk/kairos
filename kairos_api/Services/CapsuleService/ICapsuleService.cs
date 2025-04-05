using kairos_api.DTOs.CapsuleDTOs;
using kairos_api.Entities;

namespace kairos_api.Services.CapsuleService;

public interface ICapsuleService
{
    Task<IEnumerable<CapsuleDTO>> GetCapsulesForUserAsync(User user);
    Task<CapsuleDTO> GetCapsuleForUserAsync(User user, Guid capsuleId, GetCapsuleDTO dto);
    Task<string> CreateCapsuleForUserAsync(User user, CreateCapsuleDTO dto);
}
