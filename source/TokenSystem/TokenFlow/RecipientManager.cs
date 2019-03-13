using System.Collections.Generic;
using TokenSystem.StrongForceMocks;

namespace TokenSystem.TokenFlow
{
    public class RecipientManager
    {
        private readonly List<Address> recipients;

        public RecipientManager() : this(new List<Address>())
        {
        }

        public RecipientManager(List<Address> recipients)
        {
            this.recipients = recipients;
        }

        public void AddRecipient(Address recipient)
        {
            if (recipients.Contains(recipient))
            {
                return;
            }

            recipients.Add(recipient);
        }

        public bool RemoveRecipient(Address recipient)
        {
            return recipients.Remove(recipient);
        }

        public List<Address> Recipients => recipients;
    }
}