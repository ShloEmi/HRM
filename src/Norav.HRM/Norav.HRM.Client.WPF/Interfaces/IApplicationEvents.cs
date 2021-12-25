using System;
using System.Reactive;

namespace Norav.HRM.Client.WPF.Interfaces
{
    public interface IApplicationEvents
    {
        IObservable<Unit> Initializing { get; }
        IObservable<Unit> Starting { get; }
        IObservable<Unit> Exiting { get; }
    }
}