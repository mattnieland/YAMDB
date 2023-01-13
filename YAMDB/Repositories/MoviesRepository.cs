using YAMDB.Contexts;
using YAMDB.Models;

namespace YAMDB.Repositories;

public class MoviesRepository : RepositoryBase<Movies>, IMoviesRepository
{
    public MoviesRepository(YAMDBContext context) : base(context)
    {
    }
}