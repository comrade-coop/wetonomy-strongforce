using TokenSystem.StrongForceMocks;

namespace TokenSystem.Tokens
{
	public interface ITokenTagger<TTagType>
	{
		TaggedTokens<TTagType> Tag(Address owner, decimal amount);
	}
}