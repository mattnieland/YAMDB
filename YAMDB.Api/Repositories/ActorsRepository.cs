using YAMDB.Contexts;
using YAMDB.Models;

namespace YAMDB.Repositories;

public class ActorsRepository : RepositoryBase<Actors>, IActorsRepository
{
    public ActorsRepository(YAMDBContext context) : base(context)
    {
    }
}