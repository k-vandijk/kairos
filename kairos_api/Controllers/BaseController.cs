using kairos_api.Entities;
using kairos_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kairos_api.Controllers;

public class BaseController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public BaseController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        string email = User.FindFirst("email")?.Value;
        if (email == null)
        {
            return null;
        }

        var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        return user;
    }
}
