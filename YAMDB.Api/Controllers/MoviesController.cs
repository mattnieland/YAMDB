using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using YAMDB.Api.Repositories;
using YAMDB.Models;

namespace YAMDB.Api.Controllers;

/// <summary>
///     Endpoints to create/retrieve/update/delete movies
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class MoviesController : ControllerBase
{
    private readonly ILogger<MoviesController> _logger;
    private readonly IMoviesRepository _moviesRepository;

    /// <summary>
    ///     The controller for movie endpoints
    /// </summary>
    /// <param name="moviesRepository">A repository for working with movies</param>
    /// <param name="logger">Our default logger</param>
    /// ///
    public MoviesController(IMoviesRepository moviesRepository, ILogger<MoviesController> logger)
    {
        _moviesRepository = moviesRepository;
        _logger = logger;
    }

    /// <summary>
    ///     Delete a movie
    /// </summary>
    /// <param name="uuid">The unique GUID for the movie</param>
    /// <returns>204 Content</returns>
    [HttpDelete]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:movies")]
    public IActionResult Delete(Guid uuid)
    {
        try
        {
            // check that the object exists
            var existingMovie = _moviesRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingMovie == null)
            {
                return NotFound();
            }

            // delete and save
            _moviesRepository.DeleteAsync(existingMovie);
            _moviesRepository.SaveAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    ///     Get a specific movie
    /// </summary>
    /// <param name="uuid">The unique GUID for the movie</param>
    /// <returns>A movie object</returns>
    [HttpGet("{uuid:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "A movie object", typeof(Actors))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    public IActionResult Get(Guid uuid)
    {
        try
        {
            // check that the object exists
            var existingMovie = _moviesRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingMovie == null)
            {
                return NotFound();
            }

            return new OkObjectResult(existingMovie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    ///     Retrieve all movies
    /// </summary>
    /// <returns>A list of movies</returns>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, "List of movies", typeof(IEnumerable<Actors>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    public IActionResult Get()
    {
        try
        {
            var movies = _moviesRepository.FindAll();
            return new OkObjectResult(movies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    ///     Get movies by an actor
    /// </summary>
    /// <param name="actorId">The unique GUID for the actor</param>
    /// <returns>A list of movies</returns>
    [HttpGet("by-actor/{actorId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of movies", typeof(IEnumerable<Movies>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    public IActionResult GetByActorId(Guid actorId)
    {
        try
        {
            var movies = _moviesRepository.GetByActorId(actorId);
            return new OkObjectResult(movies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    ///     Create a movie
    /// </summary>
    /// <param name="movie">A movie object</param>
    /// <returns>A movie object</returns>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The movie already exists")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:movies")]
    public IActionResult Post([FromBody] Movies movie)
    {
        try
        {
            // check that it at least has an id
            if (movie.Id == 0)
            {
                return BadRequest();
            }

            // check that the object exists
            var existingMovie = _moviesRepository.FindByCondition(a => a.UUID == movie.UUID).FirstOrDefault();
            if (existingMovie != null)
            {
                return Conflict();
            }

            existingMovie = _moviesRepository.FindById(movie.Id);
            if (existingMovie != null)
            {
                return Conflict();
            }

            // create and save
            _moviesRepository.CreateAsync(movie);
            _moviesRepository.SaveAsync();
            return new OkObjectResult(movie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    ///     Update a movie object
    /// </summary>
    /// <param name="uuid">The unique GUID for the movie</param>
    /// <param name="value">The new movie object</param>
    /// <returns>201 Created</returns>
    [HttpPut("{uuid:guid}")]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The movie does not exist")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:movies")]
    public IActionResult Put(Guid uuid, [FromBody] string value)
    {
        try
        {
            // check that the object exists
            var existingMovie = _moviesRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingMovie == null)
            {
                return NotFound();
            }

            Movies? movie;
            try
            {
                // deserialize the body and validate it has an id
                movie = JsonSerializer.Deserialize<Movies>(value);
                if (movie == null || movie.Id == 0)
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

            // update and save
            _moviesRepository.UpdateAsync(movie);
            _moviesRepository.SaveAsync();
            return new ObjectResult(movie) {StatusCode = StatusCodes.Status201Created};
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    ///     Advanced search for movies
    /// </summary>
    /// <param name="filter">The filter parameters</param>
    /// <returns>A list of movies</returns>
    [HttpPost("search")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of movies", typeof(IEnumerable<Actors>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    public IActionResult Search([FromBody] FilterDTO filter)
    {
        try
        {
            var movies = _moviesRepository.Search(filter);
            return new OkObjectResult(movies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
}