using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    public static class ServiceOpService_Extensions
    {
        public static Task<TResult> Execute<TResult>(this IServiceOpService serviceOpService, string serviceOpName, CancellationToken cancellationToken)
            => serviceOpService.Execute<TResult>(serviceOpName, null, cancellationToken);

        public static Task Execute(this IServiceOpService serviceOpService, string serviceOpName, CancellationToken cancellationToken)
            => serviceOpService.Execute<object>(serviceOpName, cancellationToken);



        public static Task Execute(this IServiceOpService serviceOpService, string serviceOpName, CancellationToken cancellationToken, params object[] parameters)
            => serviceOpService.Execute<object>(serviceOpName, cancellationToken, parameters);

        public static Task<TResult> Execute<TResult>(this IServiceOpService serviceOpService, string serviceOpName, CancellationToken cancellationToken, params object[] parameters)
        {
            var serviceOpParameters = new List<ServiceOpParameter>(parameters.Length);

            foreach (var param in parameters)
            {
                var paramValue = param;

                // Check if the object is already a service op parameter. 
                // If it is, we just return it directly. Otherwise create a SerivceOpParameter for it.
                var serviceOpParameter = paramValue switch
                {
                    ServiceOpParameter premadeParameter => premadeParameter,
                    _ => new ServiceOpParameter(paramValue, IsString: paramValue is string)
                };

                serviceOpParameters.Add(serviceOpParameter);
            }

            return serviceOpService.Execute<TResult>(serviceOpName, serviceOpParameters, cancellationToken);
        }
    }
}
