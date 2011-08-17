using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;

namespace Hazychill.NicoCacheDriver {
    struct OnelineVideoInfo {
        // ^(?<beforeComment>\s*(?:http://www\.nicovideo\.jp/watch/)?(?<id>[a-z][a-z]\d+)\s*)(?:;(?<comment>.+))?$
        const string PATTERN = "^(?<beforeComment>\\s*(?:http://www\\.nicovideo\\.jp/watch/)?(?<id>[a-z][a-z]\\d+)\\s*)(?:;(?<comment>.+))?$";
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
            if (!IsValid) {
                throw new InvalidOperationException("line is invalid");
            }
            string beforeComment = Regex.Match(line, PATTERN).Groups["beforeComment"].Value;
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
                Match m = Regex.Match(line, PATTERN);
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
                Match m = Regex.Match(line, PATTERN);
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

        public bool IsValid {
            get {
                Contract.Requires(line != null);
                return Regex.IsMatch(line, PATTERN);
            }
        }
    }
}
