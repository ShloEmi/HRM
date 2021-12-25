using System.Windows;

namespace Norav.HRM.Client.WPF
{
    public partial class App
    {
        private Bootstrapper bootstrapper;

        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            bootstrapper = new Bootstrapper();
            bootstrapper.Run();
            bootstrapper.OnStart();
        }

        protected override void OnExit(ExitEventArgs args)
        {
            bootstrapper.OnExit();

            base.OnExit(args);
        }
    }
}
