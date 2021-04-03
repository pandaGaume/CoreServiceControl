using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueForest.Service
{
    public interface IBlueForestServiceHost
    {
        ValueTask OnIdleAsync(IBlueForestServiceController controller, Exception reason = null);
        ValueTask OnStartingAsync(IBlueForestServiceController controller);
        ValueTask OnStartedAsync(IBlueForestServiceController controller);
        ValueTask OnStoppingAsync(IBlueForestServiceController controller);
        ValueTask OnStoppedAsync(IBlueForestServiceController controller);
        ValueTask OnPausingAsync(IBlueForestServiceController controller);
        ValueTask OnPausedAsync(IBlueForestServiceController controller);
        ValueTask OnCompleteAsync(IBlueForestServiceController controller);
        ValueTask OnCompletedAsync(IBlueForestServiceController controller);
    }
}
