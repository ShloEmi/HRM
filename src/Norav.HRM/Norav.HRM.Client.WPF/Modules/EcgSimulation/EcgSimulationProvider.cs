using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using log4net;
using Norav.HRM.Client.WPF.Interfaces;
using ScottPlot.Plottable;

namespace Norav.HRM.Client.WPF.Modules.EcgSimulation
{
    public class EcgSimulationProvider : IEcgProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

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
            plotPresenter.Plot.XLabel("Sample");

            plotPresenter.Plot?.Title("Heartbeat graph", false);

            signalPlot = plotPresenter.Plot.AddSignal(dataBuffer);
            plotPresenter.Refresh();
        }

        private void OnStarting(Unit _)
        {
            Log.Info("called");
        }

        private void OnExiting(Unit _)
        {
            Log.Info("called");

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
            double randomValue = Math.Round(randomGenerator.NextDouble() - .5, 3);
            dataBuffer[nextDataIndex] = dataBuffer[nextDataIndex - 1] + randomValue;
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
}