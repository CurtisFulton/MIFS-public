using System.Threading.Tasks;

namespace Mifs.MEX.Authentication
{
    public interface IMEXAuthenticationService
    {
        Task<MEXJwtToken> GetAccessToken();
    }
}
