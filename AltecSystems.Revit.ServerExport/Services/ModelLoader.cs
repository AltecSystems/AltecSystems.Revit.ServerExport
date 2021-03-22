using AltecSystems.Revit.ServerExport.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AltecSystems.Revit.ServerExport.Services
{
    internal class ModelLoader
    {
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

        public async Task<Root> LoadModelAsync(string path)
        {
            return await GetResponseAsync(path);
        }
    }
}
