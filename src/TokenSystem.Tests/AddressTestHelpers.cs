using System.Collections.Generic;
using ContractsCore;

namespace TokenSystem.Tests
{
    public static class AddressTestHelpers
    {
        public static List<Address> GenerateRandomAddresses(int count)
        {
            var addressFactory = new RandomAddressFactory();
            var generatedAddresses = new List<Address>(count);
            for (var i = 0; i < count; i++)
            {
                generatedAddresses.Add(addressFactory.Create());
            }

            return generatedAddresses;
        }
    }
}