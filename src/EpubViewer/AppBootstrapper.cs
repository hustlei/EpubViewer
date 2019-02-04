using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Contracts;
using EpubViewer.Properties;

//using System.ComponentModel.Composition.Primitives;

namespace EpubViewer
{
    /// <summary>
    /// Booter
    /// </summary>
    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer _container;
        public AppBootstrapper()
        {
            Initialize();//必须添加这一句
        }
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            List<Assembly> lst = new List<Assembly>();
            lst.AddRange(base.SelectAssemblies());
            lst.AddRange(
                Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + Settings.Default.viewsPath)
                    .Where(file => file.EndsWith("dll", true, CultureInfo.CurrentCulture) || file.EndsWith("exe", true, CultureInfo.CurrentCulture))
                    .Select(Assembly.LoadFrom));
            //lst.AddRange(from file in Directory.GetFiles(Environment.CurrentDirectory + @"\views") where file.EndsWith("dll") || file.EndsWith("exe") select Assembly.LoadFrom(file));
            lst.Add(Assembly.LoadFrom(App.basePath+@"\presenter\MainViewModel.dll"));
            return lst;
        }

        protected override void Configure()
        {
            _container = new CompositionContainer(new AggregateCatalog(
               AssemblySource.Instance.Select(x => new AssemblyCatalog(x))
               ));
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            _container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);
            var exportList = exports.ToList();//避免直接用exports时 调用2次IEnumerable操作
            if (exportList.Any())
                return exportList.First();
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //if (e.Args.Length > 0) //需要VM和Bootstrapper在一个程序集
            //    MainViewModel.Args = e.Args;//找不到更方便的方法。
            DisplayRootViewFor<IShell>();
            IoC.Get<IShell>().ProcessArgs(e.Args);
            //以下代码可以正常工作。不太适合传递/开关，虽然可以但不方便
            //if (e.Args.Length > 0)
            //    ((MainViewModel)IoC.GetInstance(typeof(MainViewModel), null)).OpenFiles(e.Args);
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
            MessageBox.Show(e.Exception.Message + "\n" + sender, "Exception Unhandled");
        }
    }
}
