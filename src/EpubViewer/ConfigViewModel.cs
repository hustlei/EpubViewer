using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;

namespace EpubViewer
{
    internal class ConfigViewModel : Screen
    {
        private bool _epubChecked;
        private bool _epub3Checked;
        public bool EpubChecked
        {
            get { return _epubChecked; }
            set { _epubChecked = value; NotifyOfPropertyChange(() => EpubChecked); }
        }
        public bool Epub3Checked
        {
            get { return _epub3Checked; }
            set { _epub3Checked = value; NotifyOfPropertyChange(() => Epub3Checked); }
        }
        internal ConfigViewModel()
        {
            string progFilename = Environment.CurrentDirectory + @"\EpubViewer.exe";
            EpubChecked = Assoc.IsAssoced(progFilename, "epub");
            Epub3Checked = Assoc.IsAssoced(progFilename, "epub3");
        }
        public void Close()
        {
            ((Window)GetView()).Close();
        }

        // ReSharper disable once InconsistentNaming
        public void OK()
        {
            //string progFilename = Directory.GetCurrentDirectory() + @"\EpubViwer.exe";

            string progFilename = Environment.CurrentDirectory + @"\EpubViewer.exe";
            string type = "epub";
            string typeDescription = "epub电子书";
            string mimeType = "application/epub+zip";
            string ico = Environment.CurrentDirectory + @"\rc4net.dll,1";

            if (EpubChecked)
                Assoc.AssocType(progFilename, type, typeDescription, mimeType, ico);
            else
            {
                Assoc.UnAssocType(type);
            }
            type = "epub3";
            typeDescription = "epub3电子书";
            if (Epub3Checked)
                Assoc.AssocType(progFilename, type, typeDescription, mimeType, ico);
            else
            {
                Assoc.UnAssocType(type);
            }
            Assoc.Refresh();
            Close();
        }
    }
}
