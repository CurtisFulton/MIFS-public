using Microsoft.EntityFrameworkCore;
using Mifs.Extensions;
using Mifs.MEX.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Services
{
    public interface IAccountCodeService
    {
        Task<AccountCode> GetOrCreateAccountCode(string accountCodeName, string accountCodeDescription, IMEXDbContext dbContext, CancellationToken cancellationToken = default);
    }

    internal class AccountCodeService : IAccountCodeService
    {
        public async Task<AccountCode> GetOrCreateAccountCode(string accountCodeName,
                                                              string accountCodeDescription,
                                                              IMEXDbContext dbContext,
                                                              CancellationToken cancellationToken = default)
        {
            if (accountCodeName.IsNullOrWhiteSpace())
            {
                return null;
            }

            var accountCode = await dbContext.AccountCodes.FirstOrDefaultAsync(x => x.AccountCodeName.Trim().ToLower() == accountCodeName.Trim().ToLower(), cancellationToken);
            if (accountCode is null)
            {
                accountCode = new AccountCode()
                {
                    AccountCodeName = accountCodeName,
                    AccountCodeDescription = accountCodeDescription,
                    IsActive = true
                };

                dbContext.AccountCodes.Add(accountCode);
            }

            return accountCode;
        }
    }
}
