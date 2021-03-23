﻿using AltecSystems.Revit.ServerExport.Command;
using System;
using System.Collections.Generic;

namespace AltecSystems.Revit.ServerExport.Models
{
    internal enum LoaderType
    {
        Rest = 0,
        Proxy = 1
    }
    internal class SettingsModel : NotifyPropertyChangedBase
    {
        private string _revitServerRootPath;
        private string _savePath;
        private string _serverHost;

        public string RevitServerRootPath { get => _revitServerRootPath; set => SetField(ref _revitServerRootPath, value, nameof(RevitServerRootPath)); }
        public string SavePath { get => _savePath; set => SetField(ref _savePath, value, nameof(SavePath)); }
        public string ServerHost { get => _serverHost; set => SetField(ref _serverHost, value, nameof(ServerHost)); }
        
        public List<string> ServerVersion { get; } = new List<string> { "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019" };

        public Dictionary<string, string> RestUrls { get; } = new Dictionary<string, string>()
        {
           {"2012","/RevitServerAdminRESTService/AdminRESTService.svc"},
           {"2013","/RevitServerAdminRESTService2013/AdminRESTService.svc"},
           {"2014","/RevitServerAdminRESTService2014/AdminRESTService.svc"},
           {"2015","/RevitServerAdminRESTService2015/AdminRESTService.svc"},
           {"2016","/RevitServerAdminRESTService2016/AdminRESTService.svc"},
           {"2017","/RevitServerAdminRESTService2017/AdminRESTService.svc"},
           {"2018","/RevitServerAdminRESTService2018/AdminRESTService.svc"},
           {"2019","/RevitServerAdminRESTService2019/AdminRESTService.svc"}
        };

        public string CurrentSelectionServerVersion { get; set; }
        public string RestUrl { get => RestUrls[CurrentSelectionServerVersion]; }

        private LoaderType loaderType = LoaderType.Rest;

        public LoaderType LoaderType
        {
            get { return loaderType; }
            set
            {
                if (loaderType == value)
                    return;

                loaderType = value;
                OnPropertyChanged("LoaderType");
                OnPropertyChanged("IsProxyType");
                OnPropertyChanged("IsRestType");
            }
        }

        public bool IsProxyType
        {
            get { return LoaderType == LoaderType.Proxy; }
            set { LoaderType = value ? LoaderType.Proxy : LoaderType; }
        }

        public bool IsRestType
        {
            get { return LoaderType == LoaderType.Rest; }
            set { LoaderType = value ? LoaderType.Rest : LoaderType; }
        }

    }
}
