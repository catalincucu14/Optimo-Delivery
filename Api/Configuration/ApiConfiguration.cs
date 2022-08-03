namespace Api.Configuration
{
    public class ApiConfiguration
    {
        public readonly string JwtSecret;

        public readonly int JwtLifeSpan;

        public ApiConfiguration(IConfiguration configuration)
        {
            JwtSecret = configuration["JwtConfig:Secret"];

            JwtLifeSpan = int.Parse(configuration["JwtConfig:LifeSpan"]);
        }
    }
}
