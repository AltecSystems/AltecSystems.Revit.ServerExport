namespace AltecSystems.Revit.ServerExport.Models
{
    public class Model
    {
        public object LockContext { get; set; }
        public int LockState { get; set; }
        public object ModelLocksInProgress { get; set; }
        public int ModelSize { get; set; }
        public string Name { get; set; }
        public int ProductVersion { get; set; }
        public int SupportSize { get; set; }
    }
}