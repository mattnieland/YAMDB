using System;
using Microsoft.AspNetCore.Mvc;
using YAMDB.Models;
using YAMDB.Repositories;

namespace YAMDB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMoviesRepository _moviesRepository;

    public MoviesController(IMoviesRepository moviesRepository)
    {
        _moviesRepository = moviesRepository;
    }

    // GET: api/<MoviesController>
    [HttpGet]
    public IEnumerable<Movies> Get()
    {
        return _moviesRepository.FindAll();
    }

    // GET api/<MoviesController>/5
    [HttpGet("{uuid}")]
    public Movies? Get(Guid uuid)
    {
        return _moviesRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
    }

    //// POST api/<MoviesController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<MoviesController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<MoviesController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}