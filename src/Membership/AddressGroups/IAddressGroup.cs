using System.Collections.Generic;
using ContractsCore;

namespace Membership.AddressGroups
{
	public interface IAddressGroup
	{
		bool IsMember(Address address);

		IEnumerable<Address> GetAllMembers();
	}
}