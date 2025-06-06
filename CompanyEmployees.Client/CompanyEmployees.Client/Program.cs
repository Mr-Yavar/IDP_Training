using System.IdentityModel.Tokens.Jwt;
using CompanyEmployees.Client.Extensions;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<BearerTokenHandler>();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder
    .Services.AddHttpClient(
        "APIClient",
        client =>
        {
            client.BaseAddress = new Uri("https://localhost:5001/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        }
    )
    .AddHttpMessageHandler<BearerTokenHandler>();
;
builder.Services.AddHttpClient(
    "IDPClient",
    client =>
    {
        client.BaseAddress = new Uri("https://localhost:5005/");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
    }
);
builder
    .Services.AddAuthentication(opt =>
    {
        opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        opt =>
        {
            opt.AccessDeniedPath = "/Auth/AccessDenied";
        }
    )
    .AddOpenIdConnect(
        OpenIdConnectDefaults.AuthenticationScheme,
        opt =>
        {
            opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.Authority = "https://localhost:5005";
            opt.ClientId = "companyemployeeclient";
            opt.ResponseType = OpenIdConnectResponseType.Code;
            opt.SaveTokens = true;
            opt.ClientSecret = "CompanyEmployeeClientSecret";
            opt.UsePkce = true;
            opt.GetClaimsFromUserInfoEndpoint = true;
            opt.ClaimActions.DeleteClaims(new string[] { "sid", "idp" });
            opt.Scope.Add("address");
            opt.Scope.Add("roles");
            opt.Scope.Add("companyemployeeapi.scope");

            // opt.ClaimActions.MapUniqueJsonKey("address", "address");
            opt.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Role, "role");

            opt.TokenValidationParameters =
                new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = JwtClaimTypes.Role,
                };

            opt.Scope.Add("country");
            opt.ClaimActions.MapUniqueJsonKey("country", "country");
            opt.Scope.Add("offline_access");
        }
    );
builder.Services.AddAuthorization(authOpt =>
{
    authOpt.AddPolicy(
        "CanCreateAndModifyData",
        policyBuilder =>
        {
            policyBuilder.RequireAuthenticatedUser();
            policyBuilder.RequireRole("role", "Administrator");
            policyBuilder.RequireClaim("country", "USA");
        }
    );
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
