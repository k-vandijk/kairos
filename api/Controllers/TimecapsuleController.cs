using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

}
