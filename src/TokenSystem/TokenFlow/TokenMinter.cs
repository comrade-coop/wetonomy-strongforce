using System.Collections.Generic;
using ContractsCore;

namespace TokenSystem.TokenFlow
{
    public class TokenMinter : RecipientManager
    {
        public TokenMinter(Address address) : base(address)
        {
        }

        public TokenMinter(Address address, IList<Address> recipients) : base(address, recipients)
        {
        }
    }
}