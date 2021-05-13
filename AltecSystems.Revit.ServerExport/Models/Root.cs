using System.Collections.Generic;

namespace AltecSystems.Revit.ServerExport.Models
{
    public class Root
    {
        public string Path { get; set; }
        public long DriveFreeSpace { get; set; }
        public long DriveSpace { get; set; }
        public List<object> Files { get; set; }
        public List<Folder> Folders { get; set; }
        public object LockContext { get; set; }
        public int LockState { get; set; }
        public object ModelLocksInProgress { get; set; }
        public List<Model> Models { get; set; }
    }
}