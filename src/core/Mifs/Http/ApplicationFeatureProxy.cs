using Microsoft.AspNetCore.Http.Features;

namespace Mifs.Http
{
    /// <summary>
    /// Proxy class only required because the IFeatureCollection cannot be accessed once a host has been fully started.
    /// The IFeatureCollection is required to get the port automatically assigned to Kestrel.
    /// This is in turn used to check if a web server has been setup for the created integration.
    /// </summary>
    public class ApplicationFeatureProxy
    {
        public IFeatureCollection? FeatureCollection { get; set; }
    }
}
