using System.Numerics;
using ContractsCore;
using ContractsCore.Actions;
using ContractsCore.Contracts;
using TokenSystem.TokenManagerBase.Actions;

namespace Achievements
{
	public class Achievement : Contract
	{
		public Achievement(
			Address address,
			Address burnTokenManager,
			Address mintTokenManager,
			int exchangeRate,
			Address achiever,
			AchievementMetaData metaData)
			: base(address)
		{
			this.BurnTokenManager = burnTokenManager;
			this.MintTokenManager = mintTokenManager;
			this.ExchangeRate = exchangeRate;
			this.Achiever = achiever;
			this.MetaData = metaData;
		}

		public Address BurnTokenManager { get; }

		public Address MintTokenManager { get; }

		public int ExchangeRate { get; }

		public Address Achiever { get; }

		private AchievementMetaData MetaData { get; }

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

		private bool ExchangeTokens(TokensReceivedAction action)
		{
			if (!this.CanExchange())
			{
				return false;
			}

			BigInteger receivedAmount = action.Tokens.TotalBalance;
			this.OnSend(new BurnAction(
				string.Empty,
				this.BurnTokenManager,
				receivedAmount,
				this.Address));

			BigInteger mintAmount = receivedAmount * this.ExchangeRate;
			this.OnSend(new MintAction(
				string.Empty,
				this.MintTokenManager,
				mintAmount,
				this.Address));
			this.OnSend(new TransferAction(
				string.Empty,
				this.MintTokenManager,
				mintAmount,
				this.Address,
				this.Achiever));

			return true;
		}

		private bool CanExchange()
		{
			//TODO: Implement time limit for rewarding achievements
			//(May need additional feature from StrongForce)
			return true;
		}
	}
}