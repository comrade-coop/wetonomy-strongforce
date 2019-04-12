using System.Collections.Generic;
using ContractsCore;
using TokenSystem.TokenEventArgs;
using TokenSystem.TokenManager;

namespace TokenSystem.TokenFlow
{
	public abstract class TokenSplitter<TTokenTagType> : RecipientManager
	{
		protected TokenManager<TTokenTagType> TokenManager { get; }

		public TokenSplitter(
			Address address,
			TokenManager<TTokenTagType> tokenManager)
			: this(address, new List<Address>(), tokenManager)
		{
		}

		public TokenSplitter(
			Address address,
			IList<Address> recipients,
			TokenManager<TTokenTagType> tokenManager)
			: base(address, recipients)
		{
			this.TokenManager = tokenManager;

			this.TokenManager.TokensMinted += this.OnTokensMinted;
			this.TokenManager.TokensTransferred += this.OnTokensTransferred;
		}

		private void OnTokensMinted(object sender, TokensMintedEventArgs<TTokenTagType> mintedEventArgs)
		{
			if (!(sender is TokenManager<TTokenTagType> tokenManagerSender))
			{
				return;
			}

			if (tokenManagerSender.Address.Equals(this.TokenManager.Address) && mintedEventArgs.To.Equals(this.Address))
			{
				this.Split(mintedEventArgs.Amount);
			}
		}

		private void OnTokensTransferred(object sender, TokensTransferredEventArgs<TTokenTagType> transferredEventArgs)
		{
			if (!(sender is TokenManager<TTokenTagType> tokenManagerSender))
			{
				return;
			}

			if (tokenManagerSender.Address.Equals(this.TokenManager.Address)
			    && transferredEventArgs.To.Equals(this.Address))
			{
				this.Split(transferredEventArgs.Amount);
			}
		}

		protected abstract void Split(decimal amount);
	}
}