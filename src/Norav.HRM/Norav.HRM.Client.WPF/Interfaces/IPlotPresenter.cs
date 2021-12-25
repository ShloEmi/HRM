using ScottPlot;

namespace Norav.HRM.Client.WPF.Interfaces
{
    public interface IPlotPresenter
    {   /* TODO: Shlomi, TBC - Should be decoupled from ScottPlot */
        Plot Plot { get; }
        void Refresh(bool lowQuality = false);
    }
}