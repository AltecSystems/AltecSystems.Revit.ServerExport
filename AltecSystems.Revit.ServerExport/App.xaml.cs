using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace AltecSystems.Revit.ServerExport
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private static readonly List<CultureInfo> _languages = new List<CultureInfo>();

        public static List<CultureInfo> Languages => _languages;

        //Event to notify all application windows
        public static event EventHandler LanguageChanged;

        public App()
        {
            InitializeComponent();
            LanguageChanged += App_LanguageChanged;

            _languages.Clear();
            _languages.Add(new CultureInfo("en-US")); //Neutral culture for this project
            _languages.Add(new CultureInfo("ru-RU"));

            Language = ServerExport.Properties.Settings.Default.DefaultLanguage;
        }

        public static CultureInfo Language
        {
            get => System.Threading.Thread.CurrentThread.CurrentUICulture;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture)
                {
                    return;
                }

                //1. Changing the application language:
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;

                //2. Create a ResourceDictionary for the new culture
                ResourceDictionary dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                    {
                        dict.Source = new Uri(string.Format("ResourceDictionary/lang.{0}.xaml", value.Name), UriKind.Relative);
                        break;
                    }
                    default:
                    {
                        dict.Source = new Uri("ResourceDictionary/lang.xaml", UriKind.Relative);
                        break;
                    }
                }

                //3. Find the old ResourceDictionary and delete it and add a new ResourceDictionary
                ResourceDictionary oldDict = Current.Resources.MergedDictionaries
                    .Where(x => x.Source != null && x.Source.OriginalString.StartsWith("ResourceDictionary"))
                    .First();

                if (oldDict != null)
                {
                    int ind = Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Current.Resources.MergedDictionaries.Remove(oldDict);
                    Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Current.Resources.MergedDictionaries.Add(dict);
                }

                //4. Call an event to notify all windows.
                LanguageChanged(Current, new EventArgs());
            }
        }

        private void App_LanguageChanged(object sender, EventArgs e)
        {
            ServerExport.Properties.Settings.Default.DefaultLanguage = Language;
            ServerExport.Properties.Settings.Default.Save();
        }
    }
}