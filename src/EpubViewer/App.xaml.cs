//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Linq;
//using System.Windows;

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace EpubViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            conf(@"profiles\init.conf");
            AppDomain.CurrentDomain.UnhandledException += 
            	(sender, args) =>MessageBox.Show(((Exception)args.ExceptionObject).Message, "Exception Unhandled");
        }
        public App()
        {
            try
            {
                var booter = new AppBootstrapper();
            }
            catch (Exception e)
            {
                MessageBox.Show("BootStrapper: " + e.Message);
            }
        }

        // ReSharper disable once InconsistentNaming
        private static void conf(string p)
        {
            //AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", p);
            var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
            var fusionContext = funsion.Invoke(AppDomain.CurrentDomain, null);
            var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
            m.Invoke(null, new[] { fusionContext, "APP_CONFIG_FILE", p });
        }
    }
}
