using log4net;
using Norav.HRM.Client.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using Norav.HRM.Client.WPF.Modules.HeartbeatSimulation;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly string ReportsFolder = "Reports";

        private readonly IHeartbeatProvider heartbeatProvider;
        private readonly IPlotPresenter plotPresenter;
        private readonly IFileSystem fileSystem;

        private string title;
        private string patientName = "John Doe";
        private double? sampleIntervalSec = 10;
        private double? testTimeMin = 60;
        private bool isExecuting;
        private double bpm;



        public MainWindowViewModel(IHeartbeatProvider heartbeatProvider, IPlotPresenter plotPresenter, IFileSystem fileSystem, IScheduler dispatcherScheduler)
        {
            this.heartbeatProvider = heartbeatProvider;
            this.plotPresenter = plotPresenter;
            this.fileSystem = fileSystem;
            Title = "Heartbeat Test";

            heartbeatProvider.TestStateChanged
                .ObserveOn(dispatcherScheduler)
                .Subscribe(OnTestStateChanged);
            heartbeatProvider.HeartbeatSamples
                .Sample(TimeSpan.FromSeconds(1))
                .ObserveOn(dispatcherScheduler)
                .Subscribe(OnHeartbeatSample);
        }


        private void OnHeartbeatSample(double ecgSample) => 
            BPM = ecgSample;

        private void OnTestStateChanged(TestState testState)
        {
            switch (testState)
            {
                case TestState.Started:
                    Log.Info("called, TestState.Started");
                    break;
                case TestState.Stopped:
                    Log.Info("called, TestState.Stopped");
                    break;
                case TestState.TestTimeOver:
                    Log.Info("called, TestState.TestTimeOver");

                    ExecuteStop();
                    break;
                default:
                    Log.Error($"Unknown/Unhandled TestState: {(int)testState}");
                    break;
            }
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string PatientName
        {
            get => patientName;
            set => SetProperty(ref patientName, value);
        }

        public double BPM
        {
            get => bpm;
            set => SetProperty(ref bpm, value);
        }


        public double? SampleIntervalSec
        {
            get => sampleIntervalSec;
            set => SetProperty(ref sampleIntervalSec, value);
        }

        public double? TestTimeMin
        {
            get => testTimeMin;
            set => SetProperty(ref testTimeMin, value);
        }

        public bool IsExecuting
        {
            get => isExecuting;
            set => SetProperty(ref isExecuting, value);
        }


        public DelegateCommand Start => new(ExecuteStart);
        public DelegateCommand Stop => new(ExecuteStop);
        public DelegateCommand Print => new(ExecutePrint);
        public DelegateCommand Exit => new(ExecuteExit);



        private void ExecuteStart()
        {
            IsExecuting = true;
            heartbeatProvider.Start(sampleIntervalSec, testTimeMin);
        }

        private void ExecuteStop()
        {
            IsExecuting = false;
            heartbeatProvider.Stop();
        }

        private void ExecutePrint()
        {
            EnsureReportsFolder();

            var reportPath = $@"{ReportsFolder}\{PatientNameAsValidFileName()}.{ValidFileNameTimeStamp()}.png";

            plotPresenter.Plot.SaveFig(reportPath);
        }

        private static string ValidFileNameTimeStamp()
        {
            return DateTime.Now.ToString("s").Replace(":", "-");
        }

        private string PatientNameAsValidFileName()
        {
            return Path.GetInvalidFileNameChars().Aggregate(PatientName, (current, c) => current.Replace(c, '_'));
        }

        private void EnsureReportsFolder()
        {
            if (fileSystem.Directory.Exists(ReportsFolder))
                return;

            fileSystem.Directory.CreateDirectory(ReportsFolder);
        }

        private void ExecuteExit() => 
            Application.Current?.MainWindow?.Close();
    }
}