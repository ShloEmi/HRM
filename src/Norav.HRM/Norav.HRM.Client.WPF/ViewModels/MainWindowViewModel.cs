using Norav.HRM.Client.WPF.Modules;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEcgProvider ecgProvider;

        private string title;
        private string patientName = "John Doe";
        private double? sampleIntervalSec = 10;
        private bool isExecuting;


        public MainWindowViewModel(IEcgProvider ecgProvider)
        {
            this.ecgProvider = ecgProvider;
            Title = "Heartbeat Test";
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
            ecgProvider.Start(sampleIntervalSec);
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