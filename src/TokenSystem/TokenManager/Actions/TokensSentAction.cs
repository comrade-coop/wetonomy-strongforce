using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokensSentAction<TTokenTagType> : TokenAction<TTokenTagType>
	{
		public TokensSentAction(
			string hash,
			Address target,
			Address tokensReceiver,
			IReadOnlyTaggedTokens<TTokenTagType> tokens)
			: base(hash, target, tokens)
		{
			this.TokensReceiver = tokensReceiver;
		}

		public Address TokensReceiver { get; }
	}
}