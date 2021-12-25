using ScottPlot;

namespace Norav.HRM.Client.WPF.Interfaces
{
    public interface IPlotPresenter
    {
        Plot Plot { get; }
        void Refresh(bool lowQuality = false);
    }
}