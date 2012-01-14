using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace NicoCacheDriver {
    class MyListExtractor {
        internal IEnumerable<string> ExtractWatchUrls(string mylistUrl) {
            try {
                var request = WebRequest.Create(mylistUrl) as HttpWebRequest;
                string html;
                using (var response = request.GetResponse() as HttpWebResponse)
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, new UTF8Encoding())) {
                    html = reader.ReadToEnd();
                }
                // "watch_id":"(?<watch_id>[^"]+)"
                return Regex.Matches(html, "\"watch_id\":\"(?<watch_id>[^\"]+)\"")
                    .Cast<Match>()
                    .Select(m => m.Groups["watch_id"].Value)
                    .OrderByDescending(x => long.Parse(Regex.Match(x, "\\d+").Groups[0].Value))
                    .Select(x => string.Format("http://www.nicovideo.jp/watch/{0}", x));
            }
            catch (Exception e) {
                return new string[] { string.Format(";{0}", mylistUrl) };
            }
        }
    }
}
