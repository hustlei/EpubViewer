using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Caliburn.Micro;
using winform = System.Windows.Forms;
using System.Threading.Tasks;
using CefSharp;
using Lei.Common;

namespace EpubViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(MainViewModel))]
    public sealed class MainViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly IWindowManager _windowManager;
        private readonly EpubService _epubService;
        private readonly winform.OpenFileDialog _openDlg;
        static TaskScheduler TS = TaskScheduler.FromCurrentSynchronizationContext();
        private BindableCollection<ItemNode> _nodes; 
        public BindableCollection<ItemNode> Nodes
        {
            get { return _nodes; }
            private set { _nodes = value; NotifyOfPropertyChange("Nodes"); }
        }

        private Visibility _waitingVisible;
        public Visibility WaitingVisible
        {
            get { return _waitingVisible; }
            set { _waitingVisible = value; NotifyOfPropertyChange("WaitingVisible"); }
        }
        public bool AllowDrop { get; set; }
        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {
            DisplayName = "Epub Viewer 1.1";
            _windowManager = windowManager;
            _epubService = new EpubService();
            _openDlg = new winform.OpenFileDialog
            {
                Filter = "EPUB Files(*.epub)|*.epub|EPUB Files(*.epub3)|*.epub3",
                Multiselect = true,
                RestoreDirectory = true
            };
            WaitingVisible = Visibility.Collapsed;
            AllowDrop = true;
            Nodes = new BindableCollection<ItemNode>();
            SearchResult = new BindableCollection<ItemNode>();
        }
        public void FileOpen()
        {
            if (_openDlg.ShowDialog() == winform.DialogResult.OK)
            {
                if (_openDlg.FileNames.Length > 0 && _openDlg.FileNames[0].Length > 0)
                {
                    WaitingVisible = Visibility.Visible;
                    var t = _epubService.OpenFilesAsync(_openDlg.FileNames);
                    t.ContinueWith(t1 =>
                    {
                        if (t1.Result)
                        {
                            try
                            {
                                Nodes.Add(_epubService.EpubList[_epubService.EpubList.Count - 1].TocNode);
                                Nodes[Nodes.Count - 1].IsExpanded = true;
                                Nodes[Nodes.Count - 1].IsSelected = true;
                                Nodes[0].Icon = ItemNode.ExpandedIcon;
                                if (_epubService.EpubList.Count > 0 && _epubService.EpubList[0].IsSpine)
                                    ((ContentTabItemViewModel)ActiveItem).UseDocumentTitle = true;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                        }
                        WaitingVisible = Visibility.Collapsed;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
        public void FileClose()
        {
            _epubService.CloseFiles();
            Items.Clear();
        }
        public void Config()
        {
            var settings = new Dictionary<string, object> { { "Topmost", true }, { "Owner", GetView() } };//Owner不是依赖属性不能绑定,只能在这里设置
            _windowManager.ShowDialog(new ConfigViewModel(), null, settings);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            FileClose();
            Environment.Exit(0);
        }
        public void Drop(DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            //string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            _epubService.OpenFiles(new string[] { fileName });
        }

        public void DragEnter(DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }


        #region 左边栏treeview相关函数
        public void TocSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            ItemNode node = e.NewValue as ItemNode;
            try
            {
                string url = "";
                string text = "";

                if (node != null)
                {
                    url = node.Name;
                    text = node.Text;
                }
                if (url != "")
                {
                    if (ActiveItem == null)
                        NewTab();
                    ((ContentTabItemViewModel)ActiveItem).Address = url;
                    ((ContentTabItemViewModel)ActiveItem).DisplayName = text;
                }
                else
                {
                    MessageBox.Show("未找到页面。");
                }
            }
            catch
            {
            }
        }
        # endregion


        #region 内容主题tabcontrol相关函数
        public void NewTab()
        {
            var item = new ContentTabItemViewModel();
            ActivateItem(item);
        }
        public void CloseTab(RoutedEventArgs e)
        {
            this.CloseItem(((FrameworkElement)e.OriginalSource).DataContext);
        }
        #endregion

        public void Back()
        {
            if (ActiveItem == null)
                return;
            var browser = ((ContentTabItemViewModel)ActiveItem).WebBrowser;
            if (browser.CanGoBack)
                browser.Back();
        }
        public void Forward()
        {
            if (ActiveItem == null)
                return;
            var browser = ((ContentTabItemViewModel)ActiveItem).WebBrowser;
            if (browser.CanGoForward)
                browser.Forward();
        }
        public void SyncToc()
        {
            if (Nodes.Count < 1)
                return;
            if (ActiveItem == null)
                return;
            var browser = ((ContentTabItemViewModel)ActiveItem).WebBrowser;
            string url = browser.Address;
            Task.Factory.StartNew(() =>
            {
                foreach (ItemNode t in Nodes)
                {
                    var node = EpubBook.SearchNodeName(t, url);
                    if (node != null)
                    {
                        node.IsSelected = true;
                        return;
                    }
                }
            });
        }
        public void Print()
        {
            if (ActiveItem == null)
                return;
            var browser = ((ContentTabItemViewModel)ActiveItem).WebBrowser;
            browser.Print();
        }
        public void Encoding_Click(object sender, RoutedEventArgs e)
        {
            //item.Document.Body.Style += "fontzise:120%;";
        }

        public void ZoomIn()
        {
            if (ActiveItem == null)
                return;
            var browser = ((ContentTabItemViewModel)ActiveItem).WebBrowser;
            browser.ZoomLevel++;
        }
        public void ZoomOut()
        {
            if (ActiveItem == null)
                return;
            var browser = ((ContentTabItemViewModel)ActiveItem).WebBrowser;
            browser.ZoomLevel--;
        }
        public void Find()
        {
            if (ActiveItem != null)
            {
                var w = new FindViewModel(((ContentTabItemViewModel)ActiveItem).WebBrowser);
                _windowManager.ShowDialog(w);
            }
        }


        //about dialog display in view.xaml.cs
        //private void Info_Click(object sender, RoutedEventArgs e)
        //{
        //    about dlg = new about();
        //    dlg.Owner = this;
        //    dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //    dlg.Topmost = true;
        //    dlg.ShowDialog();
        //}

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; NotifyOfPropertyChange("SearchText"); }
        }

        private BindableCollection<ItemNode> _searchResult; 
        public BindableCollection<ItemNode> SearchResult {
            get { return _searchResult; }
            set { _searchResult = value;NotifyOfPropertyChange("SearchResult"); }
        }

        public void Search()
        {
            SearchResult.Clear();
            if (string.IsNullOrEmpty(_searchText))
            {
                return;
            }
            foreach (ItemNode t in Nodes)
            {
                EpubBook.SearchTopic(t, _searchText, SearchResult);
            }
            if (SearchResult.Count == 0)
                SearchResult.Add(new ItemNode("没有搜索到内容"));
        }

        public void SearchTextKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }
        public void SearchResultSelectionChanged(SelectionChangedEventArgs e)//
        {
            if (ActiveItem == null)
                NewTab();
            ((ContentTabItemViewModel)ActiveItem).Address = ((ItemNode)((ListView)e.Source).SelectedItem).Name;
            //contentItem.browser.Url = new Uri(((System.Windows.Forms.TocNode)searchResult.SelectedItem).Name);
            //contentItem.Header = ((System.Windows.Forms.TocNode)searchResult.SelectedItem).Text;
        }
    }
}
