// Copyright (c) Comrade Coop. All rights reserved.

namespace TestUtils
{
	using System.Collections.Generic;
	using ContractsCore;

	public static class AddressTestUtils
	{
		private const int DefaultAddressCount = 20;

		public static Address GenerateRandomAddress()
		{
			var addressFactory = new RandomAddressFactory();
			return addressFactory.Create();
		}

		public static List<Address> GenerateRandomAddresses(int addressCount = DefaultAddressCount)
		{
			var addresses = new List<Address>();

			for (var i = 0; i < addressCount; i++)
			{
				addresses.Add(GenerateRandomAddress());
			}

			return addresses;
		}
	}
}