using AltecSystems.Revit.ServerExport.Command;
using AltecSystems.Revit.ServerExport.Models;
using AltecSystems.Revit.ServerExport.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AltecSystems.Revit.ServerExport
{
    internal class ServerExportViewModel : NotifyPropertyChangedBase
    {
        public ICommand DestinCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand LoadModelCommand { get; }

        private readonly ModelLoader _loader;
        private readonly ExportModel _export;

        public ObservableCollection<Node> Nodes { get; set; }

        private readonly SettingsManager _settingsManager;

        public ProgressModel Progress { get; set; }

        public SettingsModel Settings { get; set; }

        public ServerExportViewModel()
        {
            _settingsManager = new SettingsManager();
            Settings = _settingsManager.GetSettings();
            Nodes = new ObservableCollection<Node>(); //new ObservableCollection<Node>(_settingsManager.GetCasheNodes());
            DestinCommand = new RelayCommand(null,null);
            ExportCommand = new RelayCommand(Export, null);
            LoadModelCommand = new RelayCommand(StartLoadModel, null);
            _loader = new ModelLoader();
            _export = new ExportModel();
            Progress = new ProgressModel();
          
            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Export(object obj)
        {
            Export2.Export();
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           _settingsManager.SaveSettings(Settings);   
        }

        private async void StartLoadModel(object commandParam)
        {
            var root = "http://" + Settings.ServerHost + Settings.RestUrl;
            var rootfolder = root + "/|";
            Progress.IsVisibility = System.Windows.Visibility.Visible;
            Progress.IsIndeterminate = true;
            await LoadModelAsync(Nodes,rootfolder);
            Progress.IsVisibility = System.Windows.Visibility.Collapsed;
            // _settingsManager.SaveCache(Nodes);
        }
        

        private async Task LoadModelAsync(ObservableCollection<Node> nodes, string url)
        {
            var model = await _loader.LoadModelAsync(url);

            if (Progress.IsIndeterminate)
                Progress.IsIndeterminate = false;

            Progress.Max += model.Folders.Count;
            
            foreach (var item in model.Models)
            {
                nodes.Add(new Node() { Text = item.Name });
            }
            foreach (var item in model.Folders)
            {
                var node = new Node() { Text = item.Name };
                string newurl;
                if (url.EndsWith("/|"))
                {
                    newurl = url + node.Text;
                }
                else
                {
                    newurl = url + "|" + node.Text;
                }
                
                nodes.Add(node);
                Progress.CurrentProgress++;
                await LoadModelAsync(node.Children, newurl);
            }
        } 

        //public ICommand DestinCommand
        //{
        //    get
        //    {
        //        return destinCommand ?? (destinCommand = new RelayCommand(obj =>
        //        {
        //            //Forms.FolderBrowserDialog fbd = new Forms.FolderBrowserDialog();
        //            //if (fbd.ShowDialog() == Forms.DialogResult.OK)
        //            //{
        //            //    PathFinders.First().Destination = fbd.SelectedPath;
        //            //}
        //        }));
        //    }
        //}

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