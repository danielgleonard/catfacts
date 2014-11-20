using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Google.Voice.Web
{
    public class LoginResult : IGVRequestResult
    {
        public bool RequiresRelogin { get; set;  }
    }
}
