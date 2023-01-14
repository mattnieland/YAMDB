using Microsoft.EntityFrameworkCore;
using YAMDB.Contexts;
using YAMDB.Extensions;
using YAMDB.Models;

namespace YAMDB.Api.Repositories;

/// <summary>
///     A repository for working with actors
/// </summary>
public class ActorsRepository : RepositoryBase<Actors>, IActorsRepository
{
    /// <summary>
    ///     Initialization of the object
    /// </summary>
    /// <param name="context">A DbContext to connect to</param>
    public ActorsRepository(YAMDBContext context) : base(context)
    {
        Context = context;
    }

    private YAMDBContext Context { get; set; }

    /// <summary>
    ///     Get actors by movie id
    /// </summary>
    /// <param name="movieId">The unique GUID for the movie</param>
    /// <returns>A list of actors</returns>
    public IQueryable<Actors> GetByMovieId(Guid movieId)
    {
        var movie = Context.Movies?
            .FirstOrDefault(m => m.UUID == movieId);
        if (movie == null)
        {
            return Enumerable.Empty<Actors>().AsQueryable();
        }

        var actorMovies = Context
            .ActorsInMovies?
            .Include(a => a.Actor)
            .Where(aim => aim.MovieId == movie.Id);

        if (actorMovies == null)
        {
            return Enumerable.Empty<Actors>().AsQueryable();
        }

        return actorMovies.Select(aim => aim.Actor!);
    }

    /// <summary>
    ///     Advanced actor search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of actors</returns>
    public IQueryable<Actors> Search(FilterDTO filter)
    {
        var query = Context
            .Actors!
            .AsQueryable();

        // apply sort & filter
        query = query.ToFilterView(filter);

        return query;
    }
}