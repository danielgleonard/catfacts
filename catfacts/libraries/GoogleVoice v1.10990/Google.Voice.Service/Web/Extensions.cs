using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Google.Voice.Web
{
    public static class WebExtensionMethods
    {
        public static string ToString(this HttpMethods method)
        {
            switch (method)
            {
                case HttpMethods.GET: return "GET";
                case HttpMethods.POST: return "POST";
            }

            return "GET";
        }
    }
}
