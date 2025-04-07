using System.Security.Claims;
using kairos_api.Entities;
using kairos_api.Repositories;

namespace kairos_api.Endpoints;

public abstract class BaseEndpoint
{
    protected async Task<User?> GetCurrentUserAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return null;
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return await unitOfWork.Users.GetByIdAsync(userId);
    }
}
