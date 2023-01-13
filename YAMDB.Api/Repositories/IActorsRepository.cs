using YAMDB.Models;

namespace YAMDB.Api.Repositories;

public interface IActorsRepository : IRepositoryBase<Actors>
{
    IQueryable<Actors> GetAll();
}