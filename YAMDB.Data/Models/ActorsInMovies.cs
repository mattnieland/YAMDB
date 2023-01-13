namespace YAMDB.Data.Models;

public class ActorsInMovies
{
    public int Id { get; set; }

    public int ActorId { get; set; }

    public int MovieId { get; set; }

    public string? CharacterName { get; set; }

    public virtual Actors? Actor { get; set; }

    public virtual Movies? Movie { get; set; }
}