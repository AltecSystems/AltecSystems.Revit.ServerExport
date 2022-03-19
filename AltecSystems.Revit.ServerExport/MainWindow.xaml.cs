using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace AltecSystems.Revit.ServerExport
{
    public partial class ServerExportView : Window
    {
        public ServerExportView()
        {
            DataContext = new ServerExportViewModel();
            InitializeComponent();

            App.LanguageChanged += LanguageChanged;

            CultureInfo currLang = App.Language;

            //Fill in the language change menu:
            menuLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            CultureInfo currentLanguage = App.Language;
            //Mark the desired language change item as the selected language
            foreach (MenuItem menuItem in menuLanguage.Items)
            {
                menuItem.IsChecked = menuItem.Tag is CultureInfo cultureInfo && cultureInfo.Equals(currentLanguage);
            }
        }

        private void ChangeLanguageClick(object sender, EventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                if (menuItem.Tag is CultureInfo language)
                {
                    App.Language = language;
                }
            }
        }
    }
}