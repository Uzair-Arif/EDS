using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EDS.Core.Utilities
{
    public static class WebScraping
    {
        public static string GetHeadings(string webAddress)
        {
            try
            {

                if (Uri.IsWellFormedUriString(webAddress, UriKind.Absolute))
                {
                    var response = CallUrl(webAddress).Result;

                    if (response != null)
                    {
                        var headings = ParseHtml(response);
                        return headings;
                    }
                    return string.Empty;

                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            try
            {
                HttpClient client = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                client.DefaultRequestHeaders.Accept.Clear();
                var response = client.GetStringAsync(fullUrl);
                return await response;
            }
            catch (Exception)
            {

                return null;
            }
        }


        private static string ParseHtml(string html)
        {
            try
            {

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);


                var xpath = "//*[self::h1 or self::h2 or self::h3]";

                StringBuilder headings = new StringBuilder();


                foreach (var node in htmlDoc.DocumentNode.SelectNodes(xpath))
                {
                    headings.Append(node.InnerText + ",");

                }

                if (headings.Length > 0)
                {
                    string retHeadings = headings.ToString();
                    retHeadings = retHeadings.Remove(retHeadings.LastIndexOf(','));

                    return retHeadings;
                }
                else
                {
                    return string.Empty;

                }

            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
