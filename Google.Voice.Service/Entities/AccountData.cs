using System.Collections.Generic;

namespace Google.Voice.Entities
{
    public class AccountData : ShallowEntity
    {
        public PhoneNumber number { get; set; }
        public Dictionary<string, Contact> contacts { get; set; }
        public Dictionary<string, ForwardingPhone> phones { get; set; }
        public string[] rankedContacts { get; set; }
        public string[] rank { get; set; }
    }
}
