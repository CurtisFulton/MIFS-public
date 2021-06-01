using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mifs.Extensions
{
    public static class Task_Extensions
    {
        public static async Task<TResult?> FirstOrDefault<TResult>(this Task<IEnumerable<TResult>> task)
            => (await task).FirstOrDefault();
    }
}
