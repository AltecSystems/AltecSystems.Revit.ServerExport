using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace RevitServerExport
{
	class PathFinderViewer : INotifyPropertyChanged
	{
		public ObservableCollection<PathFinder> pathFinders { get; set; }
		public ObservableCollection<Node> Nodes { get; set; }
		private string file { get; set; }
		public PathFinderViewer()
		{
			//file = Properties.Resources.tree;
			file = Directory.GetCurrentDirectory() + "\\tree.xml";
			pathFinders = new ObservableCollection<PathFinder>() { new PathFinder() };
			if (File.Exists(file))
				Nodes = Serializator.Deserialize<ObservableCollection<Node>>(file);
			OnPropertyChanged();
		}
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

		//	return result;
		//}



		private RelayCommand syncCommand;
		public RelayCommand SyncCommand
		{
			get
			{
				return syncCommand ?? (syncCommand = new RelayCommand(obj =>
				   {
					   var root = "http://" + pathFinders.First().Ip + pathFinders.First().Version;
					   var folder = "/|";

					   Nodes = NodeViewer.FillingRoot(root, folder);
					   ParallelLoopResult res = Parallel.ForEach(Nodes, i =>
					   {
						   i.Children = NodeViewer.FillingTree(root + folder, i.Text + "|");
						   foreach (var item in i.Children)
							   item.Parent = i;

					   });
					   Serializator.Serialize(file, Nodes);
					   OnPropertyChanged("Nodes");
				   }));
			}
		}

		private RelayCommand exportCommand;
		public RelayCommand ExportCommand
		{
			get
			{
				return exportCommand ?? (exportCommand = new RelayCommand(obj =>
				{
					var checkedNodes = Nodes.Where(x => x.IsChecked == true || x.IsChecked == null);
					var paths = BatBuilder.CombinePath(Nodes, @"E:\ProgramData\Revit server");
					var snd = BatBuilder.Splitter(paths, @"E:\ProgramData\Revit server");
					BatBuilder.CreateBat(snd, pathFinders.First().Ip, pathFinders.First().Destination);
					BatBuilder.RunBat(pathFinders.First().RevitRoot);
					BatBuilder.CheckLoaded(snd, pathFinders.First().Destination, pathFinders.First().RevitRoot);					
				}));
			}
		}

		private RelayCommand destinCommand;
		public RelayCommand DestinCommand
		{
			get
			{
				return destinCommand ?? (destinCommand = new RelayCommand(obj =>
				{
					Forms.FolderBrowserDialog fbd = new Forms.FolderBrowserDialog();
					if (fbd.ShowDialog() == Forms.DialogResult.OK)
					{
						pathFinders.First().Destination = fbd.SelectedPath;
					}
					
				}));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
