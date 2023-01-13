using AutoMapper;
using YAMDB.Data.Models;
using YAMDB.Data.Models.TMDb;
using YAMDB.Data.Providers;
using Mapper = YAMDB.Data.Providers.Mapper;

namespace YAMDB.Data.MappingProfiles;

public class MovieMapping : Profile
{
    private readonly Dictionary<int, string?> genreDictionary;

    //private readonly IMapper mapper;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly TMDbProvider provider;

    public MovieMapping()
    {
        //mapper = Mapper.CreateMapper();
        provider = new TMDbProvider();
        genreDictionary = provider.GetGenres().Result.ToDictionary(k => k.Id, v => v.Name);

        CreateMap<TopMovie, Movies>()
            .ForMember(tm => tm.Description, m => m.MapFrom(e => e.Overview))
            .ForMember(tm => tm.ImageUrl, m => m.MapFrom(e => e.PosterPath))
            .ForMember(tm => tm.ReleaseDate, m => m.MapFrom(e => MapReleaseDate(e.ReleaseDate)))
            .ForMember(tm => tm.Genres, m => m.MapFrom(e => MapGenres(e.GenreIds)))
            .ForMember(tm => tm.TheMovieDbID, m => m.MapFrom(e => e.Id))
            //.ForMember(tm => tm.Actors, m => m.MapFrom(e => MapActors(e.Id)))
            ;
    }

    //private List<ActorsInMovies> MapActors(int movieId)
    //{
    //    var actors = new List<ActorsInMovies>();
    //    var cast = provider.GetMovieCast(movieId).Result;
    //    foreach (var castMember in cast)
    //    {
    //        var actor = mapper.Map<Actors>(castMember);
    //        actors.Add(new ActorsInMovies
    //        {
    //            ActorId = actor.TheMovieDbID
    //        });
    //    }

    //    return actors;
    //}

    private List<string> MapGenres(List<int> genres)
    {
        var result = new List<string>();

        if (genreDictionary == null)
        {
            throw new Exception("Genre dictionary is null.");
        }

        foreach (var genre in genres)
        {
            if (genreDictionary.ContainsKey(genre))
            {
                result.Add(genreDictionary[genre]!);
            }
        }

        return result;
    }

    private static DateTime? MapReleaseDate(string? releaseDate)
    {
        if (releaseDate is null)
        {
            return null;
        }

        return DateTime.Parse(releaseDate);
    }
}