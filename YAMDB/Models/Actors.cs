using System.Text.Json.Serialization;

namespace YAMDB.Models;

public class Actors
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("uuid")] public Guid UUID { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("theMovieDbId")] public int TheMovieDbId { get; set; }

    [JsonPropertyName("movies")] public virtual ICollection<Movies>? Movies { get; set; }
}