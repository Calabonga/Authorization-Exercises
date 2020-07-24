using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Authorization.IdentityServer
{
    public static class IdentityServerConfiguration
    {
        public static IEnumerable<Client> GetClients() =>
        new List<Client>
        {
            new Client
            {
                ClientId = "client_id_js",
                RequireClientSecret = false,
                RequireConsent = false,
                RequirePkce = true,
                AllowedGrantTypes =  GrantTypes.Code,
                AllowedCorsOrigins = { "https://localhost:9001" },
                RedirectUris = { "https://localhost:9001/callback.html", "https://localhost:9001/refresh.html" },
                PostLogoutRedirectUris = { "https://localhost:9001/index.html" },
                AllowedScopes =
                {
                    "OrdersAPI",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            },
            new Client
            {
                ClientId = "client_id_swagger",
                ClientSecrets = { new Secret("client_secret_swagger".ToSha256()) },
                AllowedGrantTypes =  GrantTypes.ResourceOwnerPassword,
                AllowedCorsOrigins = { "https://localhost:7001" },
                AllowedScopes =
                {
                    "SwaggerAPI",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            },
            new Client
            {
                ClientId = "client_id",
                ClientSecrets = { new Secret("client_secret".ToSha256()) },
                
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes =
                {
                    "OrdersAPI",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            },
            new Client
            {
                ClientId = "client_id_mvc",
                ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                AllowedScopes =
                {
                    "OrdersAPI",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                },

                RedirectUris = {"https://localhost:2001/signin-oidc"},
                PostLogoutRedirectUris = {"https://localhost:2001/signout-callback-oidc"},

                RequireConsent = false,

                AccessTokenLifetime = 5,

                AllowOfflineAccess = true

                // AlwaysIncludeUserClaimsInIdToken = true
            }
        };

      
        public static IEnumerable<ApiResource> GetApiResources()
        {
            yield return new ApiResource("SwaggerAPI");
            yield return new ApiResource("OrdersAPI");
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            yield return new IdentityResources.OpenId();
            yield return new IdentityResources.Profile();
        }

        /// <summary>
        /// IdentityServer4 version 4.x.x changes
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            yield return new ApiScope("SwaggerAPI", "Swagger API");
            yield return new ApiScope("OrdersAPI", "Orders API");
        }
    }
}
