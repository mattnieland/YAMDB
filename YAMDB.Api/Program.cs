using Microsoft.AspNetCore.ResponseCompression;
using YAMDB.Contexts;
using YAMDB.Data.Providers;
using YAMDB.Models;
using YAMDB.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDbContext<YAMDBContext>();
builder.Services.AddScoped(typeof(IActorsRepository), typeof(ActorsRepository));
builder.Services.AddScoped(typeof(IMoviesRepository), typeof(MoviesRepository));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#if DEBUG
var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<YAMDBContext>();
if (db == null)
{
    throw new Exception("Could not get database context");
}

db.Database.EnsureCreated();

//var tmdbProvider = new TMDBProvider();
//var genres = tmdbProvider.GetGenres().Result.ToDictionary(k => k.Id, v => v.Name);
//var topMovies = tmdbProvider.GetTopMovies(1).Result;

//#region Seed Movies

//var seedMovies = topMovies.Select((tm, index) => new Movies
//{
//    Id = index + 1,
//    Title = tm.Title,
//    Description = tm.Overview,
//    ImageUrl = tm.PosterPath,
//    TheMovieDbId = tm.Id,
//    ReleaseDate = tm.ReleaseDate != null ? DateTime.Parse(tm.ReleaseDate) : null,
//    Genres = string.Join(", ", tm.GenreIds.Select(g => genres.ContainsKey(g) ? genres[g] : null).ToList())
//});
//db.Movies.AddRange(seedMovies);
//db.SaveChanges();

//#endregion

//#region Seed Movie Ratings

//var movieDictionary = db.Movies.ToDictionary(k => k.TheMovieDbId, v => v.Id);
//var ratings = topMovies.Select((tm, index) => new MovieRatings
//{
//    Id = index + 1,
//    MovieId = movieDictionary[tm.Id],
//    Rating = tm.VoteAverage,
//    RatingUpperLimit = 10,
//    TotalReviews = tm.VoteCount,
//    Source = "TheMovieDB"
//});
//db.MovieRatings.AddRange(ratings);
//db.SaveChanges();

//#endregion

//#region Seed Actors & Actors In Movies

//var seedActors = new List<Actors>();
//var seedActorsInMovies = new List<ActorsInMovies>();
//foreach (var movie in topMovies)
//{
//    var movieCast = tmdbProvider.GetMovieCast(movie.Id).Result;
//    movieCast.RemoveAll(ma => seedActors.Select(sa => sa.TheMovieDbId).Contains(ma.Id));
//    seedActors.AddRange(movieCast.Select((cm, index) => new Actors
//    {
//        Id = seedActors.Count + index + 1,
//        Name = cm.Name,
//        TheMovieDbId = cm.Id
//    }).ToList());

//    seedActorsInMovies.AddRange(movieCast.Select((cm, index) => new ActorsInMovies
//    {
//        Id = seedActors.Count + index + 1,
//        ActorId = seedActors.Count + index + 1,
//        MovieId = movieDictionary[movie.Id],
//        CharacterName = cm.Character
//    }).ToList());
//}

//db.Actors.AddRange(seedActors);
//db.SaveChanges();

//db.ActorsInMovies.AddRange(seedActorsInMovies);
//db.SaveChanges();

//#endregion

#endif


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();