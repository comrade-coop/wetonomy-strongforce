using ContractsCore;
using System.Collections.Generic;

namespace Members.AddressGroups
{
	public interface IAddressGroup
	{
		bool IsMember(Address address);

		HashSet<Address> GetAllMembers();
	}
}