using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AltecSystems.Revit.ServerExport.Command
{
    internal class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            var comparer = EqualityComparer<T>.Default;
            bool equals = comparer.Equals(value, field);
            if (!equals)
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
            return !equals;
        }
    }
}