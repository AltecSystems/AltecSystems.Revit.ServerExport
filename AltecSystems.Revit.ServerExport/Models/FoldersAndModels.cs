using System.Collections.Generic;

namespace AltecSystems.Revit.ServerExport.Models
{
    internal class FoldersAndModels
    {
        public IEnumerable<string> SubFolders { get; set; }
        public IEnumerable<string> SubModels { get; set; }

        public FoldersAndModels(IEnumerable<string> subFolders, IEnumerable<string> subModels)
        {
            SubFolders = subFolders;
            SubModels = subModels;
        }
    }
}
