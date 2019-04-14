using System.Collections.Generic;
using System.Numerics;
using ContractsCore;

namespace TokenSystem.TokenFlow
{
	public abstract class TokenBurner : RecipientManager
	{
		public TokenBurner(Address address) : base(address)
		{
		}

		public TokenBurner(Address address, IList<Address> recipients) : base(address, recipients)
		{
		}

		protected abstract void Burn(BigInteger amount);
	}
}