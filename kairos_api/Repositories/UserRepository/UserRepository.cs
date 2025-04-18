﻿using kairos_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace kairos_api.Repositories.UserRepository;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DataContext context) : base(context)
    {
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
