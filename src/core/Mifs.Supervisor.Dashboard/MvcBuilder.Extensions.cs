using Microsoft.Extensions.DependencyInjection;
using Mifs.Http;

namespace Mifs.Supervisor.Dashboard
{
    public static class MvcBuilder_Extensions
    {
        public static IMvcBuilder AddMifsDashboard(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(typeof(MvcBuilder_Extensions).Assembly);
            mvcBuilder.AddRazorApplicationPart(typeof(MvcBuilder_Extensions).Assembly);
            return mvcBuilder;
        }
    }
}
