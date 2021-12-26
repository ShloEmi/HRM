using log4net;
using Norav.HRM.Client.WPF.Modules;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IEcgProvider ecgProvider;

        private string title;
        private string patientName = "John Doe";
        private double? sampleIntervalSec = 10;
        private double? testTimeMin = 60;
        private bool isExecuting;


        public MainWindowViewModel(IEcgProvider ecgProvider)
        {
            this.ecgProvider = ecgProvider;
            Title = "Heartbeat Test";

            ecgProvider.TestStateChanged
                    .ObserveOn(DispatcherScheduler.Current)
                    .Subscribe(OnTestStateChanged);
        }


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
            ecgProvider.Start(sampleIntervalSec, testTimeMin);
        }

        private void ExecuteStop()
        {
            IsExecuting = false;
            ecgProvider.Stop();
        }

        private void ExecutePrint()
        {
        }

        private void ExecuteExit() => 
            Application.Current?.MainWindow?.Close();
    }
}