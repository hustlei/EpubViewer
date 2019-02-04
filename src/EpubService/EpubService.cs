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

        public List<EpubBook> EpubList { get { return _epubList; } }

        public EpubService()
        {
            _epubList = new List<EpubBook>();
        }

        /// <summary>
        /// 打开epub文件
        /// </summary>
        /// <param name="file">文件名，完整路径</param>
        /// <returns>返回新打开的文件在EpubList中的序号，null表示打开失败</returns>
        public int? OpenFile(string file)
        {
            if (_epubList.Count > 0 && _epubList.Any(epub => epub.Filename == file))
                return null;
            _epubList.Add(new EpubBook());
            int index = _epubList.Count - 1;
            if (!_epubList[index].Open(file))
            {
                _epubList[index].Close();
                _epubList.RemoveAt(index);
                return null;
            }
            return index;
        }

        /// <summary>
        /// 异步打开epub文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns>返回一个Task对象，Task.Result为新打开的文件在EpubList中的序号，Task.Result为null表示打开失败</returns>
        public Task<int?> OpenFileAsync(string file)
        {
            if (_epubList.Count > 0 && _epubList.Any(epub => epub.Filename == file))
                return Task<int?>.Factory.StartNew(() => null);

            _epubList.Add(new EpubBook());//这一句必须在UI线程上执行
            int index = _epubList.Count - 1;

            var t = _epubList[index].OpenAsync(file);
            return t.ContinueWith<int?>(t1 =>
            {
                if (t1.Result)
                    return index;

                _epubList[index].Close();
                _epubList.RemoveAt(index);
                return null;
            });
            //var t = _epubList[index].OpenAsync(file);
            //return t.ContinueWith(t1 =>
            //{
            //    if (t1.Result)
            //        return true;

            //    _epubList[index].Close();
            //    _epubList.RemoveAt(index);
            //    return false;
            //});
        }

        public void CloseFiles()
        {
            foreach (var epub in _epubList)
            {
                epub.Close();
            }
            _epubList.Clear();
        }

        public void CloseFile(String fileName)
        {
            foreach (var epub in EpubList)
            {
                if ((string) epub.TocNode.Tag != fileName) continue;
                EpubList.Remove(epub);
                epub.Close();
                break;
            }
        }

        ~EpubService()
        {
            CloseFiles();
        }
    }
}
