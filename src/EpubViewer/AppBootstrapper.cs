using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
//using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace EpubViewer
{
    /// <summary>
    /// Booter
    /// </summary>
    public class AppBootstrapper : BootstrapperBase
    {
        private readonly CompositionContainer _container;
        public AppBootstrapper()
        {
            //必须添加这一句
            Initialize();
            AggregateCatalog catalog = new AggregateCatalog(
               AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
            //AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());
            _container = new CompositionContainer(catalog);
            //Import部件可以在catalog里加入
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            //batch.AddPart只能添加Import部件，Export部件好像添加无效
            _container.Compose(batch);

        }
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            List<Assembly> lst = new List<Assembly>();
            lst.AddRange(base.SelectAssemblies());
            lst.AddRange(
                Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\views")
                    .Where(file => file.EndsWith("dll", true, CultureInfo.CurrentCulture) || file.EndsWith("exe", true, CultureInfo.CurrentCulture))
                    .Select(Assembly.LoadFrom));
            //lst.AddRange(from file in Directory.GetFiles(Environment.CurrentDirectory + @"\views") where file.EndsWith("dll") || file.EndsWith("exe") select Assembly.LoadFrom(file));
            //lst.Add(Assembly.LoadFrom(System.Environment.CurrentDirectory+@"\view\Presenter.dll"));
            return lst;
        }
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            //MainWindow win = new MainWindow();
            //if (e.Args.Length > 0)
            //{
            //    string[] files = e.Args;
            //    win.OpenFiles(files);
            //}
            //win.Show();
            DisplayRootViewFor<IShell>();
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
    }
}
