using kairos_api.Entities;
using kairos_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace kairos_api.Controllers;

public class BaseController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public BaseController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Guid? GetUserIdAsync()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return null;
        }

        return Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : null;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        Guid? userId = GetUserIdAsync();
        if (userId == null)
        {
            return null;
        }

        var user = await _unitOfWork.Users.GetByIdAsync((Guid)userId);
        return user;
    }
}
