using Microsoft.AspNetCore.Mvc;
using YAMDB.Models;
using YAMDB.Repositories;

namespace YAMDB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActorsController : ControllerBase
{
    private readonly IActorsRepository _actorsRepository;

    public ActorsController(IActorsRepository actorsRepository)
    {
        _actorsRepository = actorsRepository;
    }

    // GET: api/<ActorController>
    [HttpGet]
    public IEnumerable<Actors> Get()
    {
        return _actorsRepository.FindAll();
    }

    // GET api/<ActorController>/5
    [HttpGet("{uuid}")]
    public Actors? Get(Guid uuid)
    {
        return _actorsRepository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
    }

    //// POST api/<ActorController>
    //[HttpPost]
    //public void Post([FromBody] string value)
    //{
    //}

    //// PUT api/<ActorController>/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] string value)
    //{
    //}

    //// DELETE api/<ActorController>/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}