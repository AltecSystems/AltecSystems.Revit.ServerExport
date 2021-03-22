using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RevitServerExport
{
    class NodeViewer
    {
        public static XmlDictionaryReader GetResponse(string path)
        {
            //System.Windows.MessageBox.Show("request started");
            //Create request

            //WebRequest request = WebRequest.Create("http://" + tbxServerName.Text + supportedVersions[cbxVersion.SelectedIndex, 1] + info);
            //WebRequest request = WebRequest.Create("http://192.168.1.10/RevitServerAdminRESTService2019/AdminRESTService.svc/|/contents");
            WebRequest request = WebRequest.Create(path + "/contents");
            //MessageBox.Show(path + "/contents");


            request.Method = "GET";

            // Add the information the request needs
            // http://192.168.1.10/RevitServerAdminRESTService2019/AdminRESTService.svc/|01_Bim-отдел/contents
            request.Headers.Add("User-Name", Environment.UserDomainName);
            request.Headers.Add("User-Machine-Name", Environment.MachineName);
            request.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());
            //http://192.168.1.10/RevitServerAdminRESTService2019/AdminRESTService.svc/|/contents
            //http://192.168.1.10/RevitServerAdminRESTService2019/AdminRESTService.svc/|/contents
            // Read the response
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(request.GetResponse().GetResponseStream(), quotas);
            jsonReader.ReadString();
            //System.Windows.MessageBox.Show("request finished");
            return jsonReader;
        }
        public static ObservableCollection<Node> FillingRoot(string root, string folders)
        {
            var reader = GetResponse(root + folders);
            ObservableCollection<Node> collect = new ObservableCollection<Node>();
            #region Basic TreeView Logic
            //while (reader.Read())
            //{
            //	if (reader.NodeType == XmlNodeType.Element && reader.Name == "Folders")
            //	{
            //		collect.Add(new Node() { Text = reader.Name });
            //	}
            //}
            //reader.Close();
            ////for (int i = 0; i < 5; i++)
            ////{
            ////	collect.Add(new Node() { Text = lvlCount.ToString() + i.ToString() });

            ////}

            ////if (lvlCount > 0)
            ////{
            ////	foreach (Node item in collect)
            ////	{
            ////		item.Children = FillingTree(lvlCount - 1);
            ////		foreach (var child in item.Children)
            ////			child.Parent = item;
            ////	}
            ////}
            //return collect;
            #endregion
            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Folders")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Folders")
                            break;

                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                        {
                            reader.Read();
                            string folderName = reader.ReadContentAsString();
                            string folder = folders + "|" + folderName;
                            collect.Add(new Node()
                            {
                                Text = folderName,
                                //Children = FillingTree(root, folder)
                            });

                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Models")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Models")
                            break;

                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                        {
                            //break;
                            reader.Read();
                            var val = reader.Value;
                            collect.Add(new Node() { Text = val });

                        }
                    }
                }
            }
            reader.Close();
            foreach (Node item in collect)
            {
                foreach (var child in item.Children)
                    child.Parent = item;
            }
            return collect;
        }
        public static ObservableCollection<Node> FillingTree(string root, string folders)
        {
            var reader = GetResponse(root + folders);
            ObservableCollection<Node> collect = new ObservableCollection<Node>();
            #region Basic TreeView Logic
            //while (reader.Read())
            //{
            //	if (reader.NodeType == XmlNodeType.Element && reader.Name == "Folders")
            //	{
            //		collect.Add(new Node() { Text = reader.Name });
            //	}
            //}
            //reader.Close();
            ////for (int i = 0; i < 5; i++)
            ////{
            ////	collect.Add(new Node() { Text = lvlCount.ToString() + i.ToString() });

            ////}

            ////if (lvlCount > 0)
            ////{
            ////	foreach (Node item in collect)
            ////	{
            ////		item.Children = FillingTree(lvlCount - 1);
            ////		foreach (var child in item.Children)
            ////			child.Parent = item;
            ////	}
            ////}
            //return collect;
            #endregion

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Folders")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Folders")
                            break;

                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                        {
                            reader.Read();
                            string folderName = reader.ReadContentAsString();
                            string folder = folders + "|" + folderName;
                            collect.Add(new Node()
                            {
                                Text = folderName,
                                Children = FillingTree(root, folder)
                            });

                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Models")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Models")
                            break;

                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
                        {
                            //break;
                            reader.Read();
                            var val = reader.Value;
                            collect.Add(new Node() { Text = val });
                        }
                    }
                }
            }

            reader.Close();
            foreach (Node item in collect)
            {
                foreach (var child in item.Children)
                    child.Parent = item;
            }
            return collect;
        }

    }
}
