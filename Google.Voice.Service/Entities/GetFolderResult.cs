using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Google.Voice.Entities
{
    public class GetFolderResult : ShallowEntity
    {
        public Dictionary<string, Message> messages { get; set; }
        public int totalSize { get; set; }
        public UnreadCounts unreadCounts { get; set; }
        public int resultsPerPage { get; set; }
    }
}
