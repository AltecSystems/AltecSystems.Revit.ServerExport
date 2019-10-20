using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitServerExport
{
    class FillingTree
    {
        public static ObservableCollection<Node> FillingTreeSample(int lvlCount = 5)
        {
            ObservableCollection<Node> collect = new ObservableCollection<Node>();
            for (int i = 0; i < 5; i++)
            {
                collect.Add(new Node() { Text = "Ур. " + lvlCount.ToString() + " Э" + (i + 1).ToString() });
            }

            if (lvlCount > 0)
                foreach (Node item in collect)
                {
                    item.Children = FillingTreeSample(lvlCount - 1);
                    foreach (var child in item.Children)
                        child.Parent = item;
                }
            return collect;
        }
    }
}
