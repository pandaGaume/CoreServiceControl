using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BlueForest.Service
{
    public static class ServiceExtensions
    {
        internal static bool[][] flowValidationMatrix = {
            //          idle , starting, started, stopping, stopped, pausing, paused, complete, completed
            new bool[]{ true , true    , false  , false   , false  , false  , false , true    , false       },//idle
            new bool[]{ true , true    , true   , false   , false  , false  , false , false   , false      },//starting
            new bool[]{ false, false   , true   , true    , false  , true   , false , false   , false      },//started
            new bool[]{ true , false   , false  , true    , true   , false  , false , false   , false      },//stopping
            new bool[]{ false, true    , false  , false   , true   , false  , false , false   , false      },//stopped
            new bool[]{ true , false   , false  , false   , false  , true   , true  , false   , false      },//pausing
            new bool[]{ false, true    , false  , true    , false  , false  , true  , false   , false      },//paused
            new bool[]{ false, false   , false  , false   , false  , false  , false , true    , true       },//complete
            new bool[]{ false, false   , false  , false   , false  , false  , false , false   , true       },//completed
        };

        internal static ConcurrentDictionary<Type, ServiceControlResultCode> codes = new ConcurrentDictionary<Type, ServiceControlResultCode>(
            new KeyValuePair<Type, ServiceControlResultCode>[] {
                new KeyValuePair<Type, ServiceControlResultCode>(typeof(InvalidOperationException), ServiceControlResultCode.InvalidOperation)
            });

        public static void ValidateFlow(this IBlueForestServiceController service, BlueForestServiceStatus status)
        {
            if (!flowValidationMatrix[(int)service.Status][(int)status]) throw new InvalidOperationException(Ressources.ResultMessage.InvalidFlow);
        } 

        public static ServiceControlResultCode GetServiceControlResultCode( this Exception e)
        {
            if( codes.TryGetValue(e.GetType(), out ServiceControlResultCode c))
            {
                return c;
            }
            return ServiceControlResultCode.InternalError;
        }

    }
}
