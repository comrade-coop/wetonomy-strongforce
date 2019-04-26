using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokensBurnedAction<TTokenTagType> : TokenAction<TTokenTagType>
	{
		public TokensBurnedAction(
			string hash,
			Address target,
			IReadOnlyTaggedTokens<TTokenTagType> tokens)
			: base(hash, target, tokens)
		{
		}
	}
}