
namespace Google.Voice.Entities
{
    public class PhoneNumber : ShallowEntity
    {
        /// <summary>
        /// Raw phone number
        /// </summary>
        public string raw { get; set; }

        /// <summary>
        /// Formatted phone number
        /// </summary>
        public string formatted { get; set; }

        /// <summary>
        /// Flag for "DO-NOT-DISTURB"
        /// </summary>
        public bool dnd { get; set; }

        public string phoneType { get; set; }
    }
}
