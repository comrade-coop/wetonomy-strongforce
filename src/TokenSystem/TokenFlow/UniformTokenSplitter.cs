using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Events;
using TokenSystem.TokenManager;
using TokenSystem.TokenManager.Actions;

namespace TokenSystem.TokenFlow
{
	public class UniformTokenSplitter<TTokenTagType> : TokenSplitter<TTokenTagType>
	{
		public UniformTokenSplitter(Address address, TokenManager<TTokenTagType> tokenManager)
			: base(address, tokenManager)
		{
		}

		public UniformTokenSplitter(
			Address address,
			IList<Address> recipients,
			TokenManager<TTokenTagType> tokenManager)
			: base(address, recipients, tokenManager)
		{
		}

		protected override void Split(BigInteger amount)
		{
			BigInteger splitAmount = amount / this.Recipients.Count;

			if (splitAmount <= 0)
			{
				return;
			}

			foreach (Address recipient in this.Recipients)
			{
				TransferAction<TTokenTagType> transferAction = new TransferAction<TTokenTagType>(
					string.Empty,
					this.Address,
					this.Address,
					this.TokenManager.Address,
					splitAmount,
					this.Address,
					recipient);
				this.OnSend(new ActionEventArgs(transferAction));
			}
		}
	}
}