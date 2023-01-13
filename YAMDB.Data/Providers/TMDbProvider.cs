using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;
using YAMDB.Data.Models.TMDb;

namespace YAMDB.Data.Providers;

public class TMDbProvider
{
    private static readonly string baseUrl = "https://api.themoviedb.org/3";
    private readonly RestClient client = new(baseUrl);

    public TMDbProvider()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets<TMDbProvider>();
        Configuration = builder.Build();
    }

    private static IConfiguration? Configuration { get; set; }

    public async Task<List<Genre>> GetGenres()
    {
        try
        {
            if (Configuration == null)
            {
                throw new Exception("Configuration is null");
            }
            else if (Configuration["THEMOVIEDB_API_KEY"] == null)
            {
                throw new Exception("Missing API key for TheMovieDb.");
            }

            var endpoint =
                $"{baseUrl}/genre/movie/list?api_key={Configuration["THEMOVIEDB_API_KEY"]}&language=en-US";
            var request = new RestRequest(endpoint);
            var response = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(response.Content))
            {
                throw new Exception("Response content is empty.");
            }

            var result = JsonSerializer.Deserialize<MovieGenreResponse>(response.Content);
            if (result == null)
            {
                throw new Exception("Failed to deserialize response.");
            }

            return result.Genres;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<Cast>> GetMovieCast(int movieId)
    {
        try
        {
            if (Configuration == null)
            {
                throw new Exception("Configuration is null");
            }
            else if (Configuration["THEMOVIEDB_API_KEY"] == null)
            {
                throw new Exception("Missing API key for TheMovieDb.");
            }

            var endpoint =
                $"{baseUrl}/movie/{movieId}/credits?api_key={Configuration["THEMOVIEDB_API_KEY"]}&language=en-US";
            var request = new RestRequest(endpoint);
            var response = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(response.Content))
            {
                throw new Exception("Response content is empty.");
            }

            var result = JsonSerializer.Deserialize<MovieCastResponse>(response.Content);
            if (result == null)
            {
                throw new Exception("Failed to deserialize response.");
            }

            return result.Cast;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<TopMovie>> GetTopMovies(int? page = null)
    {
        var movies = new List<TopMovie>();
        try
        {
            var genres = (await GetGenres()).ToDictionary(k => k.Id, v => v.Name ?? string.Empty);
            if (Configuration == null)
            {
                throw new Exception("Configuration is null");
            }
            else if (Configuration["THEMOVIEDB_API_KEY"] == null)
            {
                throw new Exception("Missing API key for TheMovieDb.");
            }

            var endpoint =
                $"{baseUrl}/movie/top_rated?api_key={Configuration["THEMOVIEDB_API_KEY"]}&language=en-US&page={page ?? 1}";
            var request = new RestRequest(endpoint);
            var response = await client.ExecuteAsync(request);
            if (string.IsNullOrEmpty(response.Content))
            {
                return movies;
            }

            var result = JsonSerializer.Deserialize<TopRatedMoviesResponse>(response.Content);
            if (result == null)
            {
                return movies;
            }

            movies.AddRange(result.Results);

            #region Load More Pages

            if (page == null)
            {
                Parallel.For(2, 11, pg =>
                {
                    endpoint =
                        $"{baseUrl}/movie/top_rated?api_key={Configuration["THEMOVIEDB_API_KEY"]}&language=en-US&page={pg}";
                    request = new RestRequest(endpoint);
                    response = client.ExecuteAsync(request).Result;
                    if (string.IsNullOrEmpty(response.Content))
                    {
                        return;
                    }

                    result = JsonSerializer.Deserialize<TopRatedMoviesResponse>(response.Content);
                    if (result == null)
                    {
                        return;
                    }

                    movies.AddRange(result.Results);
                });
            }

            #endregion

            return movies;
        }
        catch (Exception)
        {
            throw;
        }
    }
}