using System;

namespace Norav.HRM.Client.WPF.Modules.HeartbeatSimulation
{
    public interface IHeartbeatProvider
    {
        void Start(double? sampleIntervalSec, double? testTimeMin);
        void Stop();

        IObservable<TestState> TestStateChanged { get; }
        IObservable<double> HeartbeatSamples { get; }

    }

    public enum TestState
    {
        Started,
        Stopped,
        TestTimeOver
    }
}