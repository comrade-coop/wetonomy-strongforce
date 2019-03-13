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

        public byte[] Value => value;

        public bool Equals(Address other)
        {
            return Equals(value, other.value);
        }

        public override int GetHashCode()
        {
            return value != null ? value.GetHashCode() : 0;
        }

        public int CompareTo(Address other)
        {
            if (value.Length != other.Value.Length)
            {
                throw new ArgumentException("Arrays must be of equal length.");
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