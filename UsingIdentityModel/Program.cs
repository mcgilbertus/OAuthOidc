using IdentityModel.Client;

const string serverUrl = "https://demo.duendesoftware.com";
const string discoveryUrl = ".well-known/openid-configuration";

var client = new HttpClient();

var discoveryDoc = await client.GetDiscoveryDocumentAsync($"{serverUrl}/{discoveryUrl}");
if (discoveryDoc.IsError)
    throw new ApplicationException(discoveryDoc.Error);

Console.WriteLine($"Discovery doc: {discoveryDoc.Json}");
    