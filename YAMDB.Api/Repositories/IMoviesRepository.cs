using YAMDB.Models;

namespace YAMDB.Api.Repositories;

public interface IMoviesRepository : IRepositoryBase<Movies>
{
    IQueryable<Movies> GetAll();
}