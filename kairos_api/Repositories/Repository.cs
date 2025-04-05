using kairos_api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace kairos_api.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Repository(DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.Where(x => x.IsActive).ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task AddAsync(TEntity entity)
    {
        Guid? userId = GetUserId();
        entity.CreatedBy = userId ?? Guid.Empty;
        await _dbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        Guid? userId = GetUserId();
        entity.UpdatedBy = userId ?? Guid.Empty;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        Guid? userId = GetUserId();
        entity.UpdatedBy = userId ?? Guid.Empty;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = false;
        _dbSet.Update(entity);
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _dbSet.Where(x => x.IsActive).AsQueryable();
    }

    private Guid? GetUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdString, out var userId) ? userId : null;
    }
}
