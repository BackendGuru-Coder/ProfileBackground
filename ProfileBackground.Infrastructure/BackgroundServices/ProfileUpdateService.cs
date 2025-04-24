using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ProfileBackground.Domain.Interfaces;

namespace ProfileBackground.Infrastructure.BackgroundServices
{
    public class ProfileUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProfileUpdateService> _logger;

        public ProfileUpdateService(IServiceProvider serviceProvider, ILogger<ProfileUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var _profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var profiles = _profileService.GetAll();

                foreach (var profile in profiles.Values)
                {
                    foreach (var key in profile.Parameters.Keys.ToList())
                    {
                        profile.Parameters[key] = profile.Parameters[key] == "true" ? "false" : "true";
                    }
                }

                _logger.LogInformation("Perfis atualizados.");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
