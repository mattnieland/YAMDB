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
        modelBuilder.Entity<Movies>();
        modelBuilder.Entity<MovieRatings>().HasOne(m => m.Movie);
        modelBuilder.Entity<Actors>();
        modelBuilder.Entity<ActorsInMovies>().HasKey(a => new { a.ActorId, a.MovieId });
    }
}