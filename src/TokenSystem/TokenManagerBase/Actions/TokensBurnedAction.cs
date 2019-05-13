using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManagerBase.Actions
{
	public class TokensBurnedAction : TokenAction
	{
		public TokensBurnedAction(
			string hash,
			Address target,
			IReadOnlyTaggedTokens tokens)
			: base(hash, target, tokens)
		{
		}
	}
}