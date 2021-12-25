using Norav.HRM.Client.WPF.Interfaces;
using Prism.Ioc;
using Prism.Modularity;
using ScottPlot.Plottable;
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Threading;

namespace Norav.HRM.Client.WPF.Modules
{
    public class EcgSimulationProvider : IEcgProvider
    {
        private readonly CompositeDisposable eventSubscription = new();
        private readonly CompositeDisposable startTimerSubscription = new();

        private readonly IPlotPresenter plotPresenter;

        private readonly double[] dataBuffer = new double[1_000_000];
        int nextDataIndex = 1;
        SignalPlot signalPlot;
        readonly Random rand;



        public EcgSimulationProvider(IApplicationEvents applicationEvents, IPlotPresenter plotPresenter)
        {
            this.plotPresenter = plotPresenter;
            rand = new Random(0);

            eventSubscription.Add(applicationEvents.Initializing.Subscribe(OnInitialized));
            // eventSubscription.Add(applicationEvents.Starting.Subscribe(OnStarting));
            eventSubscription.Add(applicationEvents.Exiting.Subscribe(OnExiting));
        }

        private void OnInitialized(Unit _)
        {
            plotPresenter.Plot.YLabel("Value");
            plotPresenter.Plot.XLabel("Sample Number");

            plotPresenter.Plot?.Title("Heartbeat graph");

            signalPlot = plotPresenter.Plot.AddSignal(dataBuffer);
        }

        /*
        private void OnStarting(Unit _)
        {
        }
        */

        private void OnExiting(Unit _)
        {
            eventSubscription.Clear();
            startTimerSubscription.Dispose();
        }


        /// <seealso cref="https://github.com/ScottPlot/ScottPlot/blob/master/src/demo/ScottPlot.Demo.WPF/WpfDemos/LiveDataGrowing.xaml.cs"/>
        void IEcgProvider.Start()
        {
            startTimerSubscription.Add(Observable
                //.Interval(TimeSpan.FromMilliseconds(1d/1000 /*Hz*/))
                .Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(1d/10 /*Hz*/))
                .ObserveOn(new DispatcherScheduler(Dispatcher.CurrentDispatcher))
                .Subscribe(OnEcgSamples));

            startTimerSubscription.Add(Observable
                // .Interval(TimeSpan.FromMilliseconds(1d/50 /*Hz*/))
                .Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(1d/5 /*Hz*/))
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(OnRender));
        }

        private void OnRender(long elapsed)
        {
            plotPresenter.Plot.AxisAuto();
            plotPresenter.Refresh();
        }

        private void OnEcgSamples(long elapsed)
        {
            if (nextDataIndex >= dataBuffer.Length)
            {
                throw new OverflowException("dataBuffer isn't long enough to accomodate new data");
                // in this situation the solution would be:
                //   1. clear the plot
                //   2. create a new larger array
                //   3. copy the old data into the start of the larger array
                //   4. plot the new (larger) array
                //   5. continue to update the new array
            }

            double randomValue = Math.Round(rand.NextDouble() - .5, 3);
            double latestValue = dataBuffer[nextDataIndex - 1] + randomValue;
            dataBuffer[nextDataIndex] = latestValue;
            signalPlot.MaxRenderIndex = nextDataIndex;
            
            nextDataIndex += 1;
        }

        void IEcgProvider.Stop()
        {
            startTimerSubscription.Clear();

            nextDataIndex = 1;
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