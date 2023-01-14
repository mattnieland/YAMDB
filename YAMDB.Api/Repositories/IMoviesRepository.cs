using YAMDB.Models;

namespace YAMDB.Api.Repositories;

/// <summary>
///     An abstraction of the movies repository
/// </summary>
public interface IMoviesRepository : IRepositoryBase<Movies>
{
    /// <summary>
    ///     Get movies by actor id
    /// </summary>
    /// <param name="actorId"></param>
    /// <returns></returns>
    IQueryable<Movies> GetByActorId(Guid actorId);

    /// <summary>
    ///     Advanced Movie search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of movies</returns>
    IQueryable<Movies> Search(FilterDTO filter);
}