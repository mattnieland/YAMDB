using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace YAMDB.Providers;

/// <summary>
/// A provider class for injecting secrets into
/// Environment variables using Doppler
/// https://www.doppler.com
/// Mimics how secrets are injected in the front end
/// </summary>
public class SecretProviders
{
    private static readonly string baseUrl = "https://api.doppler.com/v3";
    private static readonly string project = "yamdb";

    private static string config =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? "dev" : "prod";

    private static readonly RestClient client = new(baseUrl);

    static SecretProviders()
    {
        // We seed the initial provider token from the dotnet secret manager
        var builder = new ConfigurationBuilder().AddUserSecrets<SecretProviders>();
        Configuration = builder.Build();
    }

    public static bool SecretsLoaded => Environment.GetEnvironmentVariable("DOPPLER_PROJECT") != null &&
                                        Environment.GetEnvironmentVariable("DOPPLER_CONFIG") != null &&
                                        Environment.GetEnvironmentVariable("DOPPLER_ENVIRONMENT") != null;

    private static IConfiguration? Configuration { get; set; }

    public static void LoadSecrets()
    {
        if (SecretsLoaded)
        {
            return;
        }

        // Allows for separate secrets if running local
        // We can set up other trace constants
        // and more specific control
#if DEBUG
        config = "local";
#endif
        var endpoint =
            $"configs/config/secrets/download?project={project}&config={config}&format=json&include_dynamic_secrets=true&dynamic_secrets_ttl_sec=1800";
        var request = new RestRequest(endpoint);
        var token = Configuration!["DOPPLER_TOKEN"];
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