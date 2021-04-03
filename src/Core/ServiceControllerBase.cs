using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlueForest.Service
{
    public class ServiceControllerBase : IBlueForestServiceController
    {
        BlueForestServiceStatus _status;
        readonly IBlueForestServiceHost _hostedService;
        readonly SemaphoreSlim _lock;

        public ServiceControllerBase(IBlueForestServiceHost hostedService)
        {
            _hostedService = hostedService?? throw new ArgumentNullException( nameof(hostedService));
            _lock = new SemaphoreSlim(1);
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public BlueForestServiceStatus Status => _status;
 
        public async ValueTask<IServiceControlResult> CompleteAsync(CancellationToken token = default)
        {
            try
            {
                await _lock.WaitAsync(token);
                var old = Status;
                try
                {
                    this.ValidateFlow(BlueForestServiceStatus.complete);
                    SetStatus(BlueForestServiceStatus.complete);
                    this.ValidateFlow(BlueForestServiceStatus.completed);
                    await _hostedService.OnCompleteAsync(this);
                    await _hostedService.OnCompletedAsync(this);
                    SetStatus(BlueForestServiceStatus.completed);
                    return new ServiceControlResult(ServiceControlResultCode.Success, old, Status);
                }
                catch (Exception ex)
                {
                    // rollback the change
                    SetStatus(old);
                    return new ServiceControlResult(ex.GetServiceControlResultCode(), old, Status, ex.Message);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async ValueTask<IServiceControlResult> StartAsync(CancellationToken token = default)
        {
            try
            {
                await _lock.WaitAsync(token);
                var old = Status;
                try
                {
                    this.ValidateFlow(BlueForestServiceStatus.starting);
                    SetStatus(BlueForestServiceStatus.starting);
                    this.ValidateFlow(BlueForestServiceStatus.started);
                    await _hostedService.OnStartingAsync(this);
                    await _hostedService.OnStartedAsync(this);
                    SetStatus(BlueForestServiceStatus.started);
                    return new ServiceControlResult(ServiceControlResultCode.Success, old, Status);
                }
                catch (Exception ex)
                {
                    // rollback the change
                    SetStatus(old);
                    return new ServiceControlResult(ex.GetServiceControlResultCode(), old, Status, ex.Message);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async ValueTask<IServiceControlResult> StopAsync(CancellationToken token = default)
        {
            try
            {
                await _lock.WaitAsync(token);
                var old = Status;
                try
                {
                    this.ValidateFlow(BlueForestServiceStatus.stopping);
                    SetStatus(BlueForestServiceStatus.stopping);
                    this.ValidateFlow(BlueForestServiceStatus.stopped);
                    await _hostedService.OnStoppingAsync(this);
                    await _hostedService.OnStoppedAsync(this);
                    SetStatus(BlueForestServiceStatus.stopped);
                    return new ServiceControlResult(ServiceControlResultCode.Success, old, Status);
                }
                catch (Exception ex)
                {
                    // rollback the change
                    SetStatus(old);
                    return new ServiceControlResult(ex.GetServiceControlResultCode(), old, Status, ex.Message);
                }
            }
            finally
            {
                _lock.Release();
            }
        }
        public async ValueTask<IServiceControlResult> PauseAsync(CancellationToken token = default)
        {
            try
            {
                await _lock.WaitAsync(token);
                var old = Status;
                try
                {
                    this.ValidateFlow(BlueForestServiceStatus.pausing);
                    SetStatus(BlueForestServiceStatus.pausing);
                    this.ValidateFlow(BlueForestServiceStatus.paused);
                    await _hostedService.OnPausingAsync(this);
                    await _hostedService.OnPausedAsync(this);
                    SetStatus(BlueForestServiceStatus.paused);
                    return new ServiceControlResult(ServiceControlResultCode.Success, old, Status);
                }
                catch (Exception ex)
                {
                    // rollback the change
                    SetStatus(old);
                    return new ServiceControlResult(ex.GetServiceControlResultCode(), old, Status, ex.Message);
                }
            }
            finally
            {
                _lock.Release();
            }
        }
        private void SetStatus(BlueForestServiceStatus status)
        {
            var old = _status;
            _status = status;
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(old, _status));
        }

        internal class ServiceControlResult : IServiceControlResult
        {
            readonly ServiceControlResultCode _c;
            readonly BlueForestServiceStatus _o, _n;
            readonly string _m;

            internal ServiceControlResult() { }
            internal ServiceControlResult(ServiceControlResultCode code, BlueForestServiceStatus o, BlueForestServiceStatus? n=null, string mess = null) 
            {
                _c = code;
                _o = o;
                _n = n??o;
                _m = mess;
            }

            public ServiceControlResultCode Code => _c;
            public string Message => _m;
            public BlueForestServiceStatus OldStatus => _o;
            public BlueForestServiceStatus NewStatus => _n;
        }
    }
}
