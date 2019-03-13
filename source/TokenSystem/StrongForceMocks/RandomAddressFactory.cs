using System;

namespace TokenSystem.StrongForceMocks
{
    public class RandomAddressFactory : IAddressFactory
    {
        private readonly Random random;

        public RandomAddressFactory()
        {
            random = new Random();
        }

        public Address Create()
        {
            byte[] addressBuffer = new byte[32];
            random.NextBytes(addressBuffer);
            var address = new Address(addressBuffer);
            return address;
        }
    }
}