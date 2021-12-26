﻿using System;

namespace Norav.HRM.Client.WPF.Modules.EcgSimulation
{
    public interface IEcgProvider
    {
        void Start(double? sampleIntervalSec, double? testTimeMin);
        void Stop();
        IObservable<TestState> TestStateChanged { get; }
    }

    public enum TestState
    {
        Started,
        Stopped,
        TestTimeOver
    }
}