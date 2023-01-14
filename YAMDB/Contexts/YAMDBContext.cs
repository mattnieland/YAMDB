using Microsoft.EntityFrameworkCore;
using YAMDB.Models;
using YAMDB.Providers;

namespace YAMDB.Contexts;

public class YAMDBContext : DbContext
{
    public DbSet<Movies>? Movies { get; set; }
    public DbSet<Actors>? Actors { get; set; }
    public DbSet<ActorsInMovies>? ActorsInMovies { get; set; }
    public DbSet<MovieRatings>? MovieRatings { get; set; }

    protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("YAMDB");

        // Uncomment to support switching to in memory on dev
        // and SQL Server on production
        //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        //{
        //    optionsBuilder.UseInMemoryDatabase("YAMDB");
        //}
        //else
        //{
        //    var connString = Environment.GetEnvironmentVariable("YAMDB_CONNECTION_STRING");
        //    if (string.IsNullOrEmpty(connString))
        //    {
        //        throw new Exception("Database connection string is not set");
        //    }
        //    optionsBuilder.UseSqlServer(connString);
        //}
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieRatings>();
        modelBuilder.Entity<Movies>();
        modelBuilder.Entity<ActorsInMovies>();

        // support many-to-many relationship between movies and actors
        modelBuilder.Entity<Actors>()
            .HasMany(a => a.Movies)
            .WithMany(p => p.Actors)
            .UsingEntity<ActorsInMovies>(
                j => j
                    .HasOne(aim => aim.Movie)
                    .WithMany(m => m.ActorsInMovies)
                    .HasForeignKey(aim => aim.MovieId),
                j => j
                    .HasOne(aim => aim.Actor)
                    .WithMany(a => a.ActorsInMovies)
                    .HasForeignKey(aim => aim.ActorId),
                j => { j.HasKey(t => new {t.ActorId, t.MovieId}); })
            ;

        // support many-to-many relationship between movies and actors
        modelBuilder.Entity<Movies>()
            .HasMany(a => a.Actors)
            .WithMany(p => p.Movies)
            .UsingEntity<ActorsInMovies>(
                j => j
                    .HasOne(aim => aim.Actor)
                    .WithMany(m => m.ActorsInMovies)
                    .HasForeignKey(aim => aim.ActorId),
                j => j
                    .HasOne(aim => aim.Movie)
                    .WithMany(a => a.ActorsInMovies)
                    .HasForeignKey(aim => aim.MovieId),
                j => { j.HasKey(t => new {t.ActorId, t.MovieId}); })
            ;

        #region Seed the development database

#if DEBUG
        var tmdbProvider = new TMDBProvider();
        var genres = tmdbProvider.GetGenres().Result.ToDictionary(k => k.Id, v => v.Name);
        var topMovies = tmdbProvider.GetTopMovies(1).Result;

        #region Seed Movies & Ratings

        var seedMovies = topMovies.Select((tm, index) => new Movies
        {
            Id = index + 1,
            Title = tm.Title,
            Description = tm.Overview,
            ImageUrl = tm.PosterPath,
            TheMovieDbId = tm.Id,
            ReleaseDate = tm.ReleaseDate != null ? DateTime.Parse(tm.ReleaseDate) : null,
            Genres = string.Join(", ", tm.GenreIds.Select(g => genres.ContainsKey(g) ? genres[g] : null).ToList())
        }).ToList();

        modelBuilder.Entity<Movies>().HasData(seedMovies);

        #region Seed Movie Ratings

        var seedRatings = topMovies.Select((tm, index) => new MovieRatings
        {
            Id = index + 1,
            MovieId = index + 1,
            Rating = tm.VoteAverage,
            RatingUpperLimit = 10,
            TotalReviews = tm.VoteCount,
            Source = "TheMovieDB"
        }).ToList();

        #endregion

        modelBuilder.Entity<MovieRatings>().HasData(seedRatings);

        #endregion

        #region Seed Actors & Actors In Movies

        var seedActors = new List<Actors>();
        var seedActorsInMovies = new List<ActorsInMovies>();
        var movieIndex = 1;
        var actorIndex = 1;
        foreach (var movie in topMovies)
        {
            var movieCast = tmdbProvider.GetMovieCast(movie.Id).Result;
            movieCast.RemoveAll(ma => seedActors.Select(sa => sa.TheMovieDbId).Contains(ma.Id));

            foreach (var castMember in movieCast)
            {
                var actor = new Actors
                {
                    Id = actorIndex,
                    Name = castMember.Name,
                    TheMovieDbId = castMember.Id
                };
                seedActors.Add(actor);

                var actorMovie = new ActorsInMovies
                {
                    ActorId = actorIndex,
                    MovieId = movieIndex,
                    CharacterName = castMember.Character
                };
                seedActorsInMovies.Add(actorMovie);

                actorIndex++;
            }

            movieIndex++;
        }

        modelBuilder.Entity<Actors>().HasData(seedActors);
        modelBuilder.Entity<ActorsInMovies>().HasData(seedActorsInMovies);

        #endregion

#endif

        #endregion
    }
}