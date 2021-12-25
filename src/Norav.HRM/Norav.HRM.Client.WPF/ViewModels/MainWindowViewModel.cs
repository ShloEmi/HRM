using System.Windows.Input;
using Norav.HRM.Client.WPF.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IPlotProvider plotProvider;

        private string title;
        private string patientName;
        
        public MainWindowViewModel(IPlotProvider plotProvider)
        {
            this.plotProvider = plotProvider;
            Title = "Heartbeat Test";


#if false
            string[] readAllLines = File.ReadAllLines(@"ExampleData\ECG.data.csv");
            double[] dataY = readAllLines/*.Take(10*1024)*/.Select(double.Parse).ToArray();
            this.plotProvider.Plot.AddSignal(dataY);
#endif
            plotProvider.Plot?.Title("title");
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
        }
    }
}