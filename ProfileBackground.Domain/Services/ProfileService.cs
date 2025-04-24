using ProfileBackground.Domain.Interfaces;
using ProfileBackground.Domain.Models;

namespace ProfileBackground.Domain.Services
{
    public class ProfileService : IProfileService
    {
        private readonly Dictionary<string, ProfileParameter> _profiles = new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, ProfileParameter> GetAll() => _profiles;

        public ProfileParameter? Get(string profileName) =>
            _profiles.TryGetValue(profileName, out var profile) ? profile : null;

        public void Add(ProfileParameter profile) => _profiles[profile.ProfileName] = profile;

        public void Update(string profileName, Dictionary<string, string> parameters)
        {
            if (_profiles.ContainsKey(profileName))
            {
                _profiles[profileName].Parameters = parameters;
            }
        }

        public void Delete(string profileName) => _profiles.Remove(profileName);

        public bool Validate(string profileName, string action)
        {
            if (_profiles.TryGetValue(profileName, out var profile))
            {
                var param = profile.Parameters
                    .FirstOrDefault(p => string.Equals(p.Key, action, StringComparison.OrdinalIgnoreCase));

                return !string.IsNullOrEmpty(param.Key) &&
                       param.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
