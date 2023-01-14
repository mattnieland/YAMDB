using YAMDB.Models;

namespace YAMDB.Api.Repositories;

/// <summary>
///     An abstraction of the actors repository
/// </summary>
public interface IActorsRepository : IRepositoryBase<Actors>
{
    /// <summary>
    ///     Get actors by movie id
    /// </summary>
    /// <param name="movieId">The unique GUID for the movie</param>
    /// <returns></returns>
    IQueryable<Actors> GetByMovieId(Guid movieId);

    /// <summary>
    ///     Advanced actor search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of actors</returns>
    IQueryable<Actors> Search(FilterDTO filter);
}