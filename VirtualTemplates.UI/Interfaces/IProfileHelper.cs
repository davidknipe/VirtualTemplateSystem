namespace VirtualTemplates.UI.Interfaces
{
    public interface IProfileHelper
    {
        void SetProfileValue<T>(string userId, string key, T profileValue);
        T GetProfileValue<T>(string userId, string key, T defaultValue);
    }
}