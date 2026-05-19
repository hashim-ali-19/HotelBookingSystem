using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Repositories;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly HotelBookingDbContext DbContext;
    protected readonly DbSet<T> DbSet;

    public EfRepository(HotelBookingDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
    }

    public virtual Task<List<T>> GetAllAsync()
    {
        return DbSet.AsNoTracking().OrderByDescending(entity => entity.CreatedAt).ToListAsync();
    }

    public virtual Task<T?> GetByIdAsync(int id)
    {
        return DbSet.FindAsync(id).AsTask();
    }

    public virtual async Task AddAsync(T entity)
    {
        DbSet.Add(entity);
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        DbSet.Update(entity);
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync();
    }
}
