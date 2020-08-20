using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;

namespace RevitServerExport
{
    internal class PathFinderViewer : INotifyPropertyChanged
    {
        private RelayCommand destinCommand;
        private RelayCommand exportCommand;
        private RelayCommand syncCommand;

        public PathFinderViewer()
        {
            //file = Properties.Resources.tree;
            File = Directory.GetCurrentDirectory() + "\\tree.xml";
            PathFinders = new ObservableCollection<PathFinder>() { new PathFinder() };
            if (System.IO.File.Exists(File))
                Nodes = Serializator.Deserialize<ObservableCollection<Node>>(File);
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand DestinCommand
        {
            get
            {
                return destinCommand ?? (destinCommand = new RelayCommand(obj =>
                {
                    Forms.FolderBrowserDialog fbd = new Forms.FolderBrowserDialog();
                    if (fbd.ShowDialog() == Forms.DialogResult.OK)
                    {
                        PathFinders.First().Destination = fbd.SelectedPath;
                    }
                }));
            }
        }

        public RelayCommand ExportCommand
        {
            get
            {
                return exportCommand ?? (exportCommand = new RelayCommand(obj =>
                {
                    Properties.Settings.Default.Save();
                    var checkedNodes = Nodes.Where(x => x.IsChecked == true || x.IsChecked == null);
                    var rootFolder = Path.GetDirectoryName($"{Root}\\");
                    var paths = BatBuilder.CombinePath(Nodes, rootFolder);
                    var snd = BatBuilder.Splitter(paths, rootFolder);
                    BatBuilder.CreateBat(snd, PathFinders.First().Ip, PathFinders.First().Destination);
                    BatBuilder.RunBat(PathFinders.First().RevitRoot);
                    BatBuilder.CheckLoaded(snd, PathFinders.First().Destination, PathFinders.First().RevitRoot);
                }));
            }
        }

        public ObservableCollection<Node> Nodes { get; set; }
        public ObservableCollection<PathFinder> PathFinders { get; set; }

        //	return result;
        //}
        public string Root
        {
            get => Properties.Settings.Default.Root;
            set
            {
                if (value != Properties.Settings.Default.Root)
                {
                    Properties.Settings.Default.Root = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand SyncCommand
        {
            get
            {
                return syncCommand ?? (syncCommand = new RelayCommand(obj =>
                   {
                       var root = "http://" + PathFinders.First().Ip + PathFinders.First().Version;
                       var folder = "/|";

                       Nodes = NodeViewer.FillingRoot(root, folder);
                       ParallelLoopResult res = Parallel.ForEach(Nodes, i =>
                       {
                           i.Children = NodeViewer.FillingTree(root + folder, i.Text + "|");
                           foreach (var item in i.Children)
                               item.Parent = i;
                       });
                       Serializator.Serialize(File, Nodes);
                       OnPropertyChanged("Nodes");
                   }));
            }
        }

        private string File { get; set; }

        //public bool IsAdmin()
        //{
        //	bool result = false;
        //	try
        //	{
        //		result = (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        //	}
        //	catch (Exception e)
        //	{
        //		MessageBox.Show(e.Message);
        //	}
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}