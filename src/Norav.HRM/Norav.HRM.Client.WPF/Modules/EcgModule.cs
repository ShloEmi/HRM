using Prism.Ioc;
using Prism.Modularity;

namespace Norav.HRM.Client.WPF.Modules
{
    public class EcgModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IECGAdapter, ECGAdapterSimulator>();
        }
    }

    public interface IECGAdapter
    {
    }
    
    public class ECGAdapterSimulator : IECGAdapter
    {
        public ECGAdapterSimulator()
        {
        }
    }

}