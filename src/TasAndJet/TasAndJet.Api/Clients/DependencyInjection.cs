using TasAndJet.Api.Infrastructure.Options;

namespace TasAndJet.Api.Clients
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClients(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmsOptions>(configuration.GetSection("SmsOptions"));
            services.AddSingleton<ISmsClient, SmsClient>();
            return services;
        }
    }
}
