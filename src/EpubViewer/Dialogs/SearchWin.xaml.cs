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

namespace EpubHelpViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchWin : MordenWindow
    {
        public System.Windows.Forms.WebBrowser browser;
        public SearchWin()
        {
            InitializeComponent();           
        }
        public SearchWin(System.Windows.Forms.WebBrowser b):this()
        {
            browser = b;
        }
        public void SearchText(System.Windows.Forms.WebBrowser browser, string keyword)
        {
            keyword = keyword.Trim();
            if (keyword == ""|| browser==null)
                return;

            // Document的DomDocument属性，就是该对象内部的COM对象。
            //HtmlElement等类是对原有mshtml这个COM组件的不完整封装，只提供了mshtml的部分功能。
            //所以许多时候，我们仍旧要借助mshtml来实现需要的功能。
            IHTMLDocument2 document = (IHTMLDocument2)browser.Document.DomDocument;
            IHTMLTxtRange searchRange = null;

            // IE的查找逻辑就是，如果有选区，就从当前选区开头+1字符处开始查找；没有的话就从页面最初开始查找。 
            // 这个逻辑其实是有点不大恰当的，我们这里不用管，和IE一致即可。 
            if (document.selection.type.ToLower() != "none")
            {
                searchRange = (IHTMLTxtRange)document.selection.createRange();
                searchRange.collapse(true);
                searchRange.moveStart("character", 1);
            }
            else
            {
                IHTMLBodyElement body = (IHTMLBodyElement)document.body;
                searchRange = (IHTMLTxtRange)body.createTextRange();
            }

            // 如果找到了，就选取（高亮显示）该关键字；否则弹出消息。 
            if (searchRange.findText(keyword, 1, 0))
            {
                searchRange.select();
            }
            else
            {
                System.Windows.MessageBox.Show("已搜索到文档结尾。");
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchText(browser, this.text.Text);
        }

    }
}
