
namespace Google.Voice.Entities
{
    public class Contact : ShallowEntity
    {
        public string contactId { get; set; }
        public string name { get; set; }
        public string photoUrl { get; set; }
        public string phoneNumber { get; set; }
        public string phoneTypeName { get; set; }

        public bool hasSpokenName { get; set; }
        public bool hasCustomForwarding { get; set; }

        public string displayNumber { get; set; }

        public int rankp { get; set; }
        public int rankc { get; set; }
        public int response { get; set; }

        public string[] emails { get; set; }
        public string[] cEmails { get; set; }
        public PhoneNumber[] numbers { get; set; }
        public override string ToString()
        {
            return name + " - " + displayNumber;
        }
    }
}
