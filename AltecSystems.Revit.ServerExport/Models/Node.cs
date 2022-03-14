using AltecSystems.Revit.ServerExport.Command;
using System;
using System.Collections.ObjectModel;

namespace AltecSystems.Revit.ServerExport.Models
{
    internal class Node : NotifyPropertyChangedBase
    {
        public Node()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ObservableCollection<Node> Children { get; set; } = new ObservableCollection<Node>();
        public Node Parent { get; set; }

        public string Id { get; set; }
        public string Path { get; set; }
        public bool IsModel { get; set; }
        private string _text;
        private bool _isChecked;
        private bool _isExpanded;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                foreach (var item in Children)
                {
                    item.IsChecked = value;
                }
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
    }
}