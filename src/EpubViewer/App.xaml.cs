using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace EpubHelpViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow win = new MainWindow();
            if (e.Args.Length > 0)
            {
                string[] files = e.Args;
                win.OpenFiles(files);
            }
            win.Show();
        }
    }
}
