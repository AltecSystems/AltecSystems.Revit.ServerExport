using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AltecSystems.Revit.ServerExport.Models;

namespace AltecSystems.Revit.ServerExport.Interface
{
    internal interface IModelLoader
    {
        Task LoadModelAsync(ObservableCollection<Node> nodes, ProgressModel progress);
    }
}