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
    internal class ProxyModelLoader : IModelLoader
    {
        private readonly ConnectionModel _connectionModel;
        private readonly IModelService _bufferedProxy;

        public ProxyModelLoader(SettingsModel settings)
        {
            _connectionModel = new ConnectionModel() { RevitServerRootPath = settings.RevitServerRootPath, ServerHost = settings.ServerHost, RevitVersion = settings.CurrentSelectionServerVersion };
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
            return ProxyProvider.CreateProxyInstance(_connectionModel.RevitVersion).GetBufferedProxy<IModelService>(_connectionModel.ServerHost);
        }

        private async Task LoadModelAsync(ObservableCollection<Node> nodes, Node parent, string path, ProgressModel progress)
        {
            var foldersAndModels = await ListSubFoldersAndModels(path);

            if (progress.IsIndeterminate)
                progress.IsIndeterminate = false;

            progress.Max += foldersAndModels.SubFolders.Count();

            foreach (var item in foldersAndModels.SubModels)
            {
                nodes.Add(new Node() { Text = item, Parent = parent, IsModel = true, Path = path + "\\" + item });
            }
            foreach (var item in foldersAndModels.SubFolders)
            {
                var node = new Node() { Text = item, Parent = parent, Path = path + "\\" + item };
                string url = path + "\\" + item;

                nodes.Add(node);
                progress.CurrentProgress++;
                await LoadModelAsync(node.Children, node, url, progress);
            }
        }

        public async Task LoadModelAsync(ObservableCollection<Node> nodes, ProgressModel progress)
        {
            await LoadModelAsync(nodes, null, _connectionModel.RevitServerRootPath, progress);
        }
    }
}