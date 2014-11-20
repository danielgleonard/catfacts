using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Google.Voice.Web
{
    public interface IGVRequestResult
    {
        bool RequiresRelogin { get; set; }
    }
}
