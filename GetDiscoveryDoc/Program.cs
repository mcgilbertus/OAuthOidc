using System.Text.Json.Nodes;
using IdentityModel.Client;

const string serverUrl = "https://demo.duendesoftware.com";
const string discoveryUrl = ".well-known/openid-configuration";

Console.WriteLine("Using IdentityModel.Client");
var httpClient = new HttpClient();
await GetDiscoveryDocIdentityModelAsync(httpClient);
Console.WriteLine();

Console.WriteLine("Using vanilla HttpClient");
await GetDiscoveryDocHttpClientAsync(httpClient);
Console.WriteLine();

return;

async Task GetDiscoveryDocIdentityModelAsync(HttpClient httpClient)
{
    var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync($"{serverUrl}/{discoveryUrl}");
    if (discoveryDoc.IsError)
        throw new ApplicationException(discoveryDoc.Error);

    Console.WriteLine($"Discovery doc: {discoveryDoc.Json}");
    Console.WriteLine();
    // as an example, let's print the authentication flows supported
    var grantTypes = discoveryDoc.TryGetString("grant_types_supported");
    Console.WriteLine($"Authentication flows supported: {string.Join(",",grantTypes)}");

}

async Task GetDiscoveryDocHttpClientAsync(HttpClient httpClient)
{
    // Get discovery document using vanilla HttpClient
    var discoveryStr = await httpClient.GetStringAsync($"{serverUrl}/{discoveryUrl}");
    var discoveryDoc = JsonNode.Parse(discoveryStr);
    Console.WriteLine(discoveryDoc?.ToString());
    // as an example, let's print the token endpoint
    var tokenUrl = discoveryDoc["token_endpoint"]?.ToString();
    Console.WriteLine($"Token endpoint: {tokenUrl}");
}
