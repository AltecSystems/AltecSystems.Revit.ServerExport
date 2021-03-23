using AltecSystems.Revit.ServerExport.Command;
using System.Windows;

namespace AltecSystems.Revit.ServerExport.Models
{
    internal class ProgressModel : NotifyPropertyChangedBase
    {
        private int _max;

        public int Max
        {
            get => _max; set
            {
                SetField(ref _max, value, nameof(Max));
                OnPropertyChanged(nameof(StatusText));
            }
        }

        private int _currentProgress;

        public int CurrentProgress
        {
            get => _currentProgress; set
            {
                SetField(ref _currentProgress, value, nameof(CurrentProgress));
                OnPropertyChanged(nameof(StatusText));
            }
        }

        private bool _isIndeterminate;

        public bool IsIndeterminate
        {
            get => _isIndeterminate; set
            {
                SetField(ref _isIndeterminate, value, nameof(IsIndeterminate));
                OnPropertyChanged(nameof(StatusText));
            }
        }

        private Visibility _isVisible = Visibility.Collapsed;
        public Visibility IsVisibility { get => _isVisible; set => SetField(ref _isVisible, value, nameof(IsVisibility)); }

        public string StatusText
        {
            get
            {
                return IsIndeterminate ? "Ожидание загрузки" : $"Загружено {CurrentProgress} из {Max}";
            }
        }
    }
}