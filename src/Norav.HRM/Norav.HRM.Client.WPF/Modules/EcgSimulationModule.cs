using Norav.HRM.Client.WPF.Interfaces;
using Prism.Ioc;
using Prism.Modularity;
using ScottPlot.Plottable;
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Norav.HRM.Client.WPF.Modules
{
    public class EcgSimulationProvider : IEcgProvider
    {
        private readonly CompositeDisposable eventSubscription = new();
        private CompositeDisposable startTimerSubscription;
        private readonly Subject<TestState> testStateChanged = new();

        private readonly IPlotPresenter plotPresenter;

        private readonly double[] dataBuffer = new double[1_000_000];
        private int nextDataIndex = 1;
        private SignalPlot signalPlot;
        private readonly Random randomGenerator;

        private readonly double refreshRateHz = 1d / 5d;
        

        public EcgSimulationProvider(IApplicationEvents applicationEvents, IPlotPresenter plotPresenter)
        {
            this.plotPresenter = plotPresenter;
            randomGenerator = new Random();

            eventSubscription.Add(applicationEvents.Initializing.Subscribe(OnInitialized));
            eventSubscription.Add(applicationEvents.Starting.Subscribe(OnStarting));
            eventSubscription.Add(applicationEvents.Exiting.Subscribe(OnExiting));
        }


        private void OnInitialized(Unit _)
        {
            plotPresenter.Plot.YLabel("Value");
            plotPresenter.Plot.XLabel("Time [Sec]");

            plotPresenter.Plot?.Title("Heartbeat graph", false);

            signalPlot = plotPresenter.Plot.AddSignal(dataBuffer);
        }

        private void OnStarting(Unit _)
        {
            /* TODO: Shlomi, add log */
        }

        private void OnExiting(Unit _)
        {
            eventSubscription.Clear();
            startTimerSubscription?.Clear();
        }


        /// <param name="sampleIntervalSec"></param>
        /// <param name="testTimeMin"></param>
        /// <seealso cref="https://github.com/ScottPlot/ScottPlot/blob/master/src/demo/ScottPlot.Demo.WPF/WpfDemos/LiveDataGrowing.xaml.cs"/>
        void IEcgProvider.Start(double? sampleIntervalSec, double? testTimeMin)
        {
            double sampleIntervalSecValue = 10;
            double testTimeMinValue = 60;

            if (sampleIntervalSec != null)
                sampleIntervalSecValue = sampleIntervalSec.Value;
            if (testTimeMin != null)
                testTimeMinValue = testTimeMin.Value;


            testStateChanged.OnNext(TestState.Started);

            startTimerSubscription = new CompositeDisposable
            {
                Observable
                    .Interval(TimeSpan.FromSeconds(sampleIntervalSecValue))
                    .ObserveOn(DispatcherScheduler.Current)
                    .Subscribe(OnEcgSamples),
                Observable
                    .Interval(TimeSpan.FromSeconds(refreshRateHz))
                    .ObserveOn(DispatcherScheduler.Current)
                    .Subscribe(OnRender),
                Observable
                    .Interval(TimeSpan.FromMinutes(testTimeMinValue))
                    .ObserveOn(DispatcherScheduler.Current)
                    .Subscribe(OnTestTimeOver)
            };
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

            double randomValue = Math.Round(randomGenerator.NextDouble() - .5, 3);
            double latestValue = dataBuffer[nextDataIndex - 1] + randomValue;
            dataBuffer[nextDataIndex] = latestValue;
            signalPlot.MaxRenderIndex = nextDataIndex;
            
            nextDataIndex += 1;
        }

        private void OnTestTimeOver(long elapsed)
        {
            ((IEcgProvider)this).Stop();
            testStateChanged.OnNext(TestState.TestTimeOver);
        }

        void IEcgProvider.Stop()
        {
            startTimerSubscription?.Clear();
            startTimerSubscription = null;

            nextDataIndex = 1;

            testStateChanged.OnNext(TestState.Stopped);
        }


        public IObservable<TestState> TestStateChanged => testStateChanged;
    }

    public enum TestState
    {
        Started,
        Stopped,
        TestTimeOver
    }

    public interface IEcgProvider
    {
        void Start(double? sampleIntervalSec, double? testTimeMin);
        void Stop();
        IObservable<TestState> TestStateChanged { get; }
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