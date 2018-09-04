namespace VirtualTemplates.Core.Interfaces
{
    /// <summary>
    /// Used to get keys for the template based on the virtual path
    /// </summary>
    public interface ITemplateKeyConverter
    {
        /// <summary>
        /// Gets the key for the template based on path
        /// </summary>
        /// <param name="virtualPath">The virtual path of the template relative to the app root</param>
        /// <returns>The key to the virtual template</returns>
        string GetTemplateKey(string virtualPath);
    }
}
