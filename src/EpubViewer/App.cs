//本文件和App.xaml具有相同的效果，是App.xaml的纯代码实现，更加灵活，当然需要知识更多。
//默认用app.xaml
#define XAMLAPP
#if !XAMLAPP
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace EpubViewer
{
    public class App : Application
    {
        private AppBootstrapper b;
        public App():base()
        {
            try
            {
                //ResourceDictionary a=new ResourceDictionary();
                //b = new AppBootstrapper();
                //a.Add("",b);
                //this.Resources.MergedDictionaries.Add(a);
                var booter=new AppBootstrapper();
                booter.Initialize();
                this.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            }
            catch (Exception e)
            {
                MessageBox.Show("BootStrapper: "+e.Message);
            }
        }

        //[System.STAThreadAttribute()]
        [STAThread]
        static void Main()
        {
            conf(@"profiles\init.conf");
            try
            {
                var app = new App();
                app.Run();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Source.ToString() + ": " + e.Message, "in Main");
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
#endif