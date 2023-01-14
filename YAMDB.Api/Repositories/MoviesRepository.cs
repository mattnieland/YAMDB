using Microsoft.EntityFrameworkCore;
using YAMDB.Contexts;
using YAMDB.Extensions;
using YAMDB.Models;

namespace YAMDB.Api.Repositories;

/// <summary>
///     A repository for working with actors
/// </summary>
public class MoviesRepository : RepositoryBase<Movies>, IMoviesRepository
{
    /// <summary>
    ///     Initialization of the object
    /// </summary>
    /// <param name="context">A DbContext to connect to</param>
    public MoviesRepository(YAMDBContext context) : base(context)
    {
        Context = context;
    }

    private YAMDBContext Context { get; set; }

    /// <summary>
    ///     Get movies by actor id
    /// </summary>
    /// <param name="actorId">The unique GUID for the actor</param>
    /// <returns>A list of movies</returns>
    public IQueryable<Movies> GetByActorId(Guid actorId)
    {
        var actor = Context.Actors?
            .FirstOrDefault(m => m.UUID == actorId);
        if (actor == null)
        {
            return Enumerable.Empty<Movies>().AsQueryable();
        }

        var actorMovies = Context
            .ActorsInMovies?
            .Include(a => a.Movie)
            .Where(aim => aim.ActorId == actor.Id);

        if (actorMovies == null)
        {
            return Enumerable.Empty<Movies>().AsQueryable();
        }

        return actorMovies.Select(aim => aim.Movie!);
    }

    /// <summary>
    ///     Advanced Movie search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of movies</returns>
    public IQueryable<Movies> Search(FilterDTO filter)
    {
        var query = Context
            .Movies!
            .AsQueryable();

        // apply sort & filter
        query = query.ToFilterView(filter);

        return query;
    }
}