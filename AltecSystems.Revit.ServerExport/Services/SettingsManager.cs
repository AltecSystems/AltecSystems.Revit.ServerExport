using AltecSystems.Revit.ServerExport.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace AltecSystems.Revit.ServerExport.Services
{
    internal class SettingsManager
    {
        private readonly string _pathFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RevitServerExport";
        private readonly string _pathFileSettings = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RevitServerExport\\" + "RevitServerExport.json";

        public SettingsManager()
        {
            ExistFolder();
        }

        public void SaveSettings(SettingsModel settings)
        {
            var value = JsonConvert.SerializeObject(settings);
            using (var stream = new StreamWriter(_pathFileSettings, false, Encoding.Default))
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