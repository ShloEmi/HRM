using Norav.HRM.Client.WPF.Modules;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Input;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEcgProvider ecgProvider;

        private string title;
        private string patientName = "John Doe";
        private double? sampleIntervalSec = 10;
        
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


        public DelegateCommand Start => new(ExecuteStart, CanExecuteStart);
        public DelegateCommand Stop => new(ExecuteStop, CanExecuteStop);
        public DelegateCommand Print => new(ExecutePrint, CanExecutePrint);
        public DelegateCommand Exit => new(ExecuteExit, CanExecuteExit);


        private bool canExecuteStart = true;
        private bool CanExecuteStart() => 
            canExecuteStart;

        private void ExecuteStart()
        {
            if (!canExecuteStart)
                return;

            canExecuteStart = false;
            canExecuteStop = true;

            ecgProvider.Start(sampleIntervalSec);

            UpdateCanExecuteStartStopCommands();
        }

        private void UpdateCanExecuteStartStopCommands()
        {
            /* TODO: Shlomi, for some reason this is not working?! */
            Start.RaiseCanExecuteChanged();
            Stop.RaiseCanExecuteChanged();
            // CommandManager.InvalidateRequerySuggested();
        }

        private bool canExecuteStop;
        private bool CanExecuteStop() => 
            canExecuteStop;

        private void ExecuteStop()
        {
            if (!canExecuteStop)
                return;

            canExecuteStart = true;
            canExecuteStop = false;

            UpdateCanExecuteStartStopCommands();

            ecgProvider.Stop();
        }

        private bool canExecutePrint = true;
        private bool CanExecutePrint() => 
            canExecutePrint;

        private void ExecutePrint()
        {
        }

        private bool canExecuteExit = true;
        private bool CanExecuteExit() => 
            canExecuteExit;

        private void ExecuteExit()
        {
            if (!canExecuteStart)
                ExecuteStop();

            Application.Current?.MainWindow?.Close();
        }
    }
}