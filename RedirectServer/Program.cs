using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

const string VerificationStateString = "Verification_state";
//between 45 and 128 characters! it *should* be cryptographically random. See https://www.oauth.com/oauth2-servers/pkce/authorization-request/
const string CodeVerifier = "this_is_a_verifier_string_must_be_longer_than_45_characters";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/gettokenfromcode", async (IHttpClientFactory httpClientFactory, string code, string state) =>
    {
        // verifies the state string
        if (state != VerificationStateString)
            throw new BadHttpRequestException("State is wrong!");

        // state string is verified, call Auth Server to exchange the code by the token
        var httpClient = httpClientFactory.CreateClient();
        var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001/.well-known/openid-configuration");
        // constructs the token request
        var authCodeRequest = new AuthorizationCodeTokenRequest()
        {
            Address = discoveryDoc.TokenEndpoint,
            Code = code,
            ClientId = "client_code",
            ClientSecret = "secret",
            CodeVerifier = null,
            RedirectUri = "https://localhost:5002/gettokenfromcode"
        };
        var duendeResponse = await httpClient.RequestAuthorizationCodeTokenAsync(authCodeRequest);
        if (duendeResponse.IsError)
            throw new BadHttpRequestException(duendeResponse.Error);

        // return the entire response, which includes the access and id tokens 
        return duendeResponse;
    })
    .WithName("Get Token from Code");
app.MapGet("/gettokenfromcodepkce", async (IHttpClientFactory httpClientFactory, string code, string state) =>
    {
        if (state != VerificationStateString)
            throw new BadHttpRequestException("State is wrong!");

        var httpClient = httpClientFactory.CreateClient();
        var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001/.well-known/openid-configuration");
        var authCodeRequest = new AuthorizationCodeTokenRequest()
        {
            Address = discoveryDoc.TokenEndpoint,
            Code = code,
            ClientId = "client_pkce",
            ClientSecret = "secret",
            CodeVerifier = CodeVerifier,
            RedirectUri = "https://localhost:5002/gettokenfromcodepkce"
        };
        var duendeResponse = await httpClient.RequestAuthorizationCodeTokenAsync(authCodeRequest);
        if (duendeResponse.IsError)
            throw new BadHttpRequestException(duendeResponse.Error);

        return duendeResponse;
    })
    .WithName("Get Token from Code PKCE");

app.MapGet("/implicit",
    async (HttpContext context) =>
    {
        // token is sent as a 'hash param' in the URL; these parameters are not sent to the server, so we need to
        // manually replace the # by ? and redirect to another URL
        return context.Response.WriteAsync("<html><script>window.location.href=window.location.href.replace('#', '?').replace('implicit', 'authparams')</script></html>");
    });

app.MapGet("/authparams", async (string access_token, string state) =>
    {
        if (state != VerificationStateString)
            throw new BadHttpRequestException("State is wrong!");

        return new { AccessToken = access_token };
    })
    .WithName("Implicit Flow")
    .WithOpenApi();

app.Run();