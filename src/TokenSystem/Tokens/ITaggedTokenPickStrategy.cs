namespace TokenSystem.Tokens
{
	public interface ITaggedTokenPickStrategy
	{
		TaggedTokens Pick(TaggedTokens tokens, decimal amount);
	}
}