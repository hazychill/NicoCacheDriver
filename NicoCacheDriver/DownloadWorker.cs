using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Hazychill.NicoCacheDriver {
    class DownloadWorker : Component {
        private CookieContainer cookies;
        private IWebProxy proxy;
        private AsyncOperation asyncOp;
        private object asyncOpLock = new object();
        private TaskFactory taskFactory;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public event AsyncCompletedEventHandler DownloadCompleted;

        public string WatchUrl { get; set; }
        public NicoAccessTimer Timer { get; set; }
        public bool IsBusy { get; private set; }

        public bool CancellationPending {
            get {
                if (cancellationTokenSource != null) {
                    return cancellationToken.IsCancellationRequested;
                }
                else {
                    return false;
                }
            }
        }

        public DownloadWorker() {
            IsBusy = false;
            taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
            cancellationToken = CancellationToken.None;
        }

        public void Setup(string userSession, string proxyHost, int proxyPort, NicoAccessTimer timer) {
            if (userSession == null) {
                throw new ArgumentNullException("userSession");
            }
            cookies = new CookieContainer();
            Cookie cookie = new Cookie("user_session", userSession, "/", ".nicovideo.jp");
            cookies.Add(cookie);

            if (proxyHost != null && proxyPort > 0) {
                proxy = new WebProxy(proxyHost, proxyPort);
            }
            else {
                proxy = null;
            }

            Timer = timer;
        }

        public bool Download() {
            if (cookies == null) {
                throw new InvalidOperationException("Setup is not called.");
            }

            Contract.Requires(
                cookies.GetCookies(new Uri("http://www.nicovideo.jp/"))
                  .Cast<Cookie>()
                  .Where(x => string.Equals(x.Name, "user_session"))
                  .Any()
            );

            if (WatchUrl == null) {
                throw new InvalidOperationException("WatchUri is null.");
            }
            if (Timer == null) {
                throw new InvalidOperationException("Timer is null.");
            }

            Encoding utf8 = new UTF8Encoding();
            bool completed = false;
            string title = null;

            CheckCancelled();

            int waitMilliseconds = Timer.GetWaitMilliSeconds(WatchUrl);
            if (waitMilliseconds > 0) {
                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(0, 0, 0, waitMilliseconds, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }
                Wait(waitMilliseconds);
            }
            string responseText;
            HttpWebRequest request = WebRequest.Create(WatchUrl) as HttpWebRequest;
            try {
                request.CookieContainer = cookies;
                request.Proxy = proxy;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                using (Stream responseStream = response.GetResponseStream())
                using (TextReader reader = new StreamReader(responseStream, utf8)) {
                    responseText = reader.ReadToEnd();
                }
            }
            finally {
                Timer.UpdateLastAccess(WatchUrl);
            }

            // <div id="watchAPIDataContainer" style="display:none">(?<json>\{.+?\})</div>
            Match titleMatch = Regex.Match(responseText, "<div id=\"watchAPIDataContainer\" style=\"display:none\">(?<json>\\{.+?\\})</div>", RegexOptions.Singleline);
            string jsonTextRaw = titleMatch.Groups["json"].Value;
            var jsonText = string.Empty;
            var json = null as JToken;
            using (var jsonStrReader = new StringReader(string.Format("<json>{0}</json>", jsonTextRaw)))
            using (var jsonXmlReader = XmlReader.Create(jsonStrReader)) {
                while (jsonXmlReader.Read()) {
                    if (jsonXmlReader.NodeType == XmlNodeType.Element) {
                        jsonText = jsonXmlReader.ReadElementString();
                        break;
                    }
                }
            }
            try {
                json = JToken.Parse(jsonText);
                title = json["videoDetail"].Value<string>("title");
                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(0, 0, 0, title, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }

            CheckCancelled();

            string vParam = json["videoDetail"].Value<string>("v");
            // ^nm\d+$
            string videoId = json["videoDetail"].Value<string>("id");
            string as3Param = (Regex.Match(videoId, "^nm\\d+$").Success) ? ("&as3=1") : (string.Empty);
            string getflvUrl = string.Format("http://flapi.nicovideo.jp/api/getflv?v={0}{1}", vParam, as3Param);
            waitMilliseconds = Timer.GetWaitMilliSeconds(getflvUrl);
            if (waitMilliseconds > 0) {
                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(0, 0, 0, waitMilliseconds, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }
                Wait(waitMilliseconds);
            }
            try {
                request = WebRequest.Create(getflvUrl) as HttpWebRequest;
                request.CookieContainer = cookies;
                request.Proxy = proxy;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                using (Stream responseStream = response.GetResponseStream())
                using (TextReader reader = new StreamReader(responseStream, utf8)) {
                    responseText = reader.ReadToEnd();
                }
            }
            finally {
                Timer.UpdateLastAccess(getflvUrl);
            }

            CheckCancelled();

            // &url=([^&]+)&
            string urlRaw = Regex.Match(responseText, "&url=([^&]+)&").Groups[1].Value;
            string url = Uri.UnescapeDataString(urlRaw);
            waitMilliseconds = Timer.GetWaitMilliSeconds(url);
            if (waitMilliseconds > 0) {
                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(0, 0, 0, waitMilliseconds, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }
                Thread.Sleep(waitMilliseconds);
            }
            request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = cookies;
            request.Proxy = proxy;

            HttpWebResponse videoResponse = null;
            try {
                try {
                    videoResponse = request.GetResponse() as HttpWebResponse;
                }
                finally {
                    Timer.UpdateLastAccess(url);
                }
                long contentLength = videoResponse.ContentLength;

                if (contentLength <= 0) {
                    throw new Exception(string.Format("unexpected content length: {0}", contentLength));
                }

                int previousPercentage = -1;
                int percentage = 0;
                long read = 0;

                if ((previousPercentage < percentage)) {
                    previousPercentage = percentage;
                }
                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(percentage, 0, contentLength, title, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }

                CheckCancelled();

                using (Stream responseStream = videoResponse.GetResponseStream()) {
                    int count;
                    byte[] buffer = new byte[4096];
                    while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0) {
                        read += count;
                        percentage = (int)(read * 100 / contentLength);
                        if ((previousPercentage < percentage)) {
                            previousPercentage = percentage;
                        }
                        if (asyncOp != null) {
                            bool canAccess = Timer.CanAccess("urn:uuid:ae64b59c-e55e-473b-9cb5-64ae5ee53b47");
                            if (canAccess) {
                                DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(percentage, read, contentLength, title, asyncOp.UserSuppliedState);
                                asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                                Timer.UpdateLastAccess("urn:uuid:ae64b59c-e55e-473b-9cb5-64ae5ee53b47");
                            }
                        }
                        CheckCancelled();
                    }
                }

                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(percentage, read, contentLength, title, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }

                if (read == contentLength) {
                    completed = true;
                    if ((previousPercentage < percentage)) {
                        previousPercentage = percentage;
                        if (asyncOp != null) {
                            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(percentage, read, contentLength, title, asyncOp.UserSuppliedState);
                            asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                        }
                    }
                }
            }
            finally {
                if (videoResponse != null) {
                    (videoResponse as IDisposable).Dispose();
                }
            }

            return completed;
        }

        private void Wait(int waitMilliseconds) {
            DateTime start = DateTime.Now;
            DateTime now = DateTime.Now;
            while ((now-start).TotalMilliseconds < waitMilliseconds) {
                CheckCancelled();
                Thread.Sleep(100);
                now = DateTime.Now;
            }
        }

        private void CheckCancelled() {
            if (cancellationTokenSource != null) {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public void CancelAsync() {
            if (cancellationTokenSource == null) {
                throw new InvalidOperationException("Async operation is not in progress.");
            }
            cancellationTokenSource.Cancel();
        }

        public void DownloadAsync(object userState) {
            lock (asyncOpLock) {
                if (asyncOp != null) {
                    throw new InvalidOperationException("Another async operation in progress.");
                }
                asyncOp = AsyncOperationManager.CreateOperation(userState);
            }

            Contract.Requires(cancellationTokenSource == null);

            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            IsBusy = true;

            // Is Task.Dispose() required?
            taskFactory.StartNew(DownloadTaskStart, asyncOp);
        }

        private void DownloadTaskStart(object obj) {
            Contract.Requires(obj != null);
            Contract.Requires(obj is AsyncOperation);
            Contract.Requires(cancellationTokenSource != null);
            Contract.Requires(cancellationToken.IsCancellationRequested == false);

            AsyncOperation asyncOp = obj as AsyncOperation;

            Exception error = null;
            bool cancelled = false;

            try {
                bool completed = Download();
                if (completed == false) {
                    Exception incompleteDownloadException = new Exception(string.Format("Incomplete download."));
                    error = incompleteDownloadException;
                }
            }
            catch (OperationCanceledException) {
                cancelled = true;
            }
            catch (Exception e) {
                error = e;
            }
            finally {
                AsyncCompletedEventArgs asyncCompletedEventArgs = new AsyncCompletedEventArgs(error, cancelled, asyncOp.UserSuppliedState);
                asyncOp.PostOperationCompleted(OnDownloadCompleted, asyncCompletedEventArgs);
                this.asyncOp = null;
                IsBusy = false;
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
                cancellationToken = CancellationToken.None;
            }
        }

        private void OnDownloadProgressChanged(object arg) {
            Contract.Requires(arg != null);
            Contract.Requires(arg is DownloadDataCompletedEventArgs);

            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = arg as DownloadProgressChangedEventArgs;
            DownloadProgressChangedEventHandler tempHandler = DownloadProgressChanged;
            if (tempHandler != null) {
                tempHandler(this, downloadProgressChangedEventArgs);
            }
        }

        private void OnDownloadCompleted(object arg) {
            Contract.Requires(arg != null);
            Contract.Requires(arg is AsyncCompletedEventArgs);

            AsyncCompletedEventArgs asyncCompletedEventArgs = arg as AsyncCompletedEventArgs;
            AsyncCompletedEventHandler tempHandler = DownloadCompleted;
            if (tempHandler != null) {
                tempHandler(this, asyncCompletedEventArgs);
            }
        }

        public void SetUserSession(string userSession) {
            var query = cookies.GetCookies(new Uri("http://www.nicovideo.jp/watch/sm9"))
                .Cast<Cookie>()
                .Where(x => string.Equals(x.Name, "user_session"));
            bool cookieExists = false;
            foreach (Cookie cookie in query) {
                cookieExists = true;
                cookie.Value = userSession;
            }
            if (!cookieExists) {
                Cookie cookie = new Cookie("user_session", userSession, "/", ".nicovideo.jp");
                cookies.Add(cookie);
            }
        }
    }
}
