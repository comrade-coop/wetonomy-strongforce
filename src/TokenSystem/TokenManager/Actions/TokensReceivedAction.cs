using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokensReceivedAction<TTokenTagType> : TokenAction<TTokenTagType>
	{
		public TokensReceivedAction(
			string hash,
			Address target,
			Address tokensSender,
			IReadOnlyTaggedTokens<TTokenTagType> tokens)
			: base(hash, target, tokens)
		{
			this.TokensSender = tokensSender;
		}

		public Address TokensSender { get; }
	}
}