﻿//using System;
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
        }
        public App()
        {
            try
            {
                var booter = new AppBootstrapper();
                booter.Initialize();
                this.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            }
            catch (Exception e)
            {
                MessageBox.Show("BootStrapper: " + e.Message);
            }
        }

        // ReSharper disable once InconsistentNaming
        private static void conf(string p)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", p);
            var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
            var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
            m.Invoke(null, new[] { funsion.Invoke(AppDomain.CurrentDomain, null), "APP_CONFIG_FILE", p });
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "app.xaml unhandled");
        }
    }
}
