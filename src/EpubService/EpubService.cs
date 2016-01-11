using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lei.Common;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace EpubViewer
{
    public class EpubService
    {
        private readonly List<EpubBook> _epubList;

        public TreeNode TreeNode;
        public static List<BitmapImage> Images { get;set; }
        public List<EpubBook> EpubList { get { return _epubList; } }

        static EpubService()
        {
            Images = new List<BitmapImage>();
            foreach (System.Drawing.Image img in EpubBook.ImageList.Images)
            {
                Images.Add((BitmapImage)ImageConverter.ConvertFromWinform(img));
            }
        }
        public EpubService()
        {
            _epubList = new List<EpubBook>();
            TreeNode=new TreeNode();
        }

        public void OpenFiles(string[] files)
        {
            foreach (string file in files)
            {
                if (_epubList.Count == 0 || _epubList.Any(a => a.Filename != file))
                {
                    _epubList.Add(new EpubBook());
                    if (!_epubList[_epubList.Count - 1].Open(file))
                    {
                        _epubList[_epubList.Count - 1].Close();
                        _epubList.RemoveAt(_epubList.Count - 1);
                        return;
                    }
                    TreeNode.Nodes.Add(_epubList[_epubList.Count - 1].TreeNode);
                }
            }
        }

        public void CloseFiles()
        {
            foreach (var epub in _epubList)
            {
                epub.Close();
            }
            TreeNode.Nodes.Clear();
        }

        ~EpubService()
        {
            CloseFiles();
        }
    }
}
