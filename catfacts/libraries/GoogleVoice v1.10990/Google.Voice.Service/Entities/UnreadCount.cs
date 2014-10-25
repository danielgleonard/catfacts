using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;

namespace Google.Voice.Entities
{
    public class UnreadCounts : ShallowEntity
    {
        public int all { get; set; }
        public int inbox { get; set; }
        public int missed { get; set; }
        public int placed { get; set; }
        public int received { get; set; }
        public int recorded { get; set; }
        public int sms { get; set; }
        public int spam { get; set; }
        public int starred { get; set; }
        public int trash { get; set; }
        public int unread { get; set; }
        public int voicemail { get; set; }
    }
}