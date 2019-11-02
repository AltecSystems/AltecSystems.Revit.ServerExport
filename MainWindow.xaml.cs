using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Controls = System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevitServerExport
{
	
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		FolderBrowserDialog fbd = new FolderBrowserDialog();
		public static MainWindow re;
        public MainWindow()
        {
            InitializeComponent();
			//re = this;
            //tree.ItemsSource = Node.FillingTree(5);
           // DataContext = new PathFinderViewer();
			
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Controls.CheckBox currentCheckBox = (Controls.CheckBox)sender;
            CheckBoxId.checkBoxId = currentCheckBox.Uid;

        }

		private void Tree_Expanded(object sender, RoutedEventArgs e)
		{
			//PathFinderViewer pfw = new PathFinderViewer();
			//pfw.FillCommand()
		}
	}
}
