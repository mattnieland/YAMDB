using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YAMDB.Contexts;

namespace YAMDB.Api.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    public RepositoryBase(YAMDBContext context)
    {
        _context = context;
    }

    private YAMDBContext _context { get; set; }

    public IQueryable<T> FindAll()
    {
        return _context.Set<T>().AsNoTracking();
    }

    public T? FindById(int id)
    {
        return _context.Set<T>().Find(id);
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression).AsNoTracking();
    }

    public async void CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public void UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async void SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}