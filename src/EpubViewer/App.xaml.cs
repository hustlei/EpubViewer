//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Linq;
//using System.Windows;

using System;
using System.Reflection;
using System.Windows;

namespace EpubViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App: Application
    {

        public App()
        {
            conf("profiles\\init.conf");
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        private static void conf(string p)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", p);
            var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
            var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
            m.Invoke(null, new[] { funsion.Invoke(AppDomain.CurrentDomain, null), "APP_CONFIG_FILE", p });
        }
    }
}
