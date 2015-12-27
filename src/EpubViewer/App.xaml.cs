//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Linq;
//using System.Windows;

using System;
using System.Reflection;

namespace EpubViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public App()
        {
            //string s = AppDomain.CurrentDomain.BaseDirectory;
            //Environment.SetEnvironmentVariable("path", s + "occt;" + s + "occt\\3rdparty;" + Environment.GetEnvironmentVariable("path"));
            conf("profiles\\init.conf");
        }

        private void conf(string p)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", p);
            var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
            var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
            m.Invoke(null, new object[] { funsion.Invoke(AppDomain.CurrentDomain, null), "APP_CONFIG_FILE", p });
        }
    }
}
