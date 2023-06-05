using Payment.Application.Interface;

namespace Payment.Api.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConfiguration configuration;

        public ConnectionService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string? Datebase => configuration.GetConnectionString("Database");
    }
}
