using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class TokensReceivedAction : TokenAction
	{
		public TokensReceivedAction(
			string hash,
			Address target,
			Address tokensSender,
			IReadOnlyTaggedTokens tokens)
			: base(hash, target, tokens)
		{
			this.TokensSender = tokensSender;
		}

		public Address TokensSender { get; }
	}
}