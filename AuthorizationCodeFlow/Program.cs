using IdentityModel;
using IdentityModel.Client;

const string verificationStateString = "Verification_state";
const string serverUrl = "https://localhost:5001";
const string discoveryUrl = ".well-known/openid-configuration";

var httpClient = new HttpClient();
var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync($"{serverUrl}/{discoveryUrl}");
if (discoveryDoc.IsError)
    throw new ApplicationException(discoveryDoc.Error);

if (discoveryDoc == null)
    throw new ApplicationException("Failed to get discovery document");

var reqUrl = new RequestUrl(discoveryDoc.AuthorizeEndpoint!);
var codeUrl = reqUrl.CreateAuthorizeUrl(
    clientId: "client_code",
    responseType: OidcConstants.ResponseTypes.Code,
    redirectUri: "https://localhost:5002/gettokenfromcode",
    state: verificationStateString,
    scope: "openid api1.read"
);
var getCodeResponse = await httpClient.GetAsync(codeUrl);
if (!getCodeResponse.IsSuccessStatusCode)
    throw new ApplicationException($"Failed to get authorization code: {getCodeResponse.StatusCode}");

var requestUri = getCodeResponse.RequestMessage.RequestUri.ToString();
Console.WriteLine(requestUri);

var response = await httpClient.GetAsync(requestUri);
if (!response.IsSuccessStatusCode)
    throw new ApplicationException($"Failed to get token: {response.StatusCode}");

Console.WriteLine("Open your browser and navigate to the URL above to login on the auth server and get the token");
