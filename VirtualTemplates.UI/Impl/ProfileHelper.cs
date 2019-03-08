using EPiServer.ServiceLocation;
using EPiServer.Shell.Profile;
using VirtualTemplates.UI.Interfaces;

namespace VirtualTemplates.UI.Impl
{
    [ServiceConfiguration(typeof(IProfileHelper))]
    public class ProfileHelper : IProfileHelper
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileHelper(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public void SetProfileValue<T>(string userId, string key, T profileValue)
        {
            var profile = _profileRepository.GetOrCreateProfile(userId);
            profile.Settings[key] = profileValue;
            _profileRepository.Save(profile);
        }

        public T GetProfileValue<T>(string userId, string key, T defaultValue)
        {
            var profileValue = defaultValue;

            if (_profileRepository.GetOrCreateProfile(userId).Settings
                .TryGetValue(key, out var profileObj))
            {
                profileValue = (T)profileObj;
            }

            return profileValue;
        }

    }
}
