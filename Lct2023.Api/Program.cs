using System.Security.Claims;
using System.Text;
using AspNet.Security.OAuth.Vkontakte;
using AutoMapper;
using Lct2023.Api.Dal;
using Lct2023.Api.Definitions;
using Lct2023.Api.Definitions.Constants;
using Lct2023.Api.Definitions.Identity;
using Lct2023.Api.Definitions.Types;
using Lct2023.Api.Services;
using Lct2023.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", false, true);
builder.Configuration.AddEnvironmentVariables();

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
    .AddAuthentication(opt =>
    {
        opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.SaveToken = true;
        opt.RequireHttpsMetadata = builder.Environment.IsProduction();

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_ISSUER),
            ValidAudience = builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_AUDIENCE),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_SECRET))),
            ClockSkew = TimeSpan.Zero,
        };
    })
    .AddVkontakte(options =>
    {
        options.ClientId = builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.VK_CLIENT_ID);
        options.ClientSecret = builder.Configuration.GetValue<string>(ConfigurationConstants.Secrets.VK_CLIENT_SECRET);

        options.Scope.Add("email");
        //options.Scope.Add("photos");
        options.Scope.Add("friends");
        options.Fields.Add("bdate");
		options.Fields.Add("photo_max");
         
        options.ClaimActions.MapJsonKey(CustomClaimTypes.PHOTO_URL, "photo");
        options.ClaimActions.MapJsonKey(CustomClaimTypes.BIRTH_DATE, "bdate");
        options.ClaimActions.MapJsonKey(CustomClaimTypes.PHOTO_MAX, "photo_max");

        options.Events.OnTicketReceived += context =>
        {
            if (context.Principal?.Claims?.Any() != true)
            {
                return Task.CompletedTask;
            }

            var queryParams = new Dictionary<string, string>
            {
                { CustomClaimTypes.EMAIL, context.Principal.FindFirst(ClaimTypes.Email)?.Value },
                { CustomClaimTypes.FIRST_NAME, context.Principal.FindFirst(ClaimTypes.GivenName)?.Value },
                { CustomClaimTypes.LAST_NAME, context.Principal.FindFirst(ClaimTypes.Surname)?.Value },
                { CustomClaimTypes.BIRTH_DATE, context.Principal.FindFirst(CustomClaimTypes.BIRTH_DATE)?.Value },
                { CustomClaimTypes.PHOTO_URL, context.Principal.FindFirst(CustomClaimTypes.PHOTO_URL)?.Value },
                { CustomClaimTypes.PHOTO_MAX, context.Principal.FindFirst(CustomClaimTypes.PHOTO_MAX)?.Value },
                { CustomClaimTypes.EXTERNAL_LOGIN_PROVIDER, VkontakteAuthenticationDefaults.AuthenticationScheme },
                { CustomClaimTypes.EXTERNAL_LOGIN_PROVIDER_KEY, context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value },
            };

            context.ReturnUri = QueryHelpers.AddQueryString(context.ReturnUri, queryParams);

            return Task.CompletedTask;
        };
    });

builder.Services.AddAutoMapper(x => x.AddProfile<ApiMapperProfile>());

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAuthTokensService, AuthTokensService>();
builder.Services.AddTransient<IUserService, UserService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();

    scope.ServiceProvider.GetRequiredService<NpgsqlDbContext>().Database.Migrate();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest,
    MinimumSameSitePolicy = SameSiteMode.Lax,
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
