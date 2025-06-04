using Duende.IdentityServer.Models;

namespace IS
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
                { };
        public static IEnumerable<ApiResource> Apis =>
new ApiResource[]
{ };
        public static IEnumerable<Client> Clients =>
            new Client[]
                { };
    }
}