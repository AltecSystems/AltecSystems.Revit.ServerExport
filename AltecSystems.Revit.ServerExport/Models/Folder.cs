namespace AltecSystems.Revit.ServerExport.Models
{
    public class Folder
    {
        public bool HasContents { get; set; }
        public object LockContext { get; set; }
        public int LockState { get; set; }
        public object ModelLocksInProgress { get; set; }
        public string Name { get; set; }
        public object Size { get; set; }
    }
}