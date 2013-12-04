//using System.Linq;

using System.Collections.Generic;

namespace AliaSQL.Core.Services.Impl
{
    public class FileFilterService : IFileFilterService
    {
        public string[] GetFilteredFilenames(string[] allFiles, string excludeFilenameContaining)
        {
            List<string> itemsToReturn=new List<string>();
            if (string.IsNullOrEmpty(excludeFilenameContaining))
                return allFiles;


            foreach (var x in allFiles)
            {
                var beginningOfFileName = x.LastIndexOfAny(new[]{'\\','/'});
                if (!x.Substring(beginningOfFileName).Contains(excludeFilenameContaining))
                {
                    itemsToReturn.Add(x);
                }
            }
            return itemsToReturn.ToArray();
        }
    }
}