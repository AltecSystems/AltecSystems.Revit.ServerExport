using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RevitServerExport
{
    class PathFinderViewer : INotifyPropertyChanged
    {
        public PathFinderViewer()
        {
            paths = new ObservableCollection<PathFinder>() { Path };
			OnPropertyChanged();
		}
		private ObservableCollection<PathFinder> paths;
		public ObservableCollection<PathFinder> Paths
		{
			get { return paths; }
			set { paths = value; OnPropertyChanged(); }
		} 

		private PathFinder path;
        public PathFinder Path
		{
			get { return path; }
			set { path = value; OnPropertyChanged(); }
		}
        public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
