using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CefSharp;
using CefSharp.Wpf;

namespace EpubViewer
{
    public class FindViewModel:PropertyChangedBase
    {
        private string _text;
        private IWpfWebBrowser _browser;
        private bool _forward;
        private bool _matchcase;
        private bool _findnext;
        public FindViewModel(IWpfWebBrowser browser)
        {
            _browser = browser;
            _forward = true;
            _matchcase = false;
            _findnext = true;
            Text = "";
        }
        public string Text
        {
            set { _text = value;NotifyOfPropertyChange("Text"); }
            get { return _text; }
        }

        public void Find()
        {
            if (Text.Trim() != "" && _browser != null)
            {
                _browser.Find(1,Text,_forward,_matchcase,_findnext);
            }
        }
    }
}
