using kairos_api.Repositories.TimecapsuleRepository;
using kairos_api.Repositories.UserRepository;

namespace kairos_api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    public IUserRepository Users { get; }
    public ITimecapsuleRepository Timecapsules { get; }

    public UnitOfWork(DataContext context)
    {
        _context = context;
        Timecapsules = new kairos_api.Repositories.TimecapsuleRepository.TimecapsuleRepository(_context);
        Users = new kairos_api.Repositories.UserRepository.UserRepository(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
