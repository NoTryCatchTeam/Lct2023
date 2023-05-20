using Lct2023.Api.Dal;
using Lct2023.Api.Definitions.Constants;
using Lct2023.Api.Definitions.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddDbContext<NpgsqlDbContext>();

builder.Services.AddIdentity<ExtendedIdentityUser, IdentityRole<int>>(opt =>
    {
        opt.Lockout.AllowedForNewUsers = false;

        // TODO Should confirm ?
        opt.SignIn.RequireConfirmedAccount = false;
        opt.SignIn.RequireConfirmedEmail = false;
        opt.SignIn.RequireConfirmedPhoneNumber = false;

        opt.Password.RequireDigit = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequiredLength = 6;
        opt.Password.RequiredUniqueChars = 0;

        opt.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<NpgsqlDbContext>();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NpgsqlDbContext>();
    db.Database.Migrate();
}

// TODO Probably could be removed after identity will be added
// app.UseCookiePolicy(new CookiePolicyOptions()
// {
//     HttpOnly = HttpOnlyPolicy.Always,
//     Secure = CookieSecurePolicy.Always,
//     MinimumSameSitePolicy = SameSiteMode.Strict
// });

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
