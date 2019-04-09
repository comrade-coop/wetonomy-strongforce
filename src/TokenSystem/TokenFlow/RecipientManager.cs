using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.TokenFlow
{
    public class RecipientManager
    {
        private readonly IList<Address> recipients;

        public RecipientManager() : this(new List<Address>())
        {
        }

        public RecipientManager(IList<Address> recipients)
        {
            this.recipients = recipients;
        }

        public void AddRecipient(Address recipient)
        {
            if (this.recipients.Contains(recipient))
            {
                return;
            }

            this.recipients.Add(recipient);
        }

        public bool RemoveRecipient(Address recipient)
        {
            return this.recipients.Remove(recipient);
        }

        public IList<Address> Recipients => this.recipients;
    }
}