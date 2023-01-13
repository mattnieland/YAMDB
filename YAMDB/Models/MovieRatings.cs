using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace YAMDB.Models;

public class MovieRatings
{
    public int Id { get; set; }

    [ForeignKey("Movie")]
    [Display(Name = "Movie")]
    public int MovieId { get; set; }

    public virtual Movies? Movie { get; set; }

    public string? Source { get; set; }

    public double Rating { get; set; }

    public double RatingUpperLimit { get; set; }

    public int TotalReviews { get; set; }
}