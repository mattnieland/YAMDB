﻿using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YAMDB.Api.Repositories;
using YAMDB.Models;

namespace YAMDB.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActorsController : ControllerBase
{
    private readonly IActorsRepository _actorsRepository;

    public ActorsController(IActorsRepository actorsRepository)
    {
        _actorsRepository = actorsRepository;
    }

    // DELETE api/<ActorController>/5
    [HttpDelete("{uuid:guid}")]
    [Authorize("write:actors")]
    public IActionResult Delete(Guid uuid)
    {
        try
        {
            var existingActor = _actorsRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingActor == null)
            {
                return NotFound();
            }

            _actorsRepository.DeleteAsync(existingActor);
            _actorsRepository.SaveAsync();
            return NoContent();
        }
        catch (Exception)
        {
            throw;
        }
    }

    // GET: api/<ActorController>
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return new OkObjectResult(_actorsRepository.GetAll());
        }
        catch (Exception)
        {
            throw;
        }
    }

    // GET api/<ActorController>/5
    [HttpGet("{uuid:guid}")]
    public IActionResult Get(Guid uuid)
    {
        try
        {
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

    // POST api/<ActorController>
    [HttpPost]
    [Authorize("write:actors")]
    public IActionResult Post([FromBody] string value)
    {
        try
        {
            var actor = JsonSerializer.Deserialize<Actors>(value);
            if (actor == null || actor.Id == 0)
            {
                return BadRequest();
            }

            var existingActor = _actorsRepository.FindByCondition(a => a.UUID == actor.UUID).FirstOrDefault();
            if (existingActor != null)
            {
                return Conflict();
            }

            _actorsRepository.CreateAsync(actor);
            _actorsRepository.SaveAsync();
            return new OkObjectResult(actor);
        }
        catch (Exception)
        {
            throw;
        }
    }

    // PUT api/<ActorController>/5
    [HttpPut("{uuid:guid}")]
    [Authorize("write:actors")]
    public IActionResult Put(Guid uuid, [FromBody] string value)
    {
        try
        {
            var existingActor = _actorsRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingActor == null)
            {
                return NotFound();
            }

            var actor = JsonSerializer.Deserialize<Actors>(value);
            if (actor == null || actor.Id == 0)
            {
                return BadRequest();
            }

            _actorsRepository.UpdateAsync(actor);
            _actorsRepository.SaveAsync();
            return new ObjectResult(actor) {StatusCode = StatusCodes.Status201Created};
        }
        catch (Exception)
        {
            throw;
        }
    }
}