using System.Collections.Generic;

namespace HomeBankingMindHub.Models
{
    public class ClientDTO
    {

        //public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<AccountDTO> Accounts { get; set; }
        public long Id { get; internal set; }
    }
}
