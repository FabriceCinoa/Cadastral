using AccountRepository.Entities;
using AccountRepository.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
       


        public TestController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [HttpGet(Name = "Accounts")]
        public IEnumerable<Account> Get()
        {
            var r = new AccountPostGreRepository(Configuration);

            return r.GetAllAccounts();
        }
    }
}
