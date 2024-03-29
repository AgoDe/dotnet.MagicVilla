using System.Linq.Expressions;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using MagicVilla_VillaAPI.Repository.IRepository;

namespace MagicVilla_VillaAPI.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        this.dbSet = _db.Set<T>();
    }
    
    public async Task<List<T>> GetAll(Expression<Func<T,bool>> filter = null, string? includeProperties = null, int pageSize = 3, int pageNumber = 1)
    {
        IQueryable<T> query = dbSet;
        if (filter != null )
        {
            query = query.Where(filter);
        }

        if (pageSize > 0)
        {
            if (pageSize > 100)
            {
                pageSize = 100;
            }

            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }
        
        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return await query.ToListAsync();
    }

    public async Task<T> Get(Expression<Func<T,bool>>? filter = null, bool tracked = true, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        
        if (filter != null )
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task Create(T entity)
    {
        await dbSet.AddAsync(entity);
        await Save();
    }

    public async Task Remove(T entity)
    { 
        dbSet.Remove(entity);
        await Save();

    }

    public async Task Save()
    {
        await _db.SaveChangesAsync();
    }
}