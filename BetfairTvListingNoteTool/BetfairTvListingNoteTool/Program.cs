using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace BetfairTvListingNoteTool
{
    public class GZipWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            if (request == null)
            {
                return null;
            }
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string html;
            using (var wc = new GZipWebClient())
            {
                html = wc.DownloadString("http://tvguide.betfair.com/english-uk/tv");
            }
            var htmldocObject = new HtmlDocument();
            htmldocObject.LoadHtml(html);


            var query = from table in htmldocObject.DocumentNode.SelectNodes("//div[@id='listings_content']//table")
                        from row in table.SelectNodes("tr")
                        from cell in row.SelectNodes("th|td")
                        select new { Table = cell.Attributes["class"].Value, CellText = cell.InnerText };

            var events = new List<string>();
            var sports = new List<string>();
            var time = new List<string>();
            var broadcasts = new List<string>();

            foreach (var cell in query)
            {
                var tst = "";
                if (cell.Table != "event")
                {
                    tst =
                        Regex.Replace(
                            cell.CellText.Trim().Replace("\\s+", "").Trim().Replace("\n", string.Empty).Trim(), @"\s+",
                            " ");
                }

                else
                {
                    tst = cell.CellText.Trim();
                }

                if (tst == "Bet&nbspNow")
                {
                    continue;
                }

                switch (cell.Table)
                {
                    case "event":
                        events.Add(tst);
                        break;
                    case "time":
                        time.Add(tst);
                        break;
                    case "sport":
                        sports.Add(tst);
                        break;
                    case "broadcasts":
                        broadcasts.Add(tst);
                        break;
                }
            }

            var tvList = events.Select((t, i) => new TvListing
            {
                Event = events[i], Broadcast = broadcasts[i], Sport = sports[i], TimeStart = time[i]
            }).ToList();

            foreach (var t in tvList.Where(s=> s.Broadcast.Contains("ATR")))
            {
                Console.WriteLine(t.ToString());
            }

            Console.ReadLine();
        }
    }
}
