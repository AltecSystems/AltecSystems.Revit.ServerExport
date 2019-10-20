using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RevitServerExport
{
    class Request
    {
#warning Logic to retrieve folders tree from server


        private XmlDictionaryReader GetResponse(string info)
        {
            // Create request

            //WebRequest request = WebRequest.Create("http://" + tbxServerName.Text + supportedVersions[cbxVersion.SelectedIndex, 1] + info);
            WebRequest request = WebRequest.Create("http://192.168.1.10/RevitServerAdminRESTService2019/AdminRESTService.svc");
            request.Method = "GET";

            // Add the information the request needs

            request.Headers.Add("User-Name", Environment.UserName);
            request.Headers.Add("User-Machine-Name", Environment.MachineName);
            request.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());

            // Read the response
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(request.GetResponse().GetResponseStream(), quotas);

            return jsonReader;
        }
        

    }
}
