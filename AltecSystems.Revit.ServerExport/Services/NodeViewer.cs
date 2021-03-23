//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Runtime.Serialization.Json;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Xml;
//using System.Xml.Serialization;
//using AltecSystems.Revit.ServerExport.Models;

//namespace AltecSystems.Revit.ServerExport.Services
//{
//    class NodeViewer
//    {
//        public static XmlDictionaryReader GetResponse(string path)
//        {
//            WebRequest request = WebRequest.Create(path + "/contents");
//            request.Method = "GET";
//            request.Headers.Add("User-Name", Environment.UserDomainName);
//            request.Headers.Add("User-Machine-Name", Environment.MachineName);
//            request.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());
//            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
//            XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(request.GetResponse().GetResponseStream(), quotas);
//            jsonReader.ReadString();
//            return jsonReader;
//        }
//        public static ObservableCollection<Node> FillingRoot(ObservableCollection<Node> node, string root, string folders)
//        {
//            var reader = GetResponse(root + folders);

//            while (reader.Read())
//            {

//                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Folders")
//                {
//                    while (reader.Read())
//                    {
//                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Folders")
//                            break;

//                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
//                        {
//                            reader.Read();
//                            string folderName = reader.ReadContentAsString();
//                            string folder = folders + "|" + folderName;
//                            node.Add(new Node()
//                            {
//                                Text = folderName,
//                            });

//                        }
//                    }
//                }
//                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Models")
//                {
//                    while (reader.Read())
//                    {
//                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Models")
//                            break;

//                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
//                        {
//                            reader.Read();
//                            var val = reader.Value;
//                            node.Add(new Node() { Text = val });

//                        }
//                    }
//                }
//            }
//            reader.Close();
//            //foreach (Node item in node)
//            //{
//            //    foreach (var child in item.Children)
//            //        child.Parent = item;
//            //}
//            return node;
//        }
//        public static ObservableCollection<Node> FillingTree(string root, string folders)
//        {
//            var reader = GetResponse(root + folders);
//            ObservableCollection<Node> collect = new ObservableCollection<Node>();

//            while (reader.Read())
//            {
//                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Folders")
//                {
//                    while (reader.Read())
//                    {
//                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Folders")
//                            break;

//                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
//                        {
//                            reader.Read();
//                            string folderName = reader.ReadContentAsString();
//                            string folder = folders + "|" + folderName;
//                            collect.Add(new Node()
//                            {
//                                Text = folderName,
//                                Children = FillingTree(root, folder)
//                            });

//                        }
//                    }
//                }
//                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Models")
//                {
//                    while (reader.Read())
//                    {
//                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Models")
//                            break;

//                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
//                        {
//                            reader.Read();
//                            var val = reader.Value;
//                            collect.Add(new Node() { Text = val });
//                        }
//                    }
//                }
//            }

//            reader.Close();
//            //foreach (Node item in collect)
//            //{
//            //    foreach (var child in item.Children)
//            //        child.Parent = item;
//            //}
//            return collect;
//        }

//    }
//}
