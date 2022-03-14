using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using AltecSystems.Revit.ServerExport.Command;
using AltecSystems.Revit.ServerExport.Models;
using AltecSystems.Revit.ServerExport.Services;

namespace AltecSystems.Revit.ServerExport
{
    internal class ServerExportViewModel : NotifyPropertyChangedBase
    {
        public ICommand DestinCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand LoadModelCommand { get; }

        public ObservableCollection<Node> Nodes { get; set; }

        private readonly SettingsManager _settingsManager;

        public ProgressModel Progress { get; set; }

        public SettingsModel Settings { get; set; }

        public ServerExportViewModel()
        {
            _settingsManager = new SettingsManager();
            Settings = _settingsManager.GetSettings();
            Nodes = new ObservableCollection<Node>();
            DestinCommand = new RelayCommand(null, null);
            ExportCommand = new RelayCommand(Export, null);
            LoadModelCommand = new RelayCommand(StartLoadModelAsync, null);
            Progress = new ProgressModel();

            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Export(object obj)
        {
            var exportCredentials = GetExportCredentials(Nodes, new List<ExportCredential>());
            foreach (var credential in exportCredentials)
            {
                var export = new ExportModels(credential);
                try
                {
                    export.Export();
                }
                catch (FaultException)
                {
                    MessageBox.Show("Произошла ошибка при выгрузке. Проверьте версию revit");
                }
                catch (Exception)
                {
                    MessageBox.Show("Произошла ошибка при выгрузке.");
                }
            }
            MessageBox.Show("Выгрузка завершена");
        }

        private IEnumerable<ExportCredential> GetExportCredentials(ObservableCollection<Node> nodes, List<ExportCredential> credentials)
        {
            foreach (var node in nodes)
            {
                if (!node.IsModel && node.Children.Any())
                {
                    GetExportCredentials(node.Children, credentials);
                }
                else if (node.IsModel && node.IsChecked)
                {
                    var savePath = GetSavePath(node.Path);
                    credentials.Add(new ExportCredential(Settings.ServerHost, node.Path, savePath, Settings.CurrentSelectionServerVersion));
                }
            }
            return credentials;
        }

        private string GetSavePath(string modelPath)
        {
            return modelPath.Replace(Settings.RevitServerRootPath, Settings.SavePath);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _settingsManager.SaveSettings(Settings);
        }

        private async void StartLoadModelAsync(object commandParam)
        {
            Progress.IsVisibility = System.Windows.Visibility.Visible;
            Progress.IsIndeterminate = true;
            switch (Settings.LoaderType)
            {
                case LoaderType.Rest:
                {
                    var loader = new RestApiModelLoader(Settings);
                    await loader.LoadModelAsync(Nodes, Progress);
                    break;
                }
                case LoaderType.Proxy:
                {
                    var loader = new ProxyModelLoader(Settings);
                    await loader.LoadModelAsync(Nodes, Progress);
                    break;
                }
                default:
                    break;
            }
            Progress.IsVisibility = System.Windows.Visibility.Collapsed;
        }
    }
}