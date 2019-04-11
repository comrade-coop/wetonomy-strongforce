using System.Collections.Generic;
using TokenSystem.StrongForceMocks;
using TokenSystem.TokenManager;

namespace TokenSystem.TokenFlow
{
	public class TokenSplitter<TTokenTagType> : RecipientManager
	{
		private readonly TokenManager<TTokenTagType> tokenManager;
		private readonly ISplitStrategy<TTokenTagType> splitStrategy;

		public TokenSplitter(TokenManager<TTokenTagType> tokenManager, ISplitStrategy<TTokenTagType> splitStrategy) :
			this(new List<Address>(), tokenManager, splitStrategy)
		{
		}

		public TokenSplitter(
			IList<Address> recipients,
			TokenManager<TTokenTagType> tokenManager,
			ISplitStrategy<TTokenTagType> splitStrategy)
			: base(recipients)
		{
			this.tokenManager = tokenManager;
			this.splitStrategy = splitStrategy;
		}

		protected void Split()
		{
			this.splitStrategy.Split(this.Recipients, this.tokenManager);
		}
	}
}