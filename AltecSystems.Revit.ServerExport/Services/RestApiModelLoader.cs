using AltecSystems.Revit.ServerExport.Interface;
using AltecSystems.Revit.ServerExport.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AltecSystems.Revit.ServerExport.Services
{
    internal class RestApiModelLoader : IModelLoader
    {
        private readonly SettingsModel _settings;
        public RestApiModelLoader(SettingsModel settings)
        {
            _settings = settings;
        }

        private async Task<Root> GetResponseAsync(string path)
        {
            WebRequest request = WebRequest.Create(path + "/contents");
            request.Method = "GET";
            request.Headers.Add("User-Name", Environment.UserDomainName);
            request.Headers.Add("User-Machine-Name", Environment.MachineName);
            request.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());
            try
            {
                var response = (HttpWebResponse)(await request.GetResponseAsync());

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<Root>(responseFromServer);
                return data;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (var rs = e.Response.GetResponseStream())
                    using (var reader = new System.IO.StreamReader(rs))
                    {
                        string responseText = reader.ReadToEnd();
                        var data = JsonConvert.DeserializeObject<Root>(responseText);
                        return data;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task LoadModelAsync(ObservableCollection<Node> nodes, ProgressModel progress)
        {
            var root = "http://" + _settings.ServerHost + _settings.RestUrl;
            var rootfolder = root + "/|";
            await LoadModelAsync(nodes,null, rootfolder, progress);
        }

        private async Task LoadModelAsync(ObservableCollection<Node> nodes, Node parent, string url, ProgressModel progress)
        {
            var model = await GetResponseAsync(url);

            if (progress.IsIndeterminate)
                progress.IsIndeterminate = false;

            progress.Max += model.Folders.Count;

            foreach (var item in model.Models)
            {
                nodes.Add(new Node() { Text = item.Name, Parent = parent });
            }
            foreach (var item in model.Folders)
            {
                var node = new Node() { Text = item.Name, Parent = parent };
                string newurl;
                if (url.EndsWith("/|"))
                {
                    newurl = url + node.Text;
                }
                else
                {
                    newurl = url + "|" + node.Text;
                }

                nodes.Add(node);
                progress.CurrentProgress++;
                await LoadModelAsync(node.Children,node ,newurl, progress);
            }
        }
    }
}
