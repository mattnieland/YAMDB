namespace YAMDB.Models;

public class ActorsInMovies
{
    public int ActorId { get; set; }

    public Actors? Actor { get; set; }

    public int MovieId { get; set; }

    public Movies? Movie { get; set; }

    public string? CharacterName { get; set; }
}