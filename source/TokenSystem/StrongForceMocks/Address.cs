using System;
using System.Collections;

namespace TokenSystem.StrongForceMocks
{
    public class Address : IComparable<Address>
    {
        private readonly byte[] value;

        public Address(byte[] value)
        {
            this.value = value;
        }

        public Address()
        {
            value = new byte[32];
        }

        public byte[] Value => value;

        public bool Equals(Address other)
        {
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return value != null ? value.GetHashCode() : 0;
        }

        public int CompareTo(Address other)
        {
            if (value.Length != other.Value.Length)
            {
                throw new ArgumentException("Address must be a 32-byte long array.");
            }

            int length = value.Length;
            var comparisonResult = 0;

            for (var i = 0; i < length && comparisonResult == 0; i++)
            {
                comparisonResult = value[i].CompareTo(other.Value[i]);
            }

            return comparisonResult;
        }
    }
}