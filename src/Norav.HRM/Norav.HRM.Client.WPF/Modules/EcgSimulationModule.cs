using Norav.HRM.Client.WPF.Interfaces;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace Norav.HRM.Client.WPF.Modules
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

    public interface IEcgProvider
    {
        void Start();
        void Stop();
    }
    
    public class EcgSimulationProvider : IEcgProvider
    {
        private readonly CompositeDisposable eventSubscription = new();


        public EcgSimulationProvider(IApplicationEvents applicationEvents)
        {
            eventSubscription.Add(applicationEvents.Initializing.Subscribe(OnInitialized));
            eventSubscription.Add(applicationEvents.Exiting.Subscribe(OnExiting));
        }

        private void OnInitialized(Unit _)
        {
        }

        private void OnExiting(Unit _)
        {
            eventSubscription.Clear();
        }

        void IEcgProvider.Start()
        {
        }

        void IEcgProvider.Stop()
        {
        }
    }
}