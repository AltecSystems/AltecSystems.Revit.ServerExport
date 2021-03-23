using System.Windows;

namespace AltecSystems.Revit.ServerExport
{
    public partial class ServerExportView : Window
    {
        public ServerExportView()
        {
            DataContext = new ServerExportViewModel();
            InitializeComponent();
        }
    }
}