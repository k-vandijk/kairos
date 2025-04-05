using kairos_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace kairos_api.Repositories.UserRepository;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DataContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
    {
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.IsActive && u.Email.ToLower() == email.ToLower());
    }
}
