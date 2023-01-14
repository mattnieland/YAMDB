using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using YAMDB.Api.Repositories;
using YAMDB.Models;

namespace YAMDB.Api.Controllers;

/// <summary>
///     Endpoints to create/retrieve/update/delete actors
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class ActorsController : ControllerBase
{
    private readonly IActorsRepository _actorsRepository;

    /// <summary>
    ///     The controller for actor endpoints
    /// </summary>
    /// <param name="actorsRepository">A repository for working with actors</param>
    public ActorsController(IActorsRepository actorsRepository)
    {
        _actorsRepository = actorsRepository;
    }

    /// <summary>
    ///     Delete an actor
    /// </summary>
    /// <param name="uuid">The unique GUID for the actor</param>
    /// <returns>204 Content</returns>
    [HttpDelete]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:actors")]
    public IActionResult Delete(Guid uuid)
    {
        try
        {
            // check that the object exists
            var existingActor = _actorsRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingActor == null)
            {
                return NotFound();
            }

            // delete and save
            _actorsRepository.DeleteAsync(existingActor);
            _actorsRepository.SaveAsync();
            return NoContent();
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    ///     Retrieve all actors
    /// </summary>
    /// <returns>A list of actors</returns>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, "List of actors", typeof(IEnumerable<Actors>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    public IActionResult Get()
    {
        try
        {
            var actors = _actorsRepository.FindAll();
            return new OkObjectResult(actors);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    ///     Get a specific actor
    /// </summary>
    /// <param name="uuid">The unique GUID for the actor</param>
    /// <returns>An actor object</returns>
    [HttpGet("{uuid:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "An actor object", typeof(Actors))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    public IActionResult Get(Guid uuid)
    {
        try
        {
            // check that the object exists
            var existingActor = _actorsRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingActor == null)
            {
                return NotFound();
            }

            return new OkObjectResult(existingActor);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    ///     Get actors by a movie
    /// </summary>
    /// <param name="movieId">The unique GUID for the movie</param>
    /// <returns>A list of actors</returns>
    [HttpGet("by-movie/{movieId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of actors", typeof(IEnumerable<Actors>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    public IActionResult GetByMovieId(Guid movieId)
    {
        try
        {
            var actors = _actorsRepository.GetByMovieId(movieId);
            return new OkObjectResult(actors);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    ///     Create an actor
    /// </summary>
    /// <param name="actor">An actor object</param>
    /// <returns>An actor object</returns>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The actor already exists")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:actors")]
    public IActionResult Post([FromBody] Actors actor)
    {
        try
        {
            // check that it at least has an id
            if (actor.Id == 0)
            {
                return BadRequest();
            }

            // check that the object exists
            var existingActor = _actorsRepository.FindByCondition(a => a.UUID == actor.UUID).FirstOrDefault();
            if (existingActor != null)
            {
                return Conflict();
            }

            existingActor = _actorsRepository.FindById(actor.Id);
            if (existingActor != null)
            {
                return Conflict();
            }

            // create and save
            _actorsRepository.CreateAsync(actor);
            _actorsRepository.SaveAsync();
            return new OkObjectResult(actor);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    ///     Update an actor object
    /// </summary>
    /// <param name="uuid">The unique GUID for the actor</param>
    /// <param name="actor">The new actor object</param>
    /// <returns>201 Created</returns>
    [HttpPut("{uuid:guid}")]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The actor does not exist")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:actors")]
    public IActionResult Put(Guid uuid, [FromBody] Actors actor)
    {
        try
        {
            // check that it at least has an id
            if (actor.Id == 0)
            {
                return BadRequest();
            }

            // check that the object exists
            var existingActor = _actorsRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingActor == null)
            {
                return NotFound();
            }

            // update and save
            _actorsRepository.UpdateAsync(actor);
            _actorsRepository.SaveAsync();
            return new ObjectResult(actor) {StatusCode = StatusCodes.Status201Created};
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// Advanced search for actors
    /// </summary>
    /// <param name="filter">The filter parameters</param>
    /// <returns>A list of actors</returns>
    [HttpPost("search")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of actors", typeof(IEnumerable<Actors>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    public IActionResult Search([FromBody] FilterDTO filter)
    {
        try
        {
            var actors = _actorsRepository.Search(filter);
            return new OkObjectResult(actors);
        }
        catch (Exception)
        {
            throw;
        }
    }
}