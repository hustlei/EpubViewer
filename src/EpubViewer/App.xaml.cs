//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Linq;
//using System.Windows;

using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Threading;
using EpubViewer.Properties;

namespace EpubViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        internal static string basePath = "";
        static App()
        {
            basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            conf(Settings.Default.confPath);
            addPath(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Settings.Default.cefPath);
            //addPath(Environment.CurrentDirectory + "\\" + Settings.Default.cefPath);
            //AppDomain.CurrentDomain.UnhandledException +=
            //    (sender, args) =>
            //    {
            //        MessageBox.Show(((Exception) args.ExceptionObject).Message+"\n"+sender.ToString(), "Exception Unhandled");
            //        Environment.Exit(1);
            //    };
        }
        public App()
        {
            //xaml中没有资源也没有设置StartupUri，就不会有InitializeComponent函数
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
        /// <summary>
        /// 设置config文件自定义路径
        /// </summary>
        /// <param name="confPath">相对路径</param>
        private static void conf(string confPath)
        {
            if (!Path.IsPathRooted(confPath))
                confPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + confPath;
            //AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", confPath);
            var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
            var fusionContext = funsion.Invoke(AppDomain.CurrentDomain, null);
            var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
            m.Invoke(null, new[] { fusionContext, "APP_CONFIG_FILE", confPath });
        }

        /// <summary>
        /// 添加环境变量
        /// </summary>
        /// <param name="path">完整路径</param>
        private static void addPath(string path)
        {
            Environment.SetEnvironmentVariable("path", path+";" + Environment.GetEnvironmentVariable("path"));
        }
    }
}
