using kairos_api.Entities;

namespace kairos_api.Repositories.CapsuleRepository;

public class CapsuleRepository : Repository<Capsule>, ICapsuleRepository
{
    public CapsuleRepository(DataContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
    {
    }
}
