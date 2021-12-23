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
}
