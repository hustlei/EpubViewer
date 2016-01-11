using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
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

namespace EpubViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [Export(typeof (MainViewModel))]
    public class MainViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly IWindowManager _windowManager;
        private readonly EpubService _epubService;
        private readonly winform.OpenFileDialog _openDlg;

        private List<winform.TreeNode> _nod; 
        public List<winform.TreeNode> Nod {
            get { return _nod; }
            set { _nod = value;NotifyOfPropertyChange("Nod"); }
        }

        private Visibility _waitingVisible;
        public Visibility WaitingVisible {
            get { return _waitingVisible; }
            set { _waitingVisible=value;NotifyOfPropertyChange("WaitingVisible"); }
        }
        public bool AllowDrop { get; set; }
        [ImportingConstructor]
        public MainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            _epubService= new EpubService();
            _openDlg = new winform.OpenFileDialog
            {
                Filter = "EPUB Files(*.epub)|*.epub|EPUB Files(*.epub3)|*.epub3",
                Multiselect = true,
                RestoreDirectory = true
            };
            WaitingVisible=Visibility.Collapsed;
            AllowDrop = true;
        }
        public void Open()
        {
            if (_openDlg.ShowDialog() == winform.DialogResult.OK)
            {
                WaitingVisible = Visibility.Visible;
                var task = Task.Factory.StartNew(() =>
                {
                    if (_openDlg.FileNames.Length > 0 && _openDlg.FileNames[0].Length > 0)
                    {
                        _epubService.OpenFiles(_openDlg.FileNames);
                        Nod=_epubService.TreeNode;
                        if(ActiveItem==null)
                            NewTab();
                        if (_epubService.EpubList.Count > 0 && _epubService.EpubList[0].IsSpine)
                            ((ContentTabItemViewModel)ActiveItem).UseDocumentTitle = true;
                    }
                    WaitingVisible = Visibility.Collapsed;//ok可行
                });

                //        //treeView.SelectedNode = treeView.Nod[0];
                //        //treeView.SelectedNode.Expand();
            }
        }

        public void Close()
        {
            _epubService.CloseFiles();
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

        /* 多线程

        //#region 左边栏treeview相关函数
        //private void collapse(object sender, winform.TreeViewCancelEventArgs e)
        //{
        //    e.Node.ImageIndex = 0;
        //    e.Node.SelectedImageIndex = 0;
        //}
        //private void expand(object sender, winform.TreeViewCancelEventArgs e)
        //{
        //    e.Node.ImageIndex = 1;
        //    e.Node.SelectedImageIndex = 1;
        //}
        //private void afterSelect(object sender, winform.TreeViewEventArgs e)
        //{
        //    if (contentItem == null)
        //        NewTab_Click(null, null);
        //    try
        //    {
        //        string url = "";
        //        string text = "";
        //        if (e != null)
        //        {
        //            url = e.Node.Name;
        //            text = e.Node.Text;
        //        }
        //        else
        //        {
        //            if (treeView.SelectedNode != null)
        //            {
        //                url = treeView.SelectedNode.Name;
        //                text = treeView.SelectedNode.Text;
        //            }
        //        }
        //        if (url != "")
        //        {
        //            //contentItem.browser.Url = new Uri(Uri.EscapeUriString(url));
        //            //暂时不知道为什么不支持中文锚点
        //            Uri xx = new Uri(url);
        //            contentItem.browser.Navigate(xx);
        //            //    MessageBox.Show(xx + " " + contentItem.browser.Url.ToString());
        //            contentItem.Header = text;
        //        }
        //    }
        //    catch { }
        //}
        //# endregion
         */

        #region 内容主题tabcontrol相关函数
        //private void ContentTabChanged(object sender)
        //{
        //    contentItem = ((TabControl)sender).SelectedContent;
        //}
        #endregion

        public void NewTab()
        {
            var item = new ContentTabItemViewModel();
            ActivateItem(item);
            //item.DisplayName = item.WebBrowser.Title;

            //if (_epubList.Count < 1)
            //    return;
            //contentItem = new ContentItem();
            //if (_epubList[0].IsSpine)
            //    contentItem.UseDocumentTitle = true;
            //content.Items.Add(contentItem);
            //content.SelectedItem = contentItem;
        }

        public void CloseTab(RoutedEventArgs e)
        {
            this.CloseItem(((FrameworkElement)e.OriginalSource).DataContext);
        }
        /*
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (contentItem != null && contentItem.browser.CanGoBack)
                contentItem.browser.GoBack();
        }
        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (contentItem != null && contentItem.browser.CanGoForward)
                contentItem.browser.GoForward();
        }
        private void Sync_Click(object sender, RoutedEventArgs e)
        {
            if (_epubList.Count < 1)
                return;
            string url = contentItem.browser.Url.OriginalString;
            foreach (System.Windows.Forms.TreeNode t in treeView.Nod)
            {
                System.Windows.Forms.TreeNode node = EpubBook.SearchNodeName(t, url);
                if (node != null)
                {
                    treeView.SelectedNode = node;
                    return;
                }
            }
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (contentItem != null)
                contentItem.browser.Document.ExecCommand("Print", false, null);
        }
        private void Encoding_Click(object sender, RoutedEventArgs e)
        {
            //item.Document.Body.Style += "fontzise:120%;";
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                contentItem.browser.Focus();
                if (contentItem.browser.Document != null)
                {
                    contentItem.browser.Document.Focus();
                    if (contentItem.browser.Document.Body != null)
                    {
                        contentItem.browser.Document.Body.Focus();
                    }
                }
            }
            catch { }
            System.Windows.Forms.SendKeys.SendWait("^=");
        }
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                contentItem.browser.Focus();
                if (contentItem.browser.Document != null)
                {
                    contentItem.browser.Document.Focus();
                    if (contentItem.browser.Document.Body != null)
                    {
                        contentItem.browser.Document.Body.Focus();
                    }
                }
            }
            catch { }
            System.Windows.Forms.SendKeys.SendWait("^-");
        }
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            if (contentItem != null)
            {
                SearchWin w = new SearchWin(contentItem.browser);
                w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                w.ShowDialog();
            }
        }
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            about dlg = new about();
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Topmost = true;
            dlg.ShowDialog();
        }*/
        public void Config()
        {
            var settings = new Dictionary<string, object> {{"Topmost", true}, {"Owner", GetView()}};//Owner不是依赖属性不能绑定,只能在这里设置
            _windowManager.ShowDialog(new ConfigViewModel(), null, settings);
        }
        /*
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            foreach (EpubBook b in _epubList)
                b.Close();
            _epubList = new List<EpubBook>();
            treeView.Nod.Clear();
            content.Items.Clear();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            foreach (EpubBook b in _epubList)
                b.Close();
            Environment.Exit(0);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            foreach (EpubBook b in _epubList)
                b.Close();
        }

        private void searchResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contentItem.browser.Url = new Uri(((System.Windows.Forms.TreeNode)searchResult.SelectedItem).Name);
            contentItem.Header = ((System.Windows.Forms.TreeNode)searchResult.SelectedItem).Text;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string topic = searchText.Text.Trim();
            if (string.IsNullOrEmpty(topic))
            {
                return;
            }
            List<System.Windows.Forms.TreeNode> rst = new List<System.Windows.Forms.TreeNode>();
            foreach (System.Windows.Forms.TreeNode t in treeView.Nod)
            {
                EpubBook.SearchTopic(t, topic, rst);
            }
            if (rst.Count == 0)
                rst.Add(new System.Windows.Forms.TreeNode("没有搜索到内容"));
            searchResult.ItemsSource = rst;
        }

        private void searchText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                searchButton_Click(null, null);
        }*/
    }
}
