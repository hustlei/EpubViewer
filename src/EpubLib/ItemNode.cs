using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Lei.Common.Base;

namespace Lei.Common
{
    public class ItemNode : PropertyChangedBase
    {
        public static ImageSource ExpandedIcon;
        public static ImageSource CollepsedIcon;
        public static ImageSource PageIcon;
        public static ImageSource ExpandedIconSelected;
        public static ImageSource CollepsedIconSelected;
        public static ImageSource PageIconSelected;
        public static ItemNode selectedNode = null;

        static ItemNode()
        {
            try
            {
                CollepsedIcon =
                    BitmapFrame.Create(new Uri("pack://application:,,,/EpubLib;component/img/Book_angle.png", UriKind.Absolute));
                ExpandedIcon =
                    BitmapFrame.Create(new Uri("pack://application:,,,/EpubLib;component/img/Book_open.png", UriKind.RelativeOrAbsolute));
                PageIcon =
                    new BitmapImage(new Uri("/EpubLib;component/img/PageUnselect.png", UriKind.RelativeOrAbsolute));
                CollepsedIconSelected =
                    new BitmapImage(new Uri("/EpubLib;component/img/Book_angle.png", UriKind.RelativeOrAbsolute));
                ExpandedIconSelected =
                    new BitmapImage(new Uri("/EpubLib;component/img/Book_open.png", UriKind.RelativeOrAbsolute));
                PageIconSelected =
                    new BitmapImage(new Uri("/EpubLib;component/img/Page.png", UriKind.RelativeOrAbsolute));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public ItemNode()
        {
            Nodes = new BindableCollection<ItemNode>();
            _isExpanded = false;
            _isSelected = false;
            Nodes.CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    ((ItemNode)args.NewItems[0]).Parent = this;
                }
            };
        }

        public ItemNode(string text)
            : this()
        {
            Text = text;
        }
        public ItemNode(string text, string name, ImageSource icon)
            : this()
        {
            Name = name;
            _text = text;
            _icon = icon;
        }

        private string _text;
        public string Text { get { return _text; } set { _text = value; NotifyOfPropertyChange("Text"); } }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get { return _icon; }
            set { _icon = value; NotifyOfPropertyChange("Icon"); }
        }

        private bool _isExpanded;

        public bool? IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value == true;
                if (Nodes.Count > 0)
                {
                    Icon = _isExpanded ? ExpandedIcon : CollepsedIcon;
                }
                NotifyOfPropertyChange("IsExpanded");
            }
        }

        private bool _isSelected;

        public bool? IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value == true;
                if (_isSelected)
                {
                    selectedNode.IsSelected = false;
                    selectedNode = this;
                }
                NotifyOfPropertyChange("IsSelected");
                if (Nodes.Count == 0)
                    Icon = _isSelected ? PageIconSelected : PageIcon;
                else if (_isExpanded)
                    Icon = _isSelected ? ExpandedIconSelected : ExpandedIcon;
                else
                    Icon = _isSelected ? CollepsedIconSelected : CollepsedIcon;
                if (_isSelected)
                {
                    var n = this;
                    while (true)
                    {
                        if (n.Parent == null)
                            return;
                        n.Parent.IsExpanded = true;
                        n = n.Parent;
                    }
                }
            }
        }
        public BindableCollection<ItemNode> Nodes { get; set; }

        public string Name { get; set; }
        public object Tag { get; set; }
        public ItemNode Parent { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //private void NotifyPropertyChanged(string propertyName)
        //{
        //    if (this.PropertyChanged != null)
        //    {
        //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
