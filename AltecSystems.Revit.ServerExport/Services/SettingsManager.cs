using AltecSystems.Revit.ServerExport.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AltecSystems.Revit.ServerExport.Services
{
    internal class SettingsManager
    {
        private readonly string _pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RevitServerExport";
        private readonly string _pathFileSettings = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RevitServerExport\\" + "RevitServerExport.json";
        private readonly string _pathFileCache = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RevitServerExport\\" + "RevitServerExportCache.json";

        public SettingsManager()
        {
            ExistFolder();
        }

        public void SaveSettings(SettingsModel settings)
        {
            var value = JsonConvert.SerializeObject(settings);
            using (var stream = new StreamWriter(_pathFileSettings, false,Encoding.Default))
            {
                stream.Write(value);
            }
        }

        public void SaveCache(IEnumerable<Node> models)
        {
            var value = JsonConvert.SerializeObject(models);
            using (var stream = new StreamWriter(_pathFileCache, false, Encoding.Default))
            {
                stream.Write(value);
            }
        }

        public SettingsModel GetSettings()
        {
            try
            {
                using (var stream = new StreamReader(_pathFileSettings))
                {
                    var value = stream.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return GetDefaultSettings();
                    }
                    var settings = JsonConvert.DeserializeObject<SettingsModel>(value);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                return GetDefaultSettings();
            }
        }

        public IEnumerable<Node> GetCasheNodes()
        {
            try
            {
                using (var stream = new StreamReader(_pathFileCache))
                {
                    var value = stream.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return new List<Node>();
                    }
                    var settings = JsonConvert.DeserializeObject<IEnumerable<Node>>(value);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                return new List<Node>();
            }
        }

        private SettingsModel GetDefaultSettings()
        {
            return new SettingsModel()
            {
                SavePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                RevitServerRootPath = @"E:\ProgramData\Revit server",
                ServerHost = "192.168.1.10",
            };
        }

        private void ExistFolder()
        {
            if (!Directory.Exists(_pathFolder))
            {
               Directory.CreateDirectory(_pathFolder);
            }
        }
    }
}
