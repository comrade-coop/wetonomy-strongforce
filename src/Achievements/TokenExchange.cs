namespace Achievements
{
	using System.Numerics;
	using ContractsCore;
	using ContractsCore.Contracts;
	using TokenSystem.TokenManagerBase.Actions;
	using Action = ContractsCore.Actions.Action;

	public class TokenExchange : Contract
	{
		public TokenExchange(
			Address address,
			Address burnTokenManager,
			Address mintTokenManager,
			int exchangeRate)
			: base(address)
		{
			this.BurnTokenManager = burnTokenManager;
			this.MintTokenManager = mintTokenManager;
			this.ExchangeRate = exchangeRate;
		}

		public Address BurnTokenManager { get; }

		public Address MintTokenManager { get; }

		public int ExchangeRate { get; }

		protected override object GetState()
		{
			throw new System.NotImplementedException();
		}

		protected override bool HandleReceivedAction(Action action)
		{
			switch (action)
			{
				case TokensReceivedAction receivedAction:
					return this.ExchangeTokens(receivedAction);
				default:
					return false;
			}
		}

		protected virtual bool ExchangeTokens(TokensReceivedAction action)
		{
			if (!Equals(action.Sender, this.BurnTokenManager))
			{
				return false;
			}

			BigInteger receivedAmount = action.Tokens.TotalBalance;
			var burnAction = new BurnAction(
				string.Empty,
				this.BurnTokenManager,
				receivedAmount,
				this.Address);
			this.OnSend(burnAction);

			BigInteger mintAmount = receivedAmount * this.ExchangeRate;
			var mintAction = new MintAction(
				string.Empty,
				this.MintTokenManager,
				mintAmount,
				action.TokensSender);
			this.OnSend(mintAction);

			return true;
		}
	}
}