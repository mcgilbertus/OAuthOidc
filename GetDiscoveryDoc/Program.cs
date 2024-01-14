using System.Text.Json.Nodes;

const string serverUrl = "https://demo.duendesoftware.com";
const string discoveryUrl = ".well-known/openid-configuration";

// 1. Using httpClient
var httpClient = new HttpClient();
var discoveryStr = await httpClient.GetStringAsync($"{serverUrl}/{discoveryUrl}");
var discoveryDoc = JsonNode.Parse(discoveryStr);
Console.WriteLine(discoveryDoc?.ToString());

var tokenUrl = discoveryDoc["token_endpoint"]?.ToString();
Console.WriteLine($"Token endpoint: {tokenUrl}");
