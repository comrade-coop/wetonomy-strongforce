using System.Numerics;
using ContractsCore;
using TokenSystem.Tokens;

namespace TokenSystem.TokenManager
{
	public interface ITokenTagger<TTagType>
	{
		IReadOnlyTaggedTokens<TTagType> Tag(Address owner, BigInteger amount);
	}
}