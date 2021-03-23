using AltecSystems.Revit.ServerExport.Command;
using AltecSystems.Revit.ServerExport.Models;
using AltecSystems.Revit.ServerExport.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

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
            DestinCommand = new RelayCommand(null,null);
            ExportCommand = new RelayCommand(Export, null);
            LoadModelCommand = new RelayCommand(StartLoadModelAsync, null);
            Progress = new ProgressModel();
          
            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Export(object obj)
        {
            var t = "";
           //Export2.Export();
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

        //public ICommand ExportCommand
        //{
        //    get
        //    {
        //        return exportCommand ?? (exportCommand = new RelayCommand(obj =>
        //        {
        //            //Properties.Settings.Default.Save();
        //            var checkedNodes = Nodes.Where(x => x.IsChecked == true || x.IsChecked == null);
        //            var rootFolder = Path.GetDirectoryName($"{Root}\\");
        //            var paths = BatBuilder.CombinePath(Nodes, rootFolder);
        //            var snd = BatBuilder.Splitter(paths, rootFolder);
        //            BatBuilder.CreateBat(snd, PathFinders.First().Ip, PathFinders.First().Destination);
        //            BatBuilder.RunBat(PathFinders.First().RevitRoot);
        //            BatBuilder.CheckLoaded(snd, PathFinders.First().Destination, PathFinders.First().RevitRoot);
        //        }));
        //    }
        //}
    }
}