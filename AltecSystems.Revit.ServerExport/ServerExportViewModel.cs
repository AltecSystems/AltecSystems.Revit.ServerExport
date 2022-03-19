using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using AltecSystems.Revit.ServerExport.Command;
using AltecSystems.Revit.ServerExport.Models;
using AltecSystems.Revit.ServerExport.Services;
using Autodesk.RevitServer.Enterprise.Common.ClientServer.Helper.Exceptions;

namespace AltecSystems.Revit.ServerExport
{
    internal class ServerExportViewModel : NotifyPropertyChangedBase
    {
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
                catch (FaultException ex)
                {
                    MessageBox.Show(Properties.ExceptionMessages.RevitServerVersionException);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Properties.ExceptionMessages.UploadinModelExceprion);
                }
            }
            MessageBox.Show(Properties.ApplicationMessages.UploadCompletedMessage);
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
            return Path.Combine(Settings.SavePath, modelPath);
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _settingsManager.SaveSettings(Settings);
        }

        private async void StartLoadModelAsync(object commandParam)
        {
            Nodes.Clear();
            Progress.Clear();

            Progress.IsVisibility = Visibility.Visible;
            Progress.IsIndeterminate = true;

            try
            {
                var loader = new ProxyModelLoader(Settings);
                await loader.LoadModelAsync(Nodes, Progress);
            }
            catch (ProxyGenerationException ex)
            {
                MessageBox.Show(string.Format(Properties.ExceptionMessages.ProxyGenerationException, Settings.CurrentSelectionServerVersion));
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Progress.IsVisibility = Visibility.Collapsed;
        }
    }
}