using System;

namespace Google.Voice.Entities
{
    public class Message : ShallowEntity
    {
        public string id { get; set; }
        public string children { get; set; }
        public string displayNumber { get; set; }
        public DateTime displayStartDateTime { get; set; }
        public bool isRead { get; set; }
        public bool isSpam { get; set; }
        public bool isTrash { get; set; }
        public string[] labels { get; set; }
        public string messageText { get; set; }
        public string note { get; set; }
        public string phoneNumber { get; set; }
        public string relativeStartTime { get; set; }
        public bool star { get; set; }
        public long startTime { get; set; }
        public int type { get; set; }
    }
}
