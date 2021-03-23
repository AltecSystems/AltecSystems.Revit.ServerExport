using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace AltecSystems.Revit.ServerExport.Services
{
    internal class PathFinder : INotifyPropertyChanged
    {
        public PathFinder()
        {
            OnPropertyChanged();
        }

        private Dictionary<string, string> KeysValues = new Dictionary<string, string>()
        {
           {"2012",    "/RevitServerAdminRESTService/AdminRESTService.svc"},
           {"2013","/RevitServerAdminRESTService2013/AdminRESTService.svc"},
           {"2014","/RevitServerAdminRESTService2014/AdminRESTService.svc"},
           {"2015","/RevitServerAdminRESTService2015/AdminRESTService.svc"},
           {"2016","/RevitServerAdminRESTService2016/AdminRESTService.svc"},
           {"2017","/RevitServerAdminRESTService2017/AdminRESTService.svc"},
           {"2018","/RevitServerAdminRESTService2018/AdminRESTService.svc"},
           {"2019","/RevitServerAdminRESTService2019/AdminRESTService.svc"}
        };

        public ObservableCollection<string> SupportedVersionsKeys
        {
            get
            {
                return new ObservableCollection<string>()
                    {
                      {"2012"},
                      {"2013"},
                      {"2014"},
                      {"2015"},
                      {"2016"},
                      {"2017"},
                      {"2018"},
                      {"2019"},
                    };
            }
            set
            {
            }
        }

        //private string ip = Properties.Settings.Default.ServerIP;
        private string ip = "";

        public string Ip
        {
            get
            {
                return ip;
            }
            set
            {
                if (IPAddress.TryParse(value, out IPAddress ipp))
                {
                    ip = ipp.ToString();
                    //Properties.Settings.Default.ServerIP = ipp.ToString();
                }
                else
                { MessageBox.Show("Ip-адрес введен некорректно"); }
                OnPropertyChanged("Text");
            }
        }

        private string version;//= "/RevitServerAdminRESTService2019/AdminRESTService.svc";

        public string Version
        {
            get { return version; }
            set
            {
                version = KeysValues[value];
                OnPropertyChanged("SelectedItem");
            }
        }

        //private string destination = Properties.Settings.Default.Destination;
        private string destination = "";

        public string Destination
        {
            get { return destination; }
            set
            {
                destination = value;
                //Properties.Settings.Default.Destination = value;
                OnPropertyChanged("Destination");
            }
        }

        private string revitRoot = @"C:\Program Files\Autodesk\Revit 2019\";

        public string RevitRoot
        {
            get { return revitRoot; }
            set
            {
                Regex reg = new Regex(@"^[a-zA-Z]:\\[\\\S|*\S]?.*$");
                if (reg.IsMatch(value))
                    revitRoot = value;
                OnPropertyChanged("Text");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}