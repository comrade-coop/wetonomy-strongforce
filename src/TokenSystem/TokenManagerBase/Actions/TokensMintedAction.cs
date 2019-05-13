using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class TokensMintedAction : TokenAction
	{
		public TokensMintedAction(
			string hash,
			Address target,
			IReadOnlyTaggedTokens tokens)
			: base(hash, target, tokens)
		{
		}
	}
}