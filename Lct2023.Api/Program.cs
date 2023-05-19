using Lct2023.Api.Definitions.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", false, true);
builder.Configuration.AddEnvironmentVariables();

// TODO If needed
// if (!builder.Environment.IsProduction())
// {
//     builder.Configuration.AddJsonFile($"secrets.{builder.Environment.EnvironmentName}.json", false, true);
// }

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // TODO Will be included in AddIdentity<>(), so remove here
    .AddCookie(JwtBearerDefaults.AuthenticationScheme)
    // TODO For further email signin
    // .AddJwtBearer(opt =>
    // {
    //     opt.SaveToken = true;
    //     opt.RequireHttpsMetadata = builder.Environment.IsProduction();
    //
    //     opt.TokenValidationParameters = new TokenValidationParameters
    //     {
    //         ValidateIssuer = true,
    //         ValidateAudience = true,
    //         ValidateLifetime = true,
    //         ValidateIssuerSigningKey = true,
    //         ValidIssuer = builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_ISSUER),
    //         ValidAudience = builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_AUDIENCE),
    //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_SECRET))),
    //         ClockSkew = TimeSpan.Zero,
    //     };
    // })
    .AddVkontakte(options =>
    {
        options.ClientId = builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.VK_CLIENT_ID);
        options.ClientSecret = builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.VK_CLIENT_SECRET);
    });

var app = builder.Build();

// TODO Probably could be removed after identity will be added
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always,
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
