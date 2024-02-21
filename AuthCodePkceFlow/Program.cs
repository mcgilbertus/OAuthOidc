using System.Security.Cryptography;
using System.Text;
using IdentityModel;
using IdentityModel.Client;

const string verificationStateString = "Verification_state";
const string serverUrl = "https://localhost:5001";
const string discoveryUrl = ".well-known/openid-configuration";
//between 45 and 128 characters! it *should* be cryptographically random. See https://www.oauth.com/oauth2-servers/pkce/authorization-request/
const string CodeVerifier = "this_is_a_verifier_string_must_be_longer_than_45_characters";

var httpClient = new HttpClient();
var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync($"{serverUrl}/{discoveryUrl}");
if (discoveryDoc.IsError)
    throw new ApplicationException(discoveryDoc.Error);

if (discoveryDoc == null)
    throw new ApplicationException("Failed to get discovery document");

var reqUrl = new RequestUrl(discoveryDoc.AuthorizeEndpoint!);
var codeUrl = reqUrl.CreateAuthorizeUrl(
    clientId: "client_pkce",
    responseType: OidcConstants.ResponseTypes.Code,
    redirectUri: "https://localhost:5002/gettokenfromcodepkce",
    state: verificationStateString,
    scope: "openid api1.read",
    codeChallenge: GetCodeChallenge(),
    codeChallengeMethod: OidcConstants.CodeChallengeMethods.Sha256
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


string? GetCodeChallenge()
{
    using var sha256 = SHA256.Create();
    var challengeBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(CodeVerifier));
    return Base64Url.Encode(challengeBytes);
}