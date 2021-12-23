using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace Norav.HRM.Client.WPF
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            new Bootstrapper().Run();
        }
    }

    internal class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IECGAdapter, ECGAdapterSimulator>();
        }
    }
}
