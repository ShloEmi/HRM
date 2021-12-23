using System.IO;
using System.Linq;

namespace Norav.HRM.Client.WPF
{
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();

#if false
            string[] readAllLines = File.ReadAllLines(@"ExampleData\ECG.data.csv");
            double[] dataY = readAllLines.Take(10*1024).Select(double.Parse).ToArray();
            double[] dataX = Enumerable.Range(0, dataY.Length).Select(i => (double)i).ToArray();
            WpfPlot1.Plot.AddScatter(dataX, dataY);
#endif

            WpfPlot1.Refresh();
        }
    }
}
