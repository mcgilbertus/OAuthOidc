using System.Text.Json;
using IdentityModel.Client;

const string serverUrl = "https://localhost:5001";
const string discoveryUrl = ".well-known/openid-configuration";

var client = new HttpClient();
var discoveryDoc = await client.GetDiscoveryDocumentAsync($"{serverUrl}/{discoveryUrl}");
if (discoveryDoc.IsError)
    throw new ApplicationException(discoveryDoc.Error);

var tokenEndpoint = discoveryDoc.TokenEndpoint;
Console.WriteLine($"Token endpoint: {tokenEndpoint}");

var tokenResponse = await client.RequestPasswordTokenAsync(
    new PasswordTokenRequest()
    {
        Address = tokenEndpoint,
        ClientId = "client_pwd",
        ClientSecret = "secret",
        UserName = "bob",
        Password = "bob",
        Scope = "scope2"
    });

var tokenJson = JsonSerializer.Serialize(tokenResponse.Json, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine($"Token: {tokenJson}");