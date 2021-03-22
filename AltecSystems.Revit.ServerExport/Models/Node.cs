using AltecSystems.Revit.ServerExport.Command;
using System;
using System.Collections.Generic;
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
        public List<Node> ListChildren { get; set; } = new List<Node>();

        public string Id { get; set; }
        private string text;
        private bool isChecked;
        private bool isExpanded;

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                foreach (var item in Children)
                {
                    item.IsChecked = value;
                }
                OnPropertyChanged(nameof(IsChecked));
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));

            }
        }
    }
}
