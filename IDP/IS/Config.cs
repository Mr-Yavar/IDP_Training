using System.Security.Cryptography;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IS
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "User role(s)", new List<string> { "role" }),
                new IdentityResource("country", "Your country", new List<string> { "country" }),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("companyemployeeapi.scope", "CompanyEmployee API Scope"),
            };
        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("companyemployeeapi", "CompanyEmployee API")
                {
                    Scopes = { "companyemployeeapi.scope" },
                    UserClaims = new List<string>() { "role" },
                },
            };
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "companyemployeeclient",
                    ClientName = "CompanyEmployeeClient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> { "https://localhost:5010/signin-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "companyemployeeapi.scope",
                        "country",
                    },
                    ClientSecrets = { new Secret("CompanyEmployeeClientSecret".Sha512()) },
                    RequirePkce = true,
                    RequireConsent = true,
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:5010/signout-callback-oidc",
                    },
                    ClientUri = "https://localhost:5010",
                    AccessTokenLifetime = 120,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true
                },
                new Client
{
    ClientId = "adminui",
    ClientName = "Skoruba Admin UI",
    AllowedGrantTypes = GrantTypes.Code,

    RequireClientSecret = true,
    ClientSecrets =
    {
        new Secret("AdminUISecert".Sha256())
    },

    RedirectUris = {
        "https://localhost:999/signin-oidc" // Your Admin UI base URL + signin path
    },
    PostLogoutRedirectUris = {
        "https://localhost:999/signout-callback-oidc"
    },

    AllowedScopes = {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile,
        IdentityServerConstants.StandardScopes.Email,
        "roles", // if roles are used
        "admin_api" // your Admin API scope
    },

    AllowOfflineAccess = true, // For refresh tokens
    RequirePkce = true,
    AccessTokenLifetime = 3600,
    IdentityTokenLifetime = 300,

    // Optional: role claims
    AlwaysIncludeUserClaimsInIdToken = true
}
            };
    }
}
