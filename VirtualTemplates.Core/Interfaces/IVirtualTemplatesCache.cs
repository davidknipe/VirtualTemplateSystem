namespace VirtualTemplates.Core.Interfaces
{
    public interface IVirtualTemplatesCache
    {
        string VersionKey { get; }

        void Reset();
    }
}