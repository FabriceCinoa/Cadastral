using AccountRepository.DBContext;
using AccountRepository.Entities;
using Common.Repository.PostGre;
using Microsoft.Extensions.Configuration;

namespace AccountRepository.Repositories;

public class AccountPostGreRepository : APostGreRepository<AuthPostGreDBContext>
{

    public AccountPostGreRepository(IConfiguration configuration) : base(new AuthPostGreDBContext(configuration))
    {
        
        
    }

    public IList<Account> GetAllAccounts()
    {
        return this.Context.Accounts.ToList();
    }
}
