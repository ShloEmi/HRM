using Prism.Ioc;
using Prism.Modularity;

namespace Norav.HRM.Client.WPF.Modules.EcgSimulation
{
    public class EcgSimulationModule : IModule
    {
        /* TODO: Shlomi, put in self assembly! */
        /* TODO: Shlomi, load from config file, to be able to switch to diff provider */
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IEcgProvider, EcgSimulationProvider>();
        }
    }
}