using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountRepository.Entities
{

    [Table("account", Schema = "auth")]
    public class Account
    {
        public string id { get; set; }
        public string  account_id { get; set; }
        public string password { get; set; }
    }
}
