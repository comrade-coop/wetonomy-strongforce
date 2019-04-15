using System.Collections.Generic;
using System.Numerics;
using ContractsCore;
using ContractsCore.Events;
using TokenSystem.TokenManager;
using TokenSystem.TokenManager.Actions;

namespace TokenSystem.TokenFlow
{
	public class UniformOnBurnTokenMinter<TTokenTagType> : OnBurnTokenMinter<TTokenTagType>
	{
		public UniformOnBurnTokenMinter(Address address, TokenManager<TTokenTagType> tokenManager)
			: base(address, tokenManager)
		{
		}

		public UniformOnBurnTokenMinter(
			Address address,
			TokenManager<TTokenTagType> tokenManager,
			IList<Address> recipients)
			: base(address, tokenManager, recipients)
		{
		}

		protected override void Mint(BigInteger transferAmount)
		{
			BigInteger mintAmountPerRecipient = transferAmount / this.Recipients.Count;

			foreach (Address recipient in this.Recipients)
			{
				var mintAction = new MintAction(
					string.Empty,
					this.Address,
					this.Address,
					this.TokenManager.Address,
					mintAmountPerRecipient,
					recipient);
				this.OnSend(new ActionEventArgs(mintAction));
			}
		}
	}
}