using System.Collections.Generic;

namespace VirtualTemplates.Core.Interfaces
{
    public interface IFileSearcher
    {
        /// <summary>
        /// Search all files within the solution
        /// </summary>
        /// <param name="searchString">String to look for (case insenstive)</param>
        /// <returns>A list of matching files where the search string is found in the path, name or contents</returns>
        List<string> SearchFiles(string searchString);
    }
}