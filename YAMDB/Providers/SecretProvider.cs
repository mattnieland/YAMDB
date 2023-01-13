using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace YAMDB.Providers;

public class SecretProviders
{
    private static readonly string baseUrl = "https://api.doppler.com/v3";
    private static readonly string project = "yamdb";

    private static string config =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? "dev" : "prod";

    private static readonly RestClient client = new(baseUrl);

    static SecretProviders()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets<SecretProviders>();
        Configuration = builder.Build();
    }

    public static bool SecretsLoaded => Environment.GetEnvironmentVariable("DOPPLER_PROJECT") != null &&
                                        Environment.GetEnvironmentVariable("DOPPLER_CONFIG") != null &&
                                        Environment.GetEnvironmentVariable("DOPPLER_ENVIRONMENT") != null;

    private static IConfiguration? Configuration { get; set; }

    public static void LoadSecrets()
    {
        if (Environment.GetEnvironmentVariable("DOPPLER_PROJECT") != null
            && Environment.GetEnvironmentVariable("DOPPLER_CONFIG") != null
            && Environment.GetEnvironmentVariable("DOPPLER_ENVIRONMENT") != null)
        {
            return;
        }

#if DEBUG
        config = "local";
#endif
        var endpoint =
            $"configs/config/secrets/download?project={project}&config={config}&format=json&include_dynamic_secrets=true&dynamic_secrets_ttl_sec=1800";
        var request = new RestRequest(endpoint);
        var token = Configuration!["DOPPLER_TOKEN"] ?? Environment.GetEnvironmentVariable("DOPPLER_TOKEN");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", $"Basic {token}");
        var response = client.Execute(request);
        if (response.Content != null)
        {
            var secretsDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content);
            if (secretsDictionary != null)
            {
                foreach (var secret in secretsDictionary)
                {
                    Environment.SetEnvironmentVariable(secret.Key, secret.Value);
                }
            }
        }
    }
}