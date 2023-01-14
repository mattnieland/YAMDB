using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YAMDB.Api.Repositories;
using YAMDB.Models;

namespace YAMDB.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMoviesRepository _moviesRepository;

    public MoviesController(IMoviesRepository moviesRepository)
    {
        _moviesRepository = moviesRepository;
    }

    // DELETE api/<MovieController>/5
    // ReSharper disable once RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute
    [HttpDelete]
    [Authorize("write:movies")]
    public IActionResult Delete(Guid uuid)
    {
        try
        {
            var existingMovie = _moviesRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingMovie == null)
            {
                return NotFound();
            }

            _moviesRepository.DeleteAsync(existingMovie);
            _moviesRepository.SaveAsync();
            return NoContent();
        }
        catch (Exception)
        {
            throw;
        }
    }

    // GET: api/<MovieController>
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return new OkObjectResult(_moviesRepository.GetAll());
        }
        catch (Exception)
        {
            throw;
        }
    }

    // GET api/<MovieController>/5
    [HttpGet("{uuid:guid}")]
    public IActionResult Get(Guid uuid)
    {
        try
        {
            var existingMovie = _moviesRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingMovie == null)
            {
                return NotFound();
            }

            return new OkObjectResult(existingMovie);
        }
        catch (Exception)
        {
            throw;
        }
    }

    // POST api/<MovieController>
    [HttpPost]
    [Authorize("write:movies")]
    public IActionResult Post([FromBody] string value)
    {
        try
        {
            var movie = JsonSerializer.Deserialize<Movies>(value);
            if (movie == null || movie.Id == 0)
            {
                return BadRequest();
            }

            var existingMovie = _moviesRepository.FindByCondition(a => a.UUID == movie.UUID).FirstOrDefault();
            if (existingMovie != null)
            {
                return Conflict();
            }

            _moviesRepository.CreateAsync(movie);
            _moviesRepository.SaveAsync();
            return new OkObjectResult(movie);
        }
        catch (Exception)
        {
            throw;
        }
    }

    // PUT api/<MovieController>/5
    // ReSharper disable once RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute
    [HttpPut("{uuid:guid}")]
    [Authorize("write:movies")]
    public IActionResult Put(Guid uuid, [FromBody] string value)
    {
        try
        {
            var existingMovie = _moviesRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingMovie == null)
            {
                return NotFound();
            }

            var movie = JsonSerializer.Deserialize<Movies>(value);
            if (movie == null || movie.Id == 0)
            {
                return BadRequest();
            }

            _moviesRepository.UpdateAsync(movie);
            _moviesRepository.SaveAsync();
            return new ObjectResult(movie) {StatusCode = StatusCodes.Status201Created};
        }
        catch (Exception)
        {
            throw;
        }
    }
}