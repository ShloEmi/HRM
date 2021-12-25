using Norav.HRM.Client.WPF.Interfaces;
using Norav.HRM.Client.WPF.Modules;
using Prism.Ioc;
using ScottPlot;

namespace Norav.HRM.Client.WPF.Views
{
    public partial class MainWindow : IPlotProvider
    {
        public MainWindow(IECGAdapter ecgAdapter, IContainerRegistry containerRegistry)
        {
            // REMARKS: workaround for: 'plot not supporting MVVM'
            containerRegistry.Register<IPlotProvider>(() => this);

            InitializeComponent();
            
            WpfPlot1.Refresh();
        }
        

        /// <remarks>
        /// ScottPlot does not support MVVM in favor of performance! https://scottplot.net/faq/mvvm/ , this is a workaround to still work in MVVM
        /// https://scottplot.net/faq/
        /// https://github.com/ScottPlot/ScottPlot/blob/master/src/controls/ScottPlot.WPF/WpfPlot.xaml
        /// </remarks>
        public Plot Plot => WpfPlot1?.Plot;
    }
}
