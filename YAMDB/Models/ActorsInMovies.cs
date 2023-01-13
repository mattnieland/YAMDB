using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YAMDB.Models;

[PrimaryKey(nameof(ActorId), nameof(MovieId))]
public class ActorsInMovies
{
    public int Id { get; set; }

    [ForeignKey("Actor")]
    [Display(Name = "Actor")]
    public int ActorId { get; set; }

    public virtual Actors? Actor { get; set; }

    [ForeignKey("Movie")]
    [Display(Name = "Movie")]

    public int MovieId { get; set; }

    public virtual Movies? Movie { get; set; }

    public string? CharacterName { get; set; }
}