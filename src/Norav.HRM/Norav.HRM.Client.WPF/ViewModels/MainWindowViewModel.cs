using Norav.HRM.Client.WPF.Interfaces;
using Norav.HRM.Client.WPF.Modules;
using Prism.Commands;
using Prism.Mvvm;
using System.IO.Abstractions;
using System.Windows;
using System.Windows.Input;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEcgProvider iecgProvider;

        private string title;
        private string patientName;
        
        public MainWindowViewModel(IPlotPresenter plotPresenter, IFileSystem fileSystem, IEcgProvider iecgProvider)
        {
            this.iecgProvider = iecgProvider;
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

        public ICommand Start => new DelegateCommand(ExecuteStart, CanExecuteStart);
        public ICommand Stop => new DelegateCommand(ExecuteStop, CanExecuteStop);
        public ICommand Print => new DelegateCommand(ExecutePrint, CanExecutePrint);
        public ICommand Exit => new DelegateCommand(ExecuteExit, CanExecuteExit);


        private bool CanExecuteStart()
        {
            return true;
        }

        private void ExecuteStart()
        {
            iecgProvider.Start();
        }

        private bool CanExecuteStop()
        {
            return true;
        }

        private void ExecuteStop()
        {
            iecgProvider.Stop();
        }

        private bool CanExecutePrint()
        {
            return true;
        }

        private void ExecutePrint()
        {
        }

        private bool CanExecuteExit()
        {
            return true;
        }

        private void ExecuteExit()
        {
            // exit logic...
            Application.Current?.MainWindow?.Close();
        }
    }
}