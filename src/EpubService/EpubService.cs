using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lei.Common;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace EpubViewer
{
    public class EpubService
    {
        private readonly List<EpubBook> _epubList;

        //public readonly ObservableCollection<ItemNode> Nodes;
        public List<EpubBook> EpubList { get { return _epubList; } }

        public EpubService()
        {
            _epubList = new List<EpubBook>();
        }

        //public void OpenFiles(string[] files)
        //{
        //    foreach (string file in files)
        //    {
        //        if (_epubList.Count == 0 || _epubList.Any(a => a.Filename != file))
        //        {
        //            _epubList.Add(new EpubBook());
        //            if (!_epubList[_epubList.Count - 1].Open(file))
        //            {
        //                _epubList[_epubList.Count - 1].Close();
        //                _epubList.RemoveAt(_epubList.Count - 1);
        //                return;
        //            }
        //        }
        //    }
        //}
        public Task<bool> OpenFilesAsync(string[] files)
        {
            foreach (string file in files)
            {
                if (_epubList.Count == 0 || _epubList.Any(a => a.Filename != file))
                {
                    _epubList.Add(new EpubBook());
                    var t = _epubList[_epubList.Count - 1].OpenAsync(file);
                    return t.ContinueWith(t1 =>
                    {
                        if (t1.Result)
                            return true;
                        _epubList[_epubList.Count - 1].Close();
                        _epubList.RemoveAt(_epubList.Count - 1);
                        return false;
                    });
                }
                return Task<bool>.Factory.StartNew(() => false);
            }
            return Task<bool>.Factory.StartNew(() => false);
        }

        public void CloseFiles()
        {
            foreach (var epub in _epubList)
            {
                epub.Close();
            }
        }

        ~EpubService()
        {
            CloseFiles();
        }
    }
}
