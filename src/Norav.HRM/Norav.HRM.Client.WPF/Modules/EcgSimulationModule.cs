using Norav.HRM.Client.WPF.Interfaces;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;

namespace Norav.HRM.Client.WPF.Modules
{
    public class EcgSimulationProvider : IEcgProvider
    {
        private readonly CompositeDisposable eventSubscription = new();

        private readonly IPlotPresenter plotPresenter;
        private readonly IFileSystem fileSystem;
        private double[] dataY;


        public EcgSimulationProvider(IApplicationEvents applicationEvents, IPlotPresenter plotPresenter, IFileSystem fileSystem)
        {
            this.plotPresenter = plotPresenter;
            this.fileSystem = fileSystem;

            eventSubscription.Add(applicationEvents.Initializing.Subscribe(OnInitialized));
            eventSubscription.Add(applicationEvents.Starting.Subscribe(OnStarting));
            eventSubscription.Add(applicationEvents.Exiting.Subscribe(OnExiting));
        }

        private void OnInitialized(Unit _)
        {
        }

        private void OnStarting(Unit _)
        {
            string[] readAllLines = fileSystem.File.ReadAllLines(@"ExampleData\ECG.data.csv");
            dataY = readAllLines.Select(double.Parse).ToArray();

            plotPresenter.Plot?.Title("Heartbeat graph");
        }

        private void OnExiting(Unit _)
        {
            eventSubscription.Clear();
        }


        void IEcgProvider.Start()
        {
            plotPresenter.Plot.AddSignal(dataY);
            plotPresenter.Refresh();
        }

        void IEcgProvider.Stop()
        {
        }
    }

    public interface IEcgProvider
    {
        void Start();
        void Stop();
    }

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