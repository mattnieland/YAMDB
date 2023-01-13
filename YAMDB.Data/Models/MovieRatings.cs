namespace YAMDB.Data.Models;

public class MovieRatings
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public string? Source { get; set; }

    public double Rating { get; set; }

    public double RatingUpperLimit { get; set; }

    public int TotalReviews { get; set; }
}