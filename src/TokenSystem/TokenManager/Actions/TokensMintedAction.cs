using ContractsCore;
using ContractsCore.Actions;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager.Actions
{
	public class TokensMintedAction<TTokenTagType> : TokenAction<TTokenTagType>
	{
		public TokensMintedAction(
			string hash,
			Address target,
			IReadOnlyTaggedTokens<TTokenTagType> tokens)
			: base(hash, target, tokens)
		{
		}
	}
}