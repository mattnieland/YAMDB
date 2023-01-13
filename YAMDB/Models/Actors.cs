namespace YAMDB.Models;

public class Actors
{
    public int Id { get; set; }

    public Guid UUID { get; set; } = Guid.NewGuid();

    public string? Name { get; set; }

    public int TheMovieDbID { get; set; }

    public virtual ICollection<Movies> Movies { get; set; }
}