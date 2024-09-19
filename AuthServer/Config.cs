// Config.cs

using System.Security.Claims;
using Duende.IdentityServer.Models;

namespace DuendeIdentityServerwithIn_MemoryStoresandTestUsers1;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource()
            {
                Name = "role",
                UserClaims = new List<string> { "role" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("scope1"),
            new ApiScope("scope2"),
            new ApiScope("api1.access", displayName: "Access to API v1", userClaims: new[]
            {
                "name","email","address"
            }),
            new ApiScope(name: "api1.read", displayName: "api1 read access"),
            new ApiScope(name: "offline_access", displayName: "provides refresh token"),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("api1", "API v1")
            {
                Scopes = { "api1.access" },
                // AllowedAccessTokenSigningAlgorithms = { SecurityAlgorithms.HmacSha256 }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                // client credentials client
                ClientId = "client_id",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                Claims = new List<ClientClaim>
                {
                    new ClientClaim(ClaimTypes.GivenName, "John"),
                    new ClientClaim(ClaimTypes.Email, "john123@demoserver.com")
                },
                // scopes that client has access to
                AllowedScopes = { "api1.read" },
                Enabled = true
            },
            new Client
            {
                // auth code flow client
                ClientId = "client_code",
                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = { new Secret("secret".Sha256()) },
                // scopes that client has access to
                AllowedScopes = { "openid", "api1.read", "api1.access", "profile" },
                RedirectUris = { "https://localhost:5002/gettokenfromcode", "http://localhost:8000/code-callback" },
                RequirePkce = false,
                Enabled = true,
                AllowOfflineAccess = false,
            },
            new Client
            {
                // auth code with pkce flow client
                ClientId = "client_pkce",
                AllowedGrantTypes = GrantTypes.Code,
                // secret for authentication
                ClientSecrets = { new Secret("secret".Sha256()) },
                // scopes that client has access to
                AllowedScopes = { "openid", "api1.read", "api1.access", "profile" },
                RedirectUris = { "https://localhost:5002/gettokenfromcodepkce", "http://localhost:8000/pkce-callback" },
                RequirePkce = true,
                Enabled = true
            },
            new Client
            {
                // implicit flow client
                ClientId = "client_implicit",
                AllowedGrantTypes = GrantTypes.Implicit,
                // secret for authentication
                ClientSecrets = { new Secret("secret".Sha256()) },
                // scopes that client has access to
                AllowedScopes = { "openid", "api1.read" },
                RedirectUris = { "https://localhost:5002/implicit/" },
                RequirePkce = false,
                Enabled = true,
                AllowAccessTokensViaBrowser = true
            },

            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes = { "scope1" }
            },

            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:44300/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "scope2" }
            },
            new Client()
            {
                ClientId = "client_pwd",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes = { "api1.access", "scope2" }
            },
            new Client()
            {
                ClientId = "client_nopwd",
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes = { "api1.access" }
            }
        };
}