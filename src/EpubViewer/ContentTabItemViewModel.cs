using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CefSharp.Wpf;

namespace EpubViewer
{
    public class ContentTabItemViewModel : Screen
    {
        /// <summary>
        /// 如果是顺序阅读就用DocumentTitle
        /// </summary>
        public bool UseDocumentTitle = false;
        
        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyOfPropertyChange("Address");
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value;
                if (UseDocumentTitle && Title!=null) DisplayName = Title;
                NotifyOfPropertyChange("Title");
            }
        }

        private IWpfWebBrowser _webBrowser;
        public IWpfWebBrowser WebBrowser
        {
            get { return _webBrowser; }
            set { _webBrowser = value; NotifyOfPropertyChange("WebBrowser"); }
        }

        //private string _displyName;
        //public override string DisplayName { get; set; }
        //{
        //    get
        //    {
        //        if (UseDocumentTitle)
        //        {
        //            return Title;
        //        }
        //        return _displyName;
        //    }
        //    set { _displyName = value;NotifyOfPropertyChange("DisplayName"); }
        //}


        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ContentTabItemViewModel()
        {
            Address = "http://baidu.com";
            DisplayName = "about:blank";
            UseDocumentTitle = true;
        }

    }
}
