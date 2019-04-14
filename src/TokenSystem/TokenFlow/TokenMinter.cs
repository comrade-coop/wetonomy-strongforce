using System.Collections.Generic;
using System.Numerics;
using ContractsCore;

namespace TokenSystem.TokenFlow
{
	public abstract class TokenMinter : RecipientManager
	{
		public TokenMinter(Address address) : base(address)
		{
		}

		public TokenMinter(Address address, IList<Address> recipients) : base(address, recipients)
		{
		}

		protected abstract void Mint(BigInteger amount);
	}
}