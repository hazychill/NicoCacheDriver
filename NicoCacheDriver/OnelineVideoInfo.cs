using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;

namespace Hazychill.NicoCacheDriver {
    struct OnelineVideoInfo {
        // ^(?<beforeComment>\s*(?:http://www\.nicovideo\.jp/watch/)?(?<id>(?:[a-z][a-z])?\d+)\s*)(?:;(?<comment>.+))?$
        const string WATCH_PATTERN = "^(?<beforeComment>\\s*(?:http://www\\.nicovideo\\.jp/watch/)?(?<id>(?:[a-z][a-z])?\\d+)\\s*)(?:;(?<comment>.+))?$";

        // ^http://www.nicovideo.jp/mylist/\d+$
        const string MYLIST_PATTERN = "^http://www.nicovideo.jp/mylist/\\d+$";
        string line;

        private OnelineVideoInfo(string line) {
            Contract.Requires(line != null);
            this.line = line;
        }

        public static OnelineVideoInfo FromString(string line) {
            if (line == null) {
                throw new ArgumentNullException("line");
            }
            return new OnelineVideoInfo(line);
        }

        public override string ToString() {
            Contract.Requires(line != null);
            return line;
        }

        public OnelineVideoInfo SetComment(string comment) {
            if (comment == null) {
                throw new ArgumentNullException("comment");
            }
            if (!IsWatchUrl) {
                throw new InvalidOperationException("line is invalid");
            }
            string beforeComment = Regex.Match(line, WATCH_PATTERN).Groups["beforeComment"].Value;
            string newLine = string.Format("{0};{1}", beforeComment, comment);
            return FromString(newLine);
        }

        public string Url {
            get {
                Contract.Requires(line != null);
                return string.Format("http://www.nicovideo.jp/watch/{0}", Id);
            }
        }

        public string Id {
            get {
                Contract.Requires(line != null);
                Match m = Regex.Match(line, WATCH_PATTERN);
                string id;
                if (m.Success) {
                    id = m.Groups["id"].Value;
                }
                else {
                    id = null;
                }

                return id;
            }
        }

        public string Comment {
            get {
                Contract.Requires(line != null);
                Match m = Regex.Match(line, WATCH_PATTERN);
                string comment;
                if (m.Groups["comment"].Success) {
                    comment = m.Groups["comment"].Value;
                }
                else {
                    comment = string.Empty;
                }

                return comment;
            }
        }

        public bool IsWatchUrl {
            get {
                Contract.Requires(line != null);
                return Regex.IsMatch(line, WATCH_PATTERN);
            }
        }

        public bool IsMylistUrl {
            get {
                Contract.Requires(line != null);
                return Regex.IsMatch(line, MYLIST_PATTERN);
            }
        }

        // ^(?:;<(?<name>[^;>]+)>(?<value>[^;]+))*(?:;(?<tail>.*))?$
        private static string pattern = "^(?:;<(?<name>[^;>]+)>(?<value>[^;]+))*(?:;(?<tail>.*))?$";

        public OnelineVideoInfo SetParameters(params Tuple<string, string>[] parameters) {
            Dictionary<string,string> paramMap;
            string tail;
            ExtractParameters(out paramMap, out tail);

            foreach (var item in parameters.Reverse()) {
                var name = item.Item1.ToLower();
                var value = item.Item2;
                paramMap[name] = value;
            }

            var newParamsSec = paramMap.Keys.Select(x => string.Format(";<{0}>{1}", x, paramMap[x]));
            var newParams = string.Join(string.Empty, newParamsSec);

            string beforeComment = Regex.Match(line, WATCH_PATTERN).Groups["beforeComment"].Value;
            string newLine;
            if (tail == null) {
                newLine = string.Format("{0}{1}", beforeComment, newParams);
            }
            else {
                newLine = string.Format("{0}{1};{2}", beforeComment, newParams, tail);
            }

            return OnelineVideoInfo.FromString(newLine);
        }

        private void ExtractParameters(out Dictionary<string, string> paramMap, out string tail) {
            var comment = string.Format(";{0}", this.Comment);
            var m = Regex.Match(comment, pattern);
            var paramsCount = m.Groups["name"].Captures.Count;

            paramMap = new Dictionary<string, string>();
            for (var i = 0; i < paramsCount; i++) {
                var name = m.Groups["name"].Captures[i].Value.ToLower();
                var value = m.Groups["value"].Captures[i].Value;
                if (!paramMap.ContainsKey(name)) {
                    paramMap.Add(name, value);
                }
            }
            tail = m.Groups["tail"].Value;
        }

        public string GetParameter(string name) {
            Dictionary<string,string> paramMap;
            string tail;
            ExtractParameters(out paramMap, out tail);
            string value;
            if (paramMap.TryGetValue(name.ToLower(), out value)) {
                return value;
            }
            else {
                return null;
            }
        }

        public string GetCommentTail() {
            Dictionary<string, string> paramMap;
            string tail;
            ExtractParameters(out paramMap, out tail);
            if (tail == null) tail = string.Empty;
            return tail;
        }

    }
}
