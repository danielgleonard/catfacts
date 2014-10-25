using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Google.Voice.Extensions;

namespace Google.Voice
{
    public class Result
    {
        public Result(string result)
        {
            Content = result;
        }

        public string Content { get; private set; }

        public string GetJsonFromXml()
        {
            var node = XML().SelectSingleNode("/response/json");
            string json = node.InnerText;
            return json;
        }

        public T GetJsonObjectFromXml<T>()
        {
            var node = XML().SelectSingleNode("/response/json");
            string json = node.InnerText;
            return json.ToObject<T>();
        }

        public override string ToString()
        {
            return Content;
        }

        public string GetInputValueByRegExLiberal(string inputName)
        {
            var regx = "(input).*?(name)(=)(\"" + inputName + "\").*?(value)(=)(\".*?\")";

            Regex r = new Regex(regx, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match m = r.Match(Content);

            if (m.Success)
            {
                return m.Groups[7].Value.Replace("\"", "");
            }

            return null;
        }

        public string GetInputValueByRegEx(string inputName)
        {
            var path = @"//input[@name='" + inputName + "']";
            ///var regx = "name=\"" + inputName + "\"\\s+value=\"(.+)\"";
            var regx = "name=\"" + inputName + "\"\\s*.*\\s*value=\"(.+)\"";

            try
            {
                var matches = Regex.Match(Content, regx).Groups;
                var value = matches[1].Value;
                return value;
            }
            catch { }

            return null;
        }

        public string GetBlock(string startFlag = "&&&START&&&", string endFlag = "&&&END&&&")
        {
            int start = Content.IndexOf(startFlag) + startFlag.Length;
            int length = Content.Substring(start).IndexOf(endFlag);

            try
            {
                var content = Content.Substring(start, length);
                return content;
            }
            catch { }

            return null;
        }

        public Queue<string> GetBlocks(string startFlag, bool keepStart = false,
            string startAt = null, string stopAt = null)
        {
            Queue<string> blocks = new Queue<string>();


            string contentLeft;
            if (!string.IsNullOrEmpty(startAt))
            {
                contentLeft = Content.Substring(Content.IndexOf(startAt));
            }
            else
            {
                contentLeft = Content;
            }

            if (!string.IsNullOrEmpty(stopAt))
            {
                contentLeft = contentLeft.Substring(0, contentLeft.IndexOf(stopAt));
            }

            string[] split = new[] { startFlag };
            string[] allBlocks = contentLeft.Split(split, StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in allBlocks)
            {
                var currentBlock = startFlag + block;

                int absStart = currentBlock.IndexOf(startFlag);
                int start = absStart + (keepStart ? 0 : startFlag.Length);

                if (start >= 0)
                {
                    try
                    {
                        var content = currentBlock.Substring(start);
                        blocks.Enqueue(content);
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            return blocks;
        }

        public string GetInputValueByHtml(string inputName)
        {
            var path = @"//input[@name='" + inputName + "']";

            try
            {
                var nodes = HTML().DocumentNode.SelectNodes(path);
                foreach (var node in nodes)
                {
                    if (node.Attributes["value"] != null)
                    {
                        var value = node.Attributes["value"].Value;
                        return value;
                    }
                }
            }
            catch { }

            return null;
        }

        public XmlDocument XML()
        {
                if (xml == null)
                {
                    xml = new XmlDocument();
                    xml.LoadXml(Content);
                }
                return xml;
        }
        private XmlDocument xml;

        public HtmlAgilityPack.HtmlDocument HTML()
        {
                if (html == null)
                {
                    html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(Content);
                }
                return html;
        }
        private HtmlAgilityPack.HtmlDocument html;

        public static implicit operator string(Result response)
        {
            return response.Content;
        }
    }
}
