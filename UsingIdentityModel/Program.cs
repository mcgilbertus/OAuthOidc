using IdentityModel.Client;

const string serverUrl = "https://demo.duendesoftware.com";
const string discoveryUrl = ".well-known/openid-configuration";

// Get discovery document using HttpClient extended by IdentityModel
var client = new HttpClient();
var discoveryDoc = await client.GetDiscoveryDocumentAsync($"{serverUrl}/{discoveryUrl}");
if (discoveryDoc.IsError)
    throw new ApplicationException(discoveryDoc.Error);

Console.WriteLine($"Discovery doc: {discoveryDoc.Json}");
Console.WriteLine();

var grantTypes = discoveryDoc.TryGetString("grant_types_supported");
Console.WriteLine($"Authentication flows supported: {string.Join(",",grantTypes)}");
