namespace YAMDB.Models;

public class Movies
{
    public int Id { get; set; }
    public Guid UUID { get; set; } = Guid.NewGuid();

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Genres { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? ReleaseDate { get; set; }
    
    public int TheMovieDbID { get; set; }

    public virtual ICollection<MovieRatings> Ratings { get; set; }
}