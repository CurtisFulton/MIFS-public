using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Api
{
    internal class ServiceOpService : IServiceOpService
    {
        public ServiceOpService(IMEXHttpService apiService)
        {
            this.ApiService = apiService;
        }

        private IMEXHttpService ApiService { get; }

        public async Task<TResult> Execute<TResult>(string serviceOpName, IEnumerable<ServiceOpParameter> parameters, CancellationToken cancellationToken = default)
        {
            var parameterString = this.GetServiceOpContentString(parameters);
            var httpStringContent = new StringContent(parameterString);

            var result = await this.ApiService.PerformAction<TResult>(actionType: "OData", actionName: serviceOpName, httpStringContent, cancellationToken);
            return result;
        }

        private string GetServiceOpContentString(IEnumerable<ServiceOpParameter> parameters)
        {
            if (parameters?.Any() != true)
            {
                return "[NULL]";
            }

            var paramString = string.Join("", parameters.Select(param => this.GetParameterString(param)));
            return paramString;
        }

        private string GetParameterString(ServiceOpParameter parameter)
        {
            // TODO: Encode and convert smart quotes

            var paramValue = parameter.Value;
            var paramTag = "param";
            if (parameter.IsString)
            {
                paramTag = "paramstring";
            }

            return $"<{paramTag}>{paramValue}</{paramTag}>";
        }
    }
}
