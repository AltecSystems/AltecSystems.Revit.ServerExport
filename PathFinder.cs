using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitServerExport
{
    class PathFinder : INotifyPropertyChanged
    {
		public PathFinder(string ip, string version, string extra = "\\contents")
		{
			this.Version = version;
			this.Ip = ip;
			this.Extra = extra;
			OnPropertyChanged("");
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
    #warning Implement combination of path to server
        private string ip = "192.168.1.10";
        public string Ip
        {
            get { return ip; }
            set
            {
                ip = value;
				OnPropertyChanged("Text");
    #warning regex for proper ip or URL
            }
        }

        private string version = "/RevitServerAdminRESTService2019/AdminRESTService.svc";
        public string Version
        {
            get { return version; }
            set
			{
				version = KeysValues[value];
				OnPropertyChanged("SelectedItem");
			}
        }
        private string extra = "/|/contents";
        public string Extra
        {
            get { return extra; }
            set { extra = value; }
    #warning regex for proper extras

        }
        private string completePath;
        public string CompletePath
        {
            get { return completePath; }
            set { completePath = this.ip + this.version + this.extra; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
