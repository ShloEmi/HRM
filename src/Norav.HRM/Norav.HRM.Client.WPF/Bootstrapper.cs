using Norav.HRM.Client.WPF.Interfaces;
using Norav.HRM.Client.WPF.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows;
using Norav.HRM.Client.WPF.Modules.HeartbeatSimulation;
using Norav.HRM.Client.WPF.ViewModels;

namespace Norav.HRM.Client.WPF
{
    public class Bootstrapper : PrismBootstrapper, IApplicationEvents
    {
        private readonly Subject<Unit> initializing;
        private readonly Subject<Unit> starting;
        private readonly Subject<Unit> exiting;


        public Bootstrapper()
        {
            Initializing = initializing = new Subject<Unit>();
            Starting = starting = new Subject<Unit>();
            Exiting = exiting = new Subject<Unit>();
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IContainerRegistry>(() => containerRegistry);
            containerRegistry.RegisterSingleton<IApplicationEvents>(() => this);

            containerRegistry.RegisterSingleton<IHeartbeatProvider, HeartbeatSimulationProvider>();
            containerRegistry.RegisterSingleton<IFileSystem, FileSystem>();

            containerRegistry.RegisterSingleton<MainWindowViewModel>(provider => new MainWindowViewModel(
                provider.Resolve<IHeartbeatProvider>(), provider.Resolve<IPlotPresenter>(), provider.Resolve<IFileSystem>(), 
                DispatcherScheduler.Current));

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog) => 
            moduleCatalog.AddModule<HeartbeatSimulationModule>();

        protected override DependencyObject CreateShell() => 
            Container.Resolve<MainWindow>();


        /// <summary> Notifies all registered interfaces of initialization phase. </summary>
        protected override void OnInitialized()
        {
            initializing.OnNext(Unit.Default);

            base.OnInitialized();
        }


        public void OnStart() => 
            starting.OnNext(Unit.Default);

        public void OnExit() => 
            exiting.OnNext(Unit.Default);


        public IObservable<Unit> Initializing { get; }

        public IObservable<Unit> Starting { get; }

        public IObservable<Unit> Exiting { get; }
    }
}