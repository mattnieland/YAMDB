namespace YAMDB.Data.Models;

public class Actors
{
    public int ID { get; set; }

    public Guid UUID { get; set; }

    public string? Name { get; set; }

    public string? ImageUrl { get; set; }

    public int TheMovieDbID { get; set; }
}