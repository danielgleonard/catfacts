using System;
using System.Text;
using Google.Voice.Extensions;

namespace Google.Voice.Web
{
    public class WebClient
    {
        internal CookieWebClient InnerWebClient { get; private set; }

        public WebClient()
        {
            InnerWebClient = new CookieWebClient();
        }

        public static string UrlEncodeVariables(VariableCollection variables)
        {
            string output = "";
            int i = 0;

            if (variables != null)
            {
                foreach (string name in variables.AllKeys)
                {
                    if (i > 0) output += "&";
                    var value = Convert.ToString((variables[name]).Value);
                    output += name + "=" + Methods.UrlEncode(value);
                    i++;
                }
            }
            return output;
        }

        public virtual Result Request(string url, HttpMethods method, VariableCollection variables = null)
        {
            variables = variables ?? new VariableCollection();
            InnerWebClient.Encoding = Encoding.UTF8;

            string query = UrlEncodeVariables(variables);

            if (!url.Contains(query))
            {
                if (url.Contains("?"))
                {
                    url += "&" + query;
                }
                else
                {
                    url += "?" + query;
                }
            }

            string result = "";
            string verb = method.ToString();

            if (method == HttpMethods.GET)
            {
                result = InnerWebClient.DownloadString(url);
            }
            else
            {
                result = InnerWebClient.UploadString(url, verb, "");
            }

            return new Result(result);
        }

        public virtual Result Get(string url, VariableCollection variables = null)
        {
            return Request(url, HttpMethods.GET, variables);
        }

        public virtual Result Post(string url, VariableCollection variables = null)
        {
            return Request(url, HttpMethods.POST, variables);
        }

    }
}
