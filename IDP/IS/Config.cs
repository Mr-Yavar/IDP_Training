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
                
            };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[] { };
        public static IEnumerable<ApiResource> Apis => new ApiResource[] { };
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
                    },
                    ClientSecrets = { new Secret("CompanyEmployeeClientSecret".Sha512()) },
                    RequirePkce = true,
                    RequireConsent = true,
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:5010/signout-callback-oidc",
                    },
                    ClientUri = "https://localhost:5010",
                },
            };
    }
}
