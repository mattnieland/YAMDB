using System.Linq.Expressions;

namespace YAMDB.Repositories;

public interface IRepositoryBase<T>
{
    void CreateAsync(T entity);
    void DeleteAsync(T entity);
    IQueryable<T> FindAll();
    T? FindById(int id);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    void SaveAsync();
    void UpdateAsync(T entity);
}