using AltecSystems.Revit.ServerExport.Interface;
using AltecSystems.Revit.ServerExport.Models;
using AltecSystems.Revit.ServerExport.Utils;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.Proxy;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.ServiceContract.Model;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AltecSystems.Revit.ServerExport.Services
{
    internal class ModelServiceLoader : IModelLoader
    {
        private readonly ConnectionModel _connectionModel;
        private readonly IModelService _bufferedProxy;

        public ModelServiceLoader(SettingsModel settings)
        {
            _connectionModel = new ConnectionModel() { RevitServerRootPath = settings.RevitServerRootPath, ServerHost = settings.ServerHost };
            _bufferedProxy = GetBufferedProxy().Proxy;
        }

        public async Task<FoldersAndModels> ListSubFoldersAndModels(string relativeFolderPath)
        {
            var foldersAndModels = await Task<FoldersAndModels>.Factory.StartNew(() => 
            {
                var sessionToken = SessionTokenGenerator.CreateServiceSessionToken();
                _bufferedProxy.ListSubFoldersAndModels(sessionToken, relativeFolderPath, out var folders, out var models);
                var result = new FoldersAndModels(ParseList(folders), ParseList(models));
                return result;
            });
            return foldersAndModels;
        }


        private IEnumerable<string> ParseList(IEnumerable folderList)
        {
            var result = new List<string>();

            foreach (var fodler in folderList)
            {
                result.Add(fodler.ToString().Split('|')[0]);
            }
            return result;
        }

        private IClientProxy<IModelService> GetBufferedProxy()
        {
            return ProxyProvider.Instance.GetBufferedProxy<IModelService>(_connectionModel.ServerHost);
        }

        private async Task LoadModelAsync(ObservableCollection<Node> nodes, string path, ProgressModel progress)
        {
            var foldersAndModels = await ListSubFoldersAndModels(path);

            if (progress.IsIndeterminate)
                progress.IsIndeterminate = false;

            progress.Max += foldersAndModels.SubFolders.Count();

            foreach (var item in foldersAndModels.SubModels)
            {
                nodes.Add(new Node() { Text = item });
            }
            foreach (var item in foldersAndModels.SubFolders)
            {
                var node = new Node() { Text = item };
                string newurl;
                newurl = path + "\\" + item;

                nodes.Add(node);
                progress.CurrentProgress++;
                await LoadModelAsync(node.Children, newurl, progress);
            }
        }

        public async Task LoadModelAsync(ObservableCollection<Node> nodes, ProgressModel progress)
        {
            await LoadModelAsync(nodes, _connectionModel.RevitServerRootPath, progress);
        }
    }
}
