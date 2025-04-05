using kairos_api.Repositories.CapsuleRepository;
using kairos_api.Repositories.UserRepository;

namespace kairos_api.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICapsuleRepository Capsules { get; }
    Task<int> CompleteAsync();
}
