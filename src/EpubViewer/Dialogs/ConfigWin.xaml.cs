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
//using System.Windows.Shapes;
using Lei.UI;
using Lei.Common;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace EpubViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWin : MordenWindow
    {
        public ConfigWin()
        {
            InitializeComponent();
            string progFilename = Environment.CurrentDirectory + @"\EpubViewer.exe";
            if (isAssoced(progFilename, "epub"))
                CheckBoxEpub.IsChecked = true;
            else
                CheckBoxEpub.IsChecked = false;
            if (isAssoced(progFilename, "epub3"))
                CheckBoxEpub3.IsChecked = true;
            else
                CheckBoxEpub3.IsChecked = false;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            //string progFilename = Directory.GetCurrentDirectory() + @"\EpubViwer.exe";

            string progFilename = Environment.CurrentDirectory + @"\EpubViewer.exe";
            string type = "epub";
            string typeDescription = "epub电子书";
            string mimeType = "application/epub+zip";
            string ico = Environment.CurrentDirectory + @"\rc4net.dll,1";
            if (CheckBoxEpub.IsChecked != null && CheckBoxEpub3.IsChecked != null)
            {
                if (CheckBoxEpub.IsChecked == true)
                    assocFile(progFilename, type, typeDescription, mimeType, ico);
                else
                {
                    unassocFile(type);
                }
                type = "epub3";
                typeDescription = "epub3电子书";
                if (CheckBoxEpub3.IsChecked == true)
                    assocFile(progFilename, type, typeDescription, mimeType, ico);
                else
                {
                    unassocFile(type);
                }
                SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
            }
            this.Close();
        }

        /// <summary>
        /// 关联文件格式到程序
        /// </summary>
        /// <param name="progFile">关联程序的完整路径和文件名</param>
        /// <param name="type">关联文件扩展名</param>
        /// <param name="typeDescription">关联的文件类型说明</param>
        /// <param name="mimeType">关联的文件的mimetype，可以为null</param>
        /// <param name="ico">文件格式图标的完整路径</param>
        private void assocFile(string progFile, string type, string typeDescription, string mimeType = null,
            string ico = null)
        {
            try
            {
                string extName = "." + type;
                string progID = Path.GetFileNameWithoutExtension(progFile) + "." + type;
                Registry.ClassesRoot.CreateSubKey(extName).SetValue("", progID);
                if (!string.IsNullOrEmpty(progID))
                    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID))
                    {
                        if (typeDescription != null)
                            key.SetValue("", typeDescription);
                        if (ico != null)
                            key.CreateSubKey("DefaultIcon").SetValue("", ico);
                        if (progFile != null)
                            key.CreateSubKey(@"Shell\Open\Command").SetValue("", progFile + " %1");
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private bool isAssoced(string progFile, string type)
        {
            RegistryKey regKey;
            string extName = "." + type;
            regKey = Registry.ClassesRoot.OpenSubKey(extName);
            if (regKey == null)
                return false;
            else
            {
                string progID = regKey.GetValue("").ToString();
                string prog = Path.GetFileNameWithoutExtension(progFile);
                if (progID.Contains(prog))
                    return true;
                else
                {
                    return false;
                }
            }
        }
        private void unassocFile(string type)
        {
            try
            {
                RegistryKey regKey;
                string extName = "." + type;
                regKey = Registry.ClassesRoot.OpenSubKey(extName);
                if (regKey != null)
                {
                    string progID = regKey.GetValue("").ToString();
                    regKey.Close();
                    Registry.ClassesRoot.DeleteSubKeyTree(extName);
                    if (!string.IsNullOrEmpty(progID))
                        Registry.ClassesRoot.DeleteSubKeyTree(progID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}