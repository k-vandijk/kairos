using kairos_api.Repositories.CapsuleRepository;
using kairos_api.Repositories.UserRepository;

namespace kairos_api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    public ICapsuleRepository Capsules { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(DataContext context)
    {
        _context = context;
        Capsules = new kairos_api.Repositories.CapsuleRepository.CapsuleRepository(context);
        Users = new kairos_api.Repositories.UserRepository.UserRepository(context);
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
