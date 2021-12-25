using Norav.HRM.Client.WPF.Interfaces;
using Prism.Mvvm;

namespace Norav.HRM.Client.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IPlotProvider plotProvider;

        private string title;
        private string patientName;
        

        public MainWindowViewModel()
            : this(null)
        {
        }

        public MainWindowViewModel(IPlotProvider plotProvider)
        {
            this.plotProvider = plotProvider;
            Title = "Heartbeat Test";


#if false
            string[] readAllLines = File.ReadAllLines(@"ExampleData\ECG.data.csv");
            double[] dataY = readAllLines/*.Take(10*1024)*/.Select(double.Parse).ToArray();
            this.plotProvider.Plot.AddSignal(dataY);
#endif

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
    }
}