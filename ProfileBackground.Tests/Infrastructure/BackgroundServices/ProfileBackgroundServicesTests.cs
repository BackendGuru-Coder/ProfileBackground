using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ProfileBackground.Domain.Interfaces;
using ProfileBackground.Domain.Models;
using ProfileBackground.Infrastructure.BackgroundServices;

namespace ProfileBackground.Tests.Infrastructure.BackgroundServices
{
    public class ProfileBackgroundServicesTests
    {
        private (Mock<IProfileService>, Mock<IServiceScopeFactory>, ILogger<ProfileUpdateService>) SetupDependencies(Dictionary<string, ProfileParameter> profiles)
        {
            var mockProfileService = new Mock<IProfileService>();
            mockProfileService.Setup(p => p.GetAll()).Returns(profiles);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(IProfileService)))
                .Returns(mockProfileService.Object);

            var mockScope = new Mock<IServiceScope>();
            mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var logger = new LoggerFactory().CreateLogger<ProfileUpdateService>();

            return (mockProfileService, mockScopeFactory, logger);
        }

        /// <summary>
        /// Trocando os valores do parâmetros automaticamente
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExecuteAsync_ShouldToggleProfileParameters()
        {
            // Arrange
            var profiles = new Dictionary<string, ProfileParameter>
        {
            {
                "Admin", new ProfileParameter
                {
                    Parameters = new Dictionary<string, string>
                    {
                        { "CanEdit", "true" },
                        { "CanDelete", "false" }
                    }
                }
            }
        };

            var (_, scopeFactory, logger) = SetupDependencies(profiles);
            var service = new ProfileUpdateService(scopeFactory.Object, logger);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(1)); 

            // Act
            await service.StartAsync(cts.Token);

            // Assert
            Assert.Equal("false", profiles["Admin"].Parameters["CanEdit"]);
            Assert.Equal("true", profiles["Admin"].Parameters["CanDelete"]);
        }

        /// <summary>
        /// Não rodar a rotina quando há cancelamento imediato
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExecuteAsync_ShouldNotRun_WhenCancelledImmediately()
        {
            // Arrange
            var mockProfileService = new Mock<IProfileService>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IProfileService))).Returns(mockProfileService.Object);

            var mockScope = new Mock<IServiceScope>();
            mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var logger = new LoggerFactory().CreateLogger<ProfileUpdateService>();
            var service = new ProfileUpdateService(mockScopeFactory.Object, logger);

            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel(); 

            // Act
            await service.StartAsync(tokenSource.Token);

            // Assert
            mockProfileService.Verify(p => p.GetAll(), Times.Never);
        }

        /// <summary>
        /// Retornar quando há exceção
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExecuteAsync_ShouldHandleException_WhenGetAllThrows()
        {
            // Arrange
            var mockProfileService = new Mock<IProfileService>();
            mockProfileService.Setup(x => x.GetAll()).Throws(new Exception("Erro de teste"));

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IProfileService))).Returns(mockProfileService.Object);

            var mockScope = new Mock<IServiceScope>();
            mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var logger = new LoggerFactory().CreateLogger<ProfileUpdateService>();
            var service = new ProfileUpdateService(mockScopeFactory.Object, logger);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(1));

            // Act
            var exception = await Record.ExceptionAsync(() => service.StartAsync(cts.Token));

            // Assert
            Assert.Null(exception);
            mockProfileService.Verify(x => x.GetAll(), Times.AtLeastOnce);
        }
    }
}
