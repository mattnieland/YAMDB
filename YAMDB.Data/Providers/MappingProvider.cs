using AutoMapper;
using YAMDB.Data.MappingProfiles;

namespace YAMDB.Data.Providers;

public class Mapper
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MovieMapping>();
            cfg.AddProfile<ActorMapping>();
        });
        var mapper = config.CreateMapper();
        return mapper;
    }
}