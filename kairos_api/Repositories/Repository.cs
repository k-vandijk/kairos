using kairos_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace kairos_api.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly DataContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(DataContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.Where(x => x.IsActive).ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = false;
        _dbSet.Update(entity);
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _dbSet.Where(x => x.IsActive).AsQueryable();
    }
}