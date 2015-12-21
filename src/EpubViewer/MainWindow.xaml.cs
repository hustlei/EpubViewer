using System;
using System.Collections.Generic;
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
using Lei.UI;
using Lei.Common;
using mshtml;
using winform = System.Windows.Forms;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EpubViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MordenWindow
    {
        private List<EpubBook> epubList;
        private ContentItem contentItem;
        private winform.OpenFileDialog openDlg;
        public MainWindow()
        {
            InitializeComponent();
            treeView.ImageList = EpubBook.ImageList;
            epubList = new List<EpubBook>();

            openDlg = new winform.OpenFileDialog();
            openDlg.Filter = "EPUB Files(*.epub)|*.epub";
            openDlg.Multiselect = true;
            openDlg.RestoreDirectory = true;

        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (openDlg.ShowDialog() == winform.DialogResult.OK)
            {
                if (openDlg.FileNames.Length > 0 && openDlg.FileNames[0].Length > 0)
                {
                    OpenFiles(openDlg.FileNames);
                }
            }
        }
        public void OpenFiles(string[] files)
        {

            waiting.Visibility = Visibility.Visible;
            Task.Factory.StartNew(() =>
            {
                foreach (string file in files)
                {
                    if (epubList.Count == 0 || epubList.Any(a => a.Filename != file))
                    {

                        epubList.Add(new EpubBook());
                        if (!epubList[epubList.Count - 1].Open(file))
                        {
                            epubList[epubList.Count-1].Close();
                            epubList.RemoveAt(epubList.Count - 1);
                            this.Dispatcher.Invoke((Action)delegate()
                            {
                                waiting.Visibility = Visibility.Collapsed;
                            });
                            return;
                        }

                        this.Dispatcher.Invoke((Action)delegate()
                        {
                            treeView.Nodes.Add(epubList[epubList.Count - 1].TreeNode);
                        });
                    }
                }
                this.Dispatcher.Invoke((Action)delegate()
                {
                    if (epubList[0].IsSpine)
                        contentItem.UseDocumentTitle = true;
                    treeView.SelectedNode = treeView.Nodes[0];
                    treeView.SelectedNode.Expand(); 
                    waiting.Visibility = Visibility.Collapsed;
                });
            });
            /*
            waiting.Visibility = Visibility.Visible;
            Task ab= Task.Factory.StartNew(() =>
            {
                foreach (string file in files)
                {
                    if (epubList.Count == 0 || epubList.Any(a => a.Filename != file))
                    {
                        epubList.Add(new EpubBook());
                        epubList[epubList.Count - 1].Open(file);
                    }
                }
            });
            ab.ContinueWith(
                t =>
                {
                    foreach (EpubBook f in epubList)
                    {
                        treeView.Nodes.Add(f.TreeNode);
                    }
                    if (epubList[0].IsSpine)
                        contentItem.UseDocumentTitle = true;
                    treeView.SelectedNode = treeView.Nodes[0];
                    treeView.SelectedNode.Expand();
                    waiting.Visibility = Visibility.Collapsed;
                },TaskScheduler.FromCurrentSynchronizationContext()
                );
            */
            /*
            //BackgroundWorker实现异步执行,并更改界面
            waiting.Visibility = Visibility.Visible;
            private BackgroundWorker backWorker;
            backWorker = new BackgroundWorker();
            backWorker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                foreach (string file in files)
                {
                    if (epubList.Count==0 || epubList.Any(a => a.Filename != file))
                    {
                        epubList.Add(new EpubBook());
                        epubList[epubList.Count - 1].Open(file);
                    }
                }
            };
            backWorker.RunWorkerCompleted += (object seder, RunWorkerCompletedEventArgs e) =>
            {
                foreach (EpubBook f in epubList)
                {
                    treeView.Nodes.Add(f.TreeNode);
                }
                if (epubList[0].IsSpine)
                    contentItem.UseDocumentTitle = true;
                treeView.SelectedNode = treeView.Nodes[0];
                treeView.SelectedNode.Expand();
                waiting.Visibility = Visibility.Collapsed;
            };
            backWorker.RunWorkerAsync();
            */


            //waiting.Visibility = Visibility.Visible;
            ////模拟winform Doevent，效率低，建议改多线程
            //DispatcherFrame frame = new DispatcherFrame(true);
            //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (System.Threading.SendOrPostCallback)delegate(object arg)
            //{
            //    DispatcherFrame fr = arg as DispatcherFrame;
            //    fr.Continue = false;
            //}, frame);
            //Dispatcher.PushFrame(frame);
        }

        #region 左边栏treeview相关函数
        private void collapse(object sender, winform.TreeViewCancelEventArgs e)
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
        }
        private void expand(object sender, winform.TreeViewCancelEventArgs e)
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }
        private void afterSelect(object sender, winform.TreeViewEventArgs e)
        {
            if (contentItem == null)
                NewTab_Click(null, null);
            try
            {
                string url = "";
                string text = "";
                if (e != null)
                {
                    url = e.Node.Name;
                    text = e.Node.Text;
                }
                else
                {
                    if (treeView.SelectedNode != null)
                    {
                        url = treeView.SelectedNode.Name;
                        text = treeView.SelectedNode.Text;
                    }
                }
                if (url != "")
                {
                    //contentItem.browser.Url = new Uri(Uri.EscapeUriString(url));
                    //暂时不知道为什么不支持中文锚点
                    Uri xx = new Uri(url);
                    contentItem.browser.Navigate(xx);
                    //    MessageBox.Show(xx + " " + contentItem.browser.Url.ToString());
                    contentItem.Header = text;
                }
            }
            catch { }
        }
        # endregion

        #region 内容主题tabcontrol相关函数
        private void contentItemChanged(object sender, SelectionChangedEventArgs e)
        {
            contentItem = (ContentItem)content.SelectedItem;
        }
        #endregion
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
            if (epubList.Count < 1)
                return;
            string url = contentItem.browser.Url.OriginalString;
            foreach (System.Windows.Forms.TreeNode t in treeView.Nodes)
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
        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            if (epubList.Count < 1)
                return;
            contentItem = new ContentItem();
            if (epubList[0].IsSpine)
                contentItem.UseDocumentTitle = true;
            content.Items.Add(contentItem);
            content.SelectedItem = contentItem;
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
        }
        private void Config_OnClick(object sender, RoutedEventArgs e)
        {
            ConfigWin w = new ConfigWin();
            w.Owner = this;
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.Topmost = true;
            w.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            foreach (EpubBook b in epubList)
                b.Close();
            epubList = new List<EpubBook>();
            treeView.Nodes.Clear();
            content.Items.Clear();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            foreach (EpubBook b in epubList)
                b.Close();
            Environment.Exit(0);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            foreach (EpubBook b in epubList)
                b.Close();
        }

        private void searchResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contentItem.browser.Url = new Uri(((System.Windows.Forms.TreeNode)searchResult.SelectedItem).Name);
            contentItem.Header = ((System.Windows.Forms.TreeNode) searchResult.SelectedItem).Text;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            string topic = searchText.Text.Trim();
            if (string.IsNullOrEmpty(topic))
            {
                return;
            }
            List<System.Windows.Forms.TreeNode> rst = new List<System.Windows.Forms.TreeNode>();
            foreach (System.Windows.Forms.TreeNode t in treeView.Nodes)
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
        }

        private void MainWindow_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            //string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            this.OpenFiles(new string[] { fileName });
        }

    }
}
