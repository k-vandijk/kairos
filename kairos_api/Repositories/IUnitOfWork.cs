using kairos_api.Repositories.TimecapsuleRepository;
using kairos_api.Repositories.UserRepository;

namespace kairos_api.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITimecapsuleRepository Timecapsules { get; }
    Task<int> CompleteAsync();
}
