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
using System.Windows.Forms;

namespace EpubHelpViewer
{
    /// <summary>
    /// ContentItem.xaml 的交互逻辑
    /// </summary>
    public partial class ContentItem : TabItem
    {
        /// <summary>
        /// 如果是顺序阅读就用DocumentTitle
        /// </summary>
        public bool UseDocumentTitle = false;
        public ContentItem()
        {
            InitializeComponent();
            browser.Url = new Uri("about:blank");
            this.Header = "about:blank";
            if (!UseDocumentTitle)
                this.browser.DocumentTitleChanged -= browser_DocumentTitleChanged;
        }
        public void print()
        {
            browser.Document.ExecCommand("Print", false, null);
            //var printDlg = new System.Windows.Forms.PrintDialog();
            //System.Drawing.Printing.PrintDocument docToPrint = new System.Drawing.Printing.PrintDocument();
            //printDlg.AllowSomePages = true;
            //printDlg.Document = docToPrint;
            //if (printDlg.ShowDialog() == DialogResult.OK)
            //{
            //    docToPrint.Print();
            //}
        }
        private void browser_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.Header = browser.DocumentTitle;
        }
    }
}
