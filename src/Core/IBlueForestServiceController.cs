using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlueForest.Service
{
    public enum BlueForestServiceStatus
    {
        idle,
        starting,
        started,
        stopping,
        stopped,
        pausing,
        paused,
        complete,
        completed
    }

    public enum ServiceControlResultCode
    {
        Success = 0,
        InvalidOperation = 128,
        InternalError = 512
    }

    public interface IServiceControlResult
    {
        ServiceControlResultCode Code { get; }
        string Message { get; }
        BlueForestServiceStatus OldStatus { get; }
        BlueForestServiceStatus NewStatus { get; }
    }

    [Serializable]
    public class StatusChangedEventArgs : EventArgs
    {
        readonly BlueForestServiceStatus _o, _n;

        public StatusChangedEventArgs(BlueForestServiceStatus oldStatus, BlueForestServiceStatus newStatus)
        {
            _o = oldStatus;
            _n = newStatus;
        }
        public BlueForestServiceStatus OldStatus => _o;
        public BlueForestServiceStatus NewStatus => _n;
    }

    public interface IBlueForestServiceController
    {
        event EventHandler<StatusChangedEventArgs> StatusChanged;
        BlueForestServiceStatus Status { get; }
        ValueTask<IServiceControlResult> StartAsync(CancellationToken token = default);
        ValueTask<IServiceControlResult> PauseAsync(CancellationToken token = default);
        ValueTask<IServiceControlResult> StopAsync(CancellationToken token = default);
        ValueTask<IServiceControlResult> CompleteAsync(CancellationToken token = default);
    }

}
