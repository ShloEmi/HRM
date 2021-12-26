using Norav.HRM.Client.WPF.Interfaces;
using Prism.Ioc;
using ScottPlot;

namespace Norav.HRM.Client.WPF.Views
{
    public partial class MainWindow : IPlotPresenter
    {
        public MainWindow(IContainerRegistry containerRegistry)
        {
            // REMARKS: workaround for: 'plot not supporting MVVM'
            containerRegistry.Register<IPlotPresenter>(() => this); /* TODO: Shlomi, can this workaround be avoided? */
            
            InitializeComponent();
        }
        

        /// <remarks>
        /// ScottPlot does not support MVVM in favor of performance! https://scottplot.net/faq/mvvm/ , this is a workaround to still work in MVVM
        /// https://scottplot.net/faq/
        /// https://github.com/ScottPlot/ScottPlot/blob/master/src/controls/ScottPlot.WPF/WpfPlot.xaml
        /// </remarks>
        public Plot Plot => 
            PlotPresenter?.Plot;

        public void Refresh(bool lowQuality = false) => 
            PlotPresenter.Refresh(lowQuality);
    }
}
