﻿using AltecSystems.Revit.ServerExport.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AltecSystems.Revit.ServerExport.Interface
{
    internal interface IModelLoader
    {
        Task LoadModelAsync(ObservableCollection<Node> nodes, ProgressModel progress);
    }
}