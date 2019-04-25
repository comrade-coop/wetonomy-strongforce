using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokensSentAction<TTokenTagType> : Action
	{
		public TokensSentAction(
			string hash,
			Address origin,
			Address sender,
			Address target,
			Address tokensReceiver,
			IReadOnlyTaggedTokens<TTokenTagType> tokens)
			: base(hash, origin, sender, target)
		{
			this.TokensReceiver = tokensReceiver;
			this.Tokens = tokens;
		}

		public Address TokensReceiver { get; }

		public IReadOnlyTaggedTokens<TTokenTagType> Tokens { get; }
	}
}