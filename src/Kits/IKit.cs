using ContractsCore;

namespace Kits
{
	public interface IKit
	{
		void NewInstance(ContractRegistry registry, IAddressFactory addressFactory, Address originAddress);
	}
}