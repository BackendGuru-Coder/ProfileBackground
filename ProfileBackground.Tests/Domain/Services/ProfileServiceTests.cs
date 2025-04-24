using ProfileBackground.Domain.Models;
using ProfileBackground.Domain.Services;

namespace ProfileBackground.Tests.Domain.Services
{
    public class ProfileServiceTests
    {
        [Fact]
        public void Add_ShouldStoreProfile_WhenCalled()
        {
            // Arrange
            var service = new ProfileService();
            var profile = new ProfileParameter
            {
                ProfileName = "Admin",
                Parameters = new Dictionary<string, string> { { "CanEdit", "true" } }
            };

            // Act
            service.Add(profile);
            var result = service.Get("Admin");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("true", result.Parameters["CanEdit"]);
        }

        [Fact]
        public void Get_ShouldReturnNull_WhenProfileDoesNotExist()
        {
            // Arrange
            var service = new ProfileService();

            // Act
            var result = service.Get("ghost");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllProfiles()
        {
            // Arrange
            var service = new ProfileService();
            service.Add(new ProfileParameter { ProfileName = "User", Parameters = new() });
            service.Add(new ProfileParameter { ProfileName = "Admin", Parameters = new() });

            // Act
            var result = service.GetAll();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains("User", result.Keys);
            Assert.Contains("Admin", result.Keys);
        }

        [Fact]
        public void Update_ShouldModifyExistingProfile()
        {
            // Arrange
            var service = new ProfileService();
            service.Add(new ProfileParameter
            {
                ProfileName = "Editor",
                Parameters = new Dictionary<string, string> { { "CanEdit", "true" } }
            });

            // Act
            service.Update("Editor", new Dictionary<string, string> { { "CanEdit", "false" } });
            var updated = service.Get("Editor");

            // Assert
            Assert.Equal("false", updated?.Parameters["CanEdit"]);
        }

        [Fact]
        public void Update_ShouldNotThrow_WhenProfileDoesNotExist()
        {
            // Arrange
            var service = new ProfileService();

            // Act 
            var exception = Record.Exception(() =>
                service.Update("unknown", new Dictionary<string, string> { { "CanEdit", "true" } }));

            // Assert
            Assert.Null(exception);
            Assert.Null(service.Get("unknown"));
        }

        [Fact]
        public void Delete_ShouldRemoveProfile()
        {
            // Arrange
            var service = new ProfileService();
            service.Add(new ProfileParameter
            {
                ProfileName = "Temp",
                Parameters = new Dictionary<string, string>()
            });

            // Act
            service.Delete("Temp");
            var result = service.Get("Temp");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Validate_ShouldReturnTrue_WhenParameterIsTrue()
        {
            // Arrange
            var service = new ProfileService();
            service.Add(new ProfileParameter
            {
                ProfileName = "Tester",
                Parameters = new Dictionary<string, string> { { "CanDelete", "true" } }
            });

            // Act
            var result = service.Validate("Tester", "CanDelete");

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("false")]
        [InlineData("FALSE")]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_ShouldReturnFalse_WhenParameterIsNotTrue(string value)
        {
            // Arrange
            var service = new ProfileService();
            var parameters = new Dictionary<string, string>();
            if (value != null)
                parameters["CanEdit"] = value;

            service.Add(new ProfileParameter { ProfileName = "Tester", Parameters = parameters });

            // Act
            var result = service.Validate("Tester", "CanDelete");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_ShouldReturnFalse_WhenProfileDoesNotExist()
        {
            // Arrange
            var service = new ProfileService();

            // Act
            var result = service.Validate("Ghost", "CanDelete");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_ShouldReturnFalse_WhenActionNotFound()
        {
            // Arrange
            var service = new ProfileService();
            service.Add(new ProfileParameter
            {
                ProfileName = "Dev",
                Parameters = new Dictionary<string, string> { { "CanEdit", "true" } }
            });

            // Act
            var result = service.Validate("Dev", "CanDelete"); 

            // Assert
            Assert.False(result);
        }
    }
}
