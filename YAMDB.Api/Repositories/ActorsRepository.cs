using Microsoft.EntityFrameworkCore;
using YAMDB.Contexts;
using YAMDB.Models;

namespace YAMDB.Api.Repositories;

public class ActorsRepository : RepositoryBase<Actors>, IActorsRepository
{
    public ActorsRepository(YAMDBContext context) : base(context)
    {
        _context = context;
    }

    private YAMDBContext _context { get; set; }

    public IQueryable<Actors> GetAll()
    {
        return _context
            .Actors
            .Include(a => a.Movies)
            .Skip(0)
            .Take(10);
    }
}