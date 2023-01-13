using AutoMapper;
using YAMDB.Data.Models;
using YAMDB.Data.Models.TMDb;

namespace YAMDB.Data.MappingProfiles;

public class ActorMapping : Profile
{
    public ActorMapping()
    {
        CreateMap<Cast, Actors>()
            .ForMember(c => c.Name, m => m.MapFrom(e => e.Name))
            .ForMember(c => c.ImageUrl, m => m.MapFrom(e => e.ProfilePath))
            .ForMember(c => c.TheMovieDbID, m => m.MapFrom(e => e.Id))
            ;
    }
}