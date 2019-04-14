using System.Collections.Generic;
using ContractsCore;

namespace TokenSystem.Tests
{
	public static class AddressTestHelpers
	{
		public const int DefaultAddressCount = 20;

		public static List<Address> GenerateRandomAddresses(int addressCount = DefaultAddressCount)
		{
			var addressFactory = new RandomAddressFactory();
			var generatedAddresses = new List<Address>(addressCount);
			for (var i = 0; i < addressCount; i++)
			{
				generatedAddresses.Add(addressFactory.Create());
			}

			return generatedAddresses;
		}
	}
}