using System.Linq.Expressions;

namespace YAMDB.Repositories;

public interface IRepositoryBase<T>
{
    void CreateAsync(T entity);
    void DeleteAsync(T entity);
    IQueryable<T> FindAll();
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    T? FindById(int id);
    void SaveAsync();
    void UpdateAsync(T entity);
}