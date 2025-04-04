using kairos_api.Entities;

namespace kairos_api.Repositories.UserRepository;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserByEmailAsync(string email);
}
