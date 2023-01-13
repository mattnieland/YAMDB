using Microsoft.EntityFrameworkCore;
using YAMDB.Data.Providers;
using YAMDB.Models;

namespace YAMDB.Contexts;

public class YAMDBContext : DbContext
{
    public DbSet<Movies> Movies { get; set; }

    public DbSet<Actors> Actors { get; set; }
    public DbSet<ActorsInMovies> ActorsInMovies { get; set; }
    public DbSet<MovieRatings> MovieRatings { get; set; }

    protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("YAMDB");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var tmdbProvider = new TMDBProvider();
        var genres = tmdbProvider.GetGenres().Result.ToDictionary(k => k.Id, v => v.Name);
        var topMovies = tmdbProvider.GetTopMovies(1).Result;
        
        #region Seed Movies & Ratings
        if (Environment.GetEnvironmentVariable("MOVIES_SEEDED") != "true")
        {
            var seedMovies = topMovies.Select((tm, index) => new Movies
            {
                Id = index + 1,
                Title = tm.Title,
                Description = tm.Overview,
                ImageUrl = tm.PosterPath,
                TheMovieDbId = tm.Id,
                ReleaseDate = tm.ReleaseDate != null ? DateTime.Parse(tm.ReleaseDate) : null,
                Genres = string.Join(", ", tm.GenreIds.Select(g => genres.ContainsKey(g) ? genres[g] : null).ToList())
            });

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
            });
            #endregion

            modelBuilder.Entity<MovieRatings>().HasData(seedRatings);
            Environment.SetEnvironmentVariable("MOVIES_SEEDED", "true");
        }
        else
        {
            modelBuilder.Entity<Movies>();
            modelBuilder.Entity<MovieRatings>();
        }
        #endregion

        modelBuilder.Entity<MovieRatings>()
            .HasOne(mr => mr.Movie)
            .WithMany(m => m.Ratings)
            .HasForeignKey(mr => mr.MovieId);

        #region Seed Actors & Actors In Movies
        if (Environment.GetEnvironmentVariable("ACTORS_SEEDED") != "true")
        {
            var seedActors = new List<Actors>();
            var seedActorsInMovies = new List<ActorsInMovies>();
            var movieIndex = 1;
            foreach (var movie in topMovies)
            {
                var movieCast = tmdbProvider.GetMovieCast(movie.Id).Result;
                movieCast.RemoveAll(ma => seedActors.Select(sa => sa.TheMovieDbId).Contains(ma.Id));
                seedActors.AddRange(movieCast.Select((cm, index) => new Actors
                {
                    Id = seedActors.Count + index + 1,
                    Name = cm.Name,
                    TheMovieDbId = cm.Id
                }).ToList());

                seedActorsInMovies.AddRange(movieCast.Select((cm, index) => new ActorsInMovies
                {
                    Id = seedActors.Count + index + 1,
                    ActorId = seedActors.Count + index + 1,
                    MovieId = movieIndex,
                    CharacterName = cm.Character
                }).ToList());

                movieIndex++;
            }

            modelBuilder.Entity<Actors>().HasData(seedActors);
            modelBuilder.Entity<ActorsInMovies>().HasData(seedActorsInMovies);
            Environment.SetEnvironmentVariable("ACTORS_SEEDED", "true");
        }
        else
        {
            modelBuilder.Entity<Actors>();
            modelBuilder.Entity<ActorsInMovies>();
        }
        #endregion

        //modelBuilder.Entity<ActorsInMovies>()
        //    .HasOne(am => am.Actor)
        //    .WithMany(a => a.Movies)
        //    .HasForeignKey(am => am.ActorId);
    }
}