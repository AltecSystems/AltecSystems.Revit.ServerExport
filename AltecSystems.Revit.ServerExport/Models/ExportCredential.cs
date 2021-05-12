using Autodesk.RevitServer.Enterprise.Common.ClientServer.DataContract.Model;
using System;
using System.IO;

namespace AltecSystems.Revit.ServerExport.Models
{
    internal class ExportCredential
    {
        public string HostIp { get; set; }
        public string ModelPath { get; set; }
        public string SsoUserName { get; } = string.Empty;
        public string UserName { get; set; }
        public string TempTargetFolder { get; }

        public string SavePath { get; }

        public ModelLocation ModelLocation { get; }

        public ExportCredential(string hostId, string modelPath, string savePath)
        {
            HostIp = hostId;
            SavePath = savePath;
            ModelPath = modelPath;
            TempTargetFolder = GetTempTargetFolder();
            UserName = GetUserName();
            ModelLocation = new ModelLocation(HostIp, ModelPath, ModelLocationType.Server);
        }

        private string GetTempTargetFolder()
        {
            string arg = "RevitServerTool_ModelDataFileDownload_";
            string arg2 = string.Format("{0}{1}", arg, Guid.NewGuid().ToString("N"));
            return Path.Combine(Path.GetTempPath(), arg2);
        }

        private string GetUserName()
        {
            return $"RevitServerTool:{Environment.MachineName}:1";
        }
    }
}
