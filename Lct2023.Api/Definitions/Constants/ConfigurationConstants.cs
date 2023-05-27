namespace Lct2023.Api.Definitions.Constants;

public static class ConfigurationConstants
{
    public static class Secrets
    {
        public const string VK_CLIENT_ID = "Vk:ClientId";

        public const string VK_CLIENT_SECRET = "Vk:ClientSecret";

        public const string DB_CONNECTION_STRING = "DbConnectionString";

        public const string JWT_SECRET = "Jwt:Secret";

        public const string JWT_ISSUER = "Jwt:Issuer";

        public const string JWT_AUDIENCE = "Jwt:Audience";

        public const string JWT_ACCESS_TOKEN_EXPIRES_IN_DAYS = "Jwt:AccessTokenExpiresInDays";

        public const string JWT_REFRESH_TOKEN_EXPIRES_IN_DAYS = "Jwt:RefreshTokenExpiresInDays";

        public const string CMS_API_KEY = "Cms:ApiKey";
    }
}
