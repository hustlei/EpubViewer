using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace EpubViewer
{
    internal class Assoc
    {
        private Assoc()
        {
        }

        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public static void Refresh()
        {
            SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
        }
        public static bool IsAssoced(string progFile, string type)
        {
            string extName = "." + type;
            var regKey = Registry.ClassesRoot.OpenSubKey(extName);
            if (regKey == null)
                return false;
            string progId = regKey.GetValue("").ToString();
            string prog = Path.GetFileNameWithoutExtension(progFile);
            if (prog != null && progId.Contains(prog))
                return true;
            return false;
        }
        /// <summary>
        /// 关联文件格式到程序
        /// </summary>
        /// <param name="progFile">关联程序的完整路径和文件名</param>
        /// <param name="type">关联文件扩展名</param>
        /// <param name="typeDescription">关联的文件类型说明</param>
        /// <param name="mimeType">关联的文件的mimetype，可以为null</param>
        /// <param name="ico">文件格式图标的完整路径</param>
        public static void AssocType(string progFile, string type, string typeDescription, string mimeType = null,
            string ico = null)
        {
            try
            {
                string extName = "." + type;
                string progId = Path.GetFileNameWithoutExtension(progFile) + "." + type;
                Registry.ClassesRoot.CreateSubKey(extName).SetValue("", progId);
                if (!string.IsNullOrEmpty(progId))
                    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progId))
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


        public static void UnAssocType(string type)
        {
            try
            {
                string extName = "." + type;
                var regKey = Registry.ClassesRoot.OpenSubKey(extName);
                if (regKey != null)
                {
                    string progId = regKey.GetValue("").ToString();
                    regKey.Close();
                    Registry.ClassesRoot.DeleteSubKeyTree(extName);
                    if (!string.IsNullOrEmpty(progId))
                        Registry.ClassesRoot.DeleteSubKeyTree(progId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
