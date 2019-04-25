using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokensReceivedAction<TTokenTagType> : Action
	{
		public TokensReceivedAction(
			string hash,
			Address origin,
			Address sender,
			Address target,
			Address tokensSender,
			IReadOnlyTaggedTokens<TTokenTagType> tokens)
			: base(hash, origin, sender, target)
		{
			this.TokensSender = tokensSender;
			this.Tokens = tokens;
		}

		public Address TokensSender { get; }

		public IReadOnlyTaggedTokens<TTokenTagType> Tokens { get; }
	}
}