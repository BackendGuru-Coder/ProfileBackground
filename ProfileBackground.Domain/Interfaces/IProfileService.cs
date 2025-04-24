using ProfileBackground.Domain.Models;

namespace ProfileBackground.Domain.Interfaces
{
    public interface IProfileService
    {
        Dictionary<string, ProfileParameter> GetAll();
        ProfileParameter? Get(string profileName);
        void Add(ProfileParameter profile);
        void Update(string profileName, Dictionary<string, string> parameters);
        void Delete(string profileName);
        bool Validate(string profileName, string action);
    }
}
