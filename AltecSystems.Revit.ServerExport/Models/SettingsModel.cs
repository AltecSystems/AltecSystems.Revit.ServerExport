﻿using System.Collections.Generic;
using AltecSystems.Revit.ServerExport.Command;
using Newtonsoft.Json;

namespace AltecSystems.Revit.ServerExport.Models
{
    internal class SettingsModel : NotifyPropertyChangedBase
    {
        private string _revitServerRootPath;
        private string _savePath;
        private string _serverHost;

        public string RevitServerRootPath { get => _revitServerRootPath; set => SetField(ref _revitServerRootPath, value, nameof(RevitServerRootPath)); }
        public string SavePath { get => _savePath; set => SetField(ref _savePath, value, nameof(SavePath)); }
        public string ServerHost { get => _serverHost; set => SetField(ref _serverHost, value, nameof(ServerHost)); }

        [JsonIgnore]
        public List<string> ServerVersion { get; } = new List<string> { "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020", "2021", "2022" };

        public string CurrentSelectionServerVersion { get; set; }
    }
}