namespace YAMDB.Data.Models;

public class Movies
{
    public int ID { get; set; }

    public Guid UUID { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public List<string> Genres { get; set; } = new();

    public string? ImageUrl { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public virtual IEnumerable<ActorsInMovies>? Actors { get; set; }

    public int TheMovieDbID { get; set; }
}