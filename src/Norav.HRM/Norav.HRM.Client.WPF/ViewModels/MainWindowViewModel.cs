using Norav.HRM.Client.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using System.IO.Abstractions;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IPlotPresenter plotPresenter;
        private readonly IFileSystem fileSystem;

        private string title;
        private string patientName;
        
        public MainWindowViewModel(IPlotPresenter plotPresenter, IFileSystem fileSystem)
        {
            this.plotPresenter = plotPresenter;
            this.fileSystem = fileSystem;
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
            string[] readAllLines = fileSystem.File.ReadAllLines(@"ExampleData\ECG.data.csv");
            double[] dataY = readAllLines.Select(double.Parse).ToArray();
            plotPresenter.Plot.AddSignal(dataY);
            plotPresenter.Plot?.Title("Heartbeat graph");
            plotPresenter.Refresh();
        }

        private bool CanExecuteStop()
        {
            return true;
        }

        private void ExecuteStop()
        {
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