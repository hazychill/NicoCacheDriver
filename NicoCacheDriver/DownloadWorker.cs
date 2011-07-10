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

namespace Hazychill.NicoCacheDriver {
    class DownloadWorker : Component {
        private CookieContainer cookies;
        private IWebProxy proxy;
        private AsyncOperation asyncOp;
        private object asyncOpLock = new object();
        private bool cancellationPending;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public event AsyncCompletedEventHandler DownloadCompleted;

        public string WatchUrl { get; set; }
        public NicoAccessTimer Timer { get; private set; }
        public bool IsBusy { get; private set; }

        public DownloadWorker() {
            IsBusy = false;
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

            CheckCancelled();

            int waitMilliseconds = Timer.WaitBeforeAccess(WatchUrl);
            if (waitMilliseconds > 0) {
                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(0, 0, 0, waitMilliseconds, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }
                Thread.Sleep(waitMilliseconds);
            }
            HttpWebRequest request = WebRequest.Create(WatchUrl) as HttpWebRequest;
            request.CookieContainer = cookies;
            request.Proxy = proxy;
            string responseText;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (Stream responseStream = response.GetResponseStream())
            using (TextReader reader = new StreamReader(responseStream, utf8)) {
                responseText = reader.ReadToEnd();
            }

            CheckCancelled();

            // addVideoToDeflist\((\d+),
            string vParam = Regex.Match(responseText, "addVideoToDeflist\\((\\d+),").Groups[1].Value;
            string getflvUrl = string.Format("http://flapi.nicovideo.jp/api/getflv?v={0}", vParam);
            waitMilliseconds = Timer.WaitBeforeAccess(getflvUrl);
            if (waitMilliseconds > 0) {
                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(0, 0, 0, waitMilliseconds, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }
                Thread.Sleep(waitMilliseconds);
            }
            request = WebRequest.Create(getflvUrl) as HttpWebRequest;
            request.CookieContainer = cookies;
            request.Proxy = proxy;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (Stream responseStream = response.GetResponseStream())
            using (TextReader reader = new StreamReader(responseStream, utf8)) {
                responseText = reader.ReadToEnd();
            }

            CheckCancelled();

            // &url=([^&]+)&
            string urlRaw = Regex.Match(responseText, "&url=([^&]+)&").Groups[1].Value;
            string url = Uri.UnescapeDataString(urlRaw);
            waitMilliseconds = Timer.WaitBeforeAccess(url);
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
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
                long contentLength = response.ContentLength;
                int previousPercentage = -1;
                int percentage = 0;
                long read = 0;

                if ((previousPercentage < percentage)) {
                    previousPercentage = percentage;
                }
                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(percentage, 0, contentLength, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }

                CheckCancelled();

                using (Stream responseStream = response.GetResponseStream()) {
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
                                DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(percentage, read, contentLength, asyncOp.UserSuppliedState);
                                asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                            }
                        }
                        CheckCancelled();
                    }
                }

                if (asyncOp != null) {
                    DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(percentage, read, contentLength, asyncOp.UserSuppliedState);
                    asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                }

                if (read == contentLength) {
                    completed = true;
                    if ((previousPercentage < percentage)) {
                        previousPercentage = percentage;
                        if (asyncOp != null) {
                            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = new DownloadProgressChangedEventArgs(percentage, read, contentLength, asyncOp.UserSuppliedState);
                            asyncOp.Post(OnDownloadProgressChanged, downloadProgressChangedEventArgs);
                        }
                    }
                }
            }

            return completed;
        }

        private void CheckCancelled() {
            if (cancellationPending) {
                throw new CancelException();
            }
        }

        public void CancelAsync() {
            cancellationPending = true;
        }

        public void DownloadAsync(object userState) {
            lock (asyncOpLock) {
                if (asyncOp != null) {
                    throw new InvalidOperationException("Another async operation in progress.");
                }
                asyncOp = AsyncOperationManager.CreateOperation(userState);
            }

            cancellationPending = false;
            IsBusy = true;

            ParameterizedThreadStart start = DownloadThreadStart;
            Thread thread = new Thread(start);
            thread.Start(asyncOp);

        }

        private void DownloadThreadStart(object obj) {
            Contract.Requires(obj != null);
            Contract.Requires(obj is AsyncOperation);

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
            catch (CancelException) {
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
    }
}
