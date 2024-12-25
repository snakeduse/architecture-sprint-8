using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var keycloak = Environment.GetEnvironmentVariable("APP_KEYCLOAK_URL");
var realm = Environment.GetEnvironmentVariable("APP_KEYCLOAK_REALM");

builder.Services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, ClaimsTransformer>();

builder.Services
    .AddAuthentication()
    .AddJwtBearer(x =>
    {
      x.RequireHttpsMetadata = false;
      x.MetadataAddress = $"{keycloak}/realms/{realm}/.well-known/openid-configuration";
      x.ClaimsIssuer = $"{keycloak}/realms/{realm}";
      x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidIssuers = new[] { $"{keycloak}/realms/{realm}" },
        SignatureValidator = (token, parameters) => new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token)
      };
    });

builder.Services.AddAuthorization(x => x.AddPolicy("reports", y =>
{
  y.RequireRole("prothetic_user");
}));
builder.Services.AddCors();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());


app.MapGet("/reports", [Authorize("reports")] (HttpContext context) =>
{
    return "a,b,c\n1,2,3\n4,5,6";
});


app.Run();
