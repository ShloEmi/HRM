using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;

namespace Norav.HRM.Client.WPF
{
    internal class Bootstrapper : PrismBootstrapper
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IECGAdapter, ECGAdapterSimulator>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<EcgModule>();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
    }
}