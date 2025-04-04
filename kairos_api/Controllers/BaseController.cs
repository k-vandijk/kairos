using kairos_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kairos_api.Controllers;

public class BaseController : Controller
{
    private readonly DataContext _context;
    public BaseController(DataContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser> GetCurrentUserAsync()
    {
        string email = User.FindFirst("email")?.Value;
        if (email == null)
        {
            return null;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        if (user == null)
        {
            return null;
        }

        return user;
    }
}
