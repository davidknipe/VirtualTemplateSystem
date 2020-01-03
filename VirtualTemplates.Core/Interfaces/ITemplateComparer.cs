namespace VirtualTemplates.Core.Interfaces
{
    public interface ITemplateComparer
    {
        /// <summary>
        /// Compares a virtual file against the original version
        /// </summary>
        /// <param name="currentFileContents">The string representing the current file</param>
        /// <param name="originalPath">The original path of the file to compare</param>
        /// <returns>True if the file has changed, false if not</returns>
        bool TemplateIsChanged(string currentFileContents, string originalPath);
    }
}