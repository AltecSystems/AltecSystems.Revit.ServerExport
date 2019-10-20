﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RevitServerExport
{
    public class Node : INotifyPropertyChanged
    {
        public Node()
        {
            Id = Guid.NewGuid().ToString();
        }
        public ObservableCollection<Node> Children { get; set; } = new ObservableCollection<Node>();
        public Node Parent { get; set; }
        public string Id { get; set; }
        private string text;
        private bool? isChecked = false;
        private bool isExpanded;

        public bool? IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                RaisePropertyChanged("IsChecked");
                //RaisePropertyChanged();
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                //RaisePropertyChanged("Text");
                RaisePropertyChanged();
            }
        }
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                //RaisePropertyChanged("IsExpanded");
                RaisePropertyChanged();

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (this.Id == CheckBoxId.checkBoxId && this.Children.Count != 0)
                ChangeChildren(this.Children, this.IsChecked);

            if (this.Id == CheckBoxId.checkBoxId && this.Parent != null)
                ChangeParent(this, this.IsChecked);
        }
        private void ChangeChildren(ObservableCollection<Node> items, bool? isChecked)
        {
            foreach (Node item in items)
            {
                item.IsChecked = isChecked;

                if (item.Children.Count != 0)
                    ChangeChildren(item.Children, isChecked);
            }
        }
        private void ChangeParent(Node item, bool? isChecked)
        {
            if (item.Parent != null)
            {
                if (item.Parent.Children.Count != 0)
                {
                    if (item.Parent.Children.All(x => x.isChecked == true))
                    {
                        item.Parent.IsChecked = isChecked;
                        ChangeParent(item.Parent, isChecked);
                    }
                    else if (item.Parent.Children.Any(x => x.isChecked == true))
                    {
                        item.Parent.IsChecked = null;
                        ChangeParent(item.Parent, null);
                    }
                    if (item.Parent.Children.Any(x => x.isChecked == null))
                    {
                        item.Parent.IsChecked = null;
                        ChangeParent(item.Parent, null);
                    }
                    if (item.Parent.Children.All(x => x.isChecked == false))
                    {
                        item.Parent.IsChecked = isChecked;
                        ChangeParent(item.Parent, isChecked);
                    }
                }
            }

        }
    }
    public struct CheckBoxId
    {
        public static string checkBoxId;
    }
}
