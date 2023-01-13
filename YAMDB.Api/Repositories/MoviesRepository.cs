using Microsoft.EntityFrameworkCore;
using YAMDB.Contexts;
using YAMDB.Models;

namespace YAMDB.Api.Repositories;

public class MoviesRepository : RepositoryBase<Movies>, IMoviesRepository
{
    public MoviesRepository(YAMDBContext context) : base(context)
    {
        _context = context;
    }

    private YAMDBContext _context { get; set; }

    public IQueryable<Movies> GetAll()
    {
        return _context
                .Movies
                .Include(m => m.Ratings)
                .Include(m => m.Actors)
                .Skip(0)
                .Take(10)
            ;
    }
}