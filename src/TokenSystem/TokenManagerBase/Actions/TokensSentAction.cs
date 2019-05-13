using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class TokensSentAction : TokenAction
	{
		public TokensSentAction(
			string hash,
			Address target,
			Address tokensReceiver,
			IReadOnlyTaggedTokens tokens)
			: base(hash, target, tokens)
		{
			this.TokensReceiver = tokensReceiver;
		}

		public Address TokensReceiver { get; }
	}
}