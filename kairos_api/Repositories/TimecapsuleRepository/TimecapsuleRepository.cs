using kairos_api.Entities;

namespace kairos_api.Repositories.TimecapsuleRepository;

public class TimecapsuleRepository : Repository<Timecapsule>, ITimecapsuleRepository
{
    public TimecapsuleRepository(DataContext context) : base(context)
    {
    }
}
