using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ProfileBackground.Domain.Interfaces;

namespace ProfileBackground.Infrastructure.BackgroundServices
{
    public class ProfileUpdateService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProfileUpdateService> _logger;

        public ProfileUpdateService(IServiceScopeFactory scopeFactory, ILogger<ProfileUpdateService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var _profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

                try
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
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar perfis.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
