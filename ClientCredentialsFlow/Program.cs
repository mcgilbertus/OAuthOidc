using System.Text.Json;
using System.Text.Json.Nodes;
using IdentityModel.Client;

/// Get an access token from the identity server using ClientCredentials flow

const string serverUrl = "https://demo.duendesoftware.com";
const string discoveryUrl = ".well-known/openid-configuration";

Console.WriteLine("Using IdentityModel.Client");
var httpClient = new HttpClient();
await PrintTokenIdentityModel(httpClient);
Console.WriteLine();

Console.WriteLine("Using vanilla HttpClient");
await PrintTokenHttpClient(httpClient);
Console.WriteLine();

return;


async Task PrintTokenIdentityModel(HttpClient client)
{
    // using IdentityModel
    var discoveryDoc = await client.GetDiscoveryDocumentAsync($"{serverUrl}/{discoveryUrl}");
    if (discoveryDoc.IsError)
        throw new ApplicationException(discoveryDoc.Error);

    var tokenEndpoint = discoveryDoc.TokenEndpoint;
    Console.WriteLine($"Token endpoint: {tokenEndpoint}");
    var tokenResponse = await client.RequestClientCredentialsTokenAsync(
        new ClientCredentialsTokenRequest()
        {
            Address = tokenEndpoint,
            ClientId = "m2m",
            ClientSecret = "secret",
            Scope = "api"
        });

    var tokenJson = JsonSerializer.Serialize(tokenResponse.Json, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine($"Token: {tokenJson}");
}

async Task PrintTokenHttpClient(HttpClient client)
{
    // using vanilla HttpClient
    var discoveryStr = await client.GetStringAsync($"{serverUrl}/{discoveryUrl}");
    var discoveryDoc = JsonNode.Parse(discoveryStr);

    var tokenUrl = discoveryDoc!["token_endpoint"]?.ToString();
    var parameters = new Dictionary<string, string>
    {
        { "address", tokenUrl },
        { "client_id", "m2m" },
        { "client_secret", "secret" },
        { "grant_type", "client_credentials" },
        { "scope", "api" }
    };
    
    using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
    request.Headers.Add("Accept", "application/json");
    request.Content = new FormUrlEncodedContent(parameters);
    var tokenResponse = await client.SendAsync(request);
    var tokenStr = await tokenResponse.Content.ReadAsStringAsync();
    
    var tokenJson = JsonSerializer.Serialize(JsonNode.Parse(tokenStr), new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine($"Token: {tokenJson}");
}
