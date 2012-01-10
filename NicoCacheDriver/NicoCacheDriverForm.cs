using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Hazychill.Setting;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;
using NicoCacheDriver;
using System.Threading.Tasks;

namespace Hazychill.NicoCacheDriver {
    public partial class NicoCacheDriverForm : Form {
        private const string NEWLINE = "\r\n";

        bool settingsLoaded;
        OnelineVideoInfo workingUrl;
        SettingsManager smng;
        bool interrapting;
        bool isClosing;
        string settingsFilePath;
        FormWindowState lastWindowState;
        Size lastSize;
        TextWriter logWriter;

        SynchronizationContext uiContext;

        public NicoCacheDriverForm(string settingsFilePath) {
            InitializeComponent();
            settingsLoaded = false;
            isClosing = false;
            this.settingsFilePath = settingsFilePath;
            lastWindowState = this.WindowState;
            lastSize = this.Size;
            logWriter = TextWriter.Null;
            uiContext = SynchronizationContext.Current;
        }

        #region Event handlers

        private void Form1_Shown(object sender, EventArgs e) {
            SetAllControlEnabledStatus(false);
            Text = "NicoCacheDriver (Loading settings...)";
            OutputMessage(string.Format("Using : {0}", GetSettingsFilePath()));

            var taskFactory = new TaskFactory();

            var loadSettingsTask = taskFactory.StartNew(LoadSettings);

            var applySettingsTask = loadSettingsTask.ContinueWith(_ => {
                if (settingsLoaded) {
                    SetAllControlEnabledStatus(true);
                    interceptButton.Enabled = false;
                    cancelDLButton.Enabled = false;
                    Text = "NicoCacheDriver";
                }
                else {
                    Text = "NicoCacheDriver (Loading settings failed!)";
                    splitContainer1.Enabled = true;
                    splitContainer1.Panel2.Enabled = true;
                    exitButton.Enabled = true;
                }

                bool timeEnabled;
                if (!smng.TryGetItem(SettingsConstants.TIME_ENABLED, out timeEnabled)) {
                    timeEnabled = false;
                }
                downloadableTimeEnabled.Checked = timeEnabled;

                DateTime start;
                if (smng.TryGetItem(SettingsConstants.START, out start)) {
                    downloadableTimeStart.Value = start;
                }
                DateTime end;
                if (smng.TryGetItem(SettingsConstants.END, out end)) {
                    downloadableTimeEnd.Value = end;
                }

                string logFile;
                if (smng.TryGetItem(SettingsConstants.LOG_FILE, out logFile)) {
                    logWriter.Dispose();
                    Stream logStream = File.Open(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    logWriter = new StreamWriter(logStream, new UTF8Encoding());
                }

                bool autoStart;
                if (!smng.TryGetItem<bool>(SettingsConstants.AUTO_START, out autoStart)) {
                    autoStart = false;
                }
                if (autoStart) {
                    pollingTimer.Start();
                    onlineController.Checked = true;
                    onlineController.Text = "Online";
                }
                else {
                    onlineController.Checked = false;
                    onlineController.Text = "Offline";
                }

                queueingUrls.Focus();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            isClosing = true;
            this.Hide();

            smng.RemoveAll<string>(SettingsConstants.URL);
            var lineQuery = queueingUrls.Lines
                .SkipWhile(x => string.IsNullOrEmpty(x))
                .Reverse()
                .SkipWhile(x => string.IsNullOrEmpty(x))
                .Reverse();
            foreach (string line in lineQuery) {
                smng.AddItem(SettingsConstants.URL, line);
            }

            pollingTimer.Stop();
            if (downloadWorker.IsBusy) {
                smng.AddItem(SettingsConstants.URL, downloadWorker.WatchUrl);
                SaveSettings();
                downloadWorker.CancelAsync();
            }
            else {
                SaveSettings();
            }
            while (downloadWorker.IsBusy) {
                Thread.Sleep(1 * 1000);
            }

            logWriter.Dispose();
        }

        private void downloadWorker1_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            Contract.Requires(workingUrl.IsValid);

            if (progressBar1.Maximum != e.TotalBytesToReceive) {
                progressBar1.Maximum = (int)e.TotalBytesToReceive;
            }
            progressBar1.Value = (int)e.BytesReceived;
            string id = workingUrl.Id;
            if (e.WillWait > 0) {
                label1.Text = string.Format("{0} (waiting {1}s)", id, e.WillWait / 1000.0);
            }
            else if (e.Title != null) {
                label1.Text = string.Format("{0} ({1}/{2}) {3}", id, e.BytesReceived, e.TotalBytesToReceive, e.Title);
            }
            else {
                label1.Text = string.Format("{0} ({1}/{2})", id, e.BytesReceived, e.TotalBytesToReceive);
            }
            if (string.IsNullOrEmpty(workingUrl.Comment) && e.Title != null) {
                workingUrl = workingUrl.SetComment(e.Title);
            }
        }

        private void downloadWorker1_DownloadCompleted(object sender, AsyncCompletedEventArgs e) {
            if (isClosing) {
                return;
            }
            string msg;
            if (e.Error != null) {
                if (e.Cancelled == true) {
                    msg = "Error     ";
                }
                else {
                    msg = "Error     ";
                }
            }
            else {
                if (e.Cancelled == true) {
                    msg = "Canceled  ";
                }
                else {
                    msg = "Completed ";
                }
            }

            if (e.Cancelled && !interrapting) {
                WithEditQueueingUrls(delegate(string[] currentLines) {
                    return currentLines.Concat(Enumerable.Repeat(workingUrl.ToString(), 1)).ToArray();
                });
                label1.Text = string.Empty;
                progressBar1.Value = 0;
            }
            OutputMessage(string.Format("{0}{1}", msg, workingUrl.Url));
            if (!string.IsNullOrEmpty(workingUrl.Comment)) {
                OutputMessage(string.Format("          {0}", workingUrl.Comment));
            }
            if (e.Error != null) {
                WithEditQueueingUrls(currentLines => {
                    var immediateRetryValue = workingUrl.GetParameter("retry");
                    var queuedRetryValue = workingUrl.GetParameter("retry*");
                    RetryType defaultRetryType;
                    int defaultRetryCount;
                    GetDefaultRetrySetting(smng, out defaultRetryType, out defaultRetryCount);

                    RetryType retryType;
                    int retryCount;

                    if (int.TryParse(immediateRetryValue, out retryCount)) {
                        retryType = RetryType.Immediate;
                        retryCount = Math.Max(0, retryCount);
                    }
                    else if (int.TryParse(queuedRetryValue, out retryCount)) {
                        retryType = RetryType.Queued;
                        retryCount = Math.Max(0, retryCount);
                    }
                    else {
                        retryType = defaultRetryType;
                        retryCount = Math.Max(0, defaultRetryCount);
                    }

                    if (retryType == RetryType.Immediate && retryCount >= 1) {
                        retryCount--;
                        var newUrl = workingUrl.SetParameters(new Tuple<string, string>("retry", retryCount.ToString()));
                        return currentLines
                            .Concat(new string[] { newUrl.ToString() })
                            .ToArray();
                    }

                    if (retryType == RetryType.Queued && retryCount >= 1) {
                        retryCount--;
                        var newUrl = workingUrl.SetParameters(new Tuple<string, string>("retry*", retryCount.ToString()));
                        return new string[] { newUrl.ToString() }
                            .Concat(currentLines)
                            .ToArray();
                    }

                    return new string[] { string.Format(";{0}", workingUrl.ToString()) }
                        .Concat(currentLines)
                        .ToArray();
                });
                OutputLog(e.Error.ToString());
            }
            label1.Text = string.Empty;
            progressBar1.Value = 0; ;
            interrapting = false;

            interceptButton.Enabled = false;
            cancelDLButton.Enabled = false;
            reloadSettingsButton.Enabled = true;
        }

        private void pollingTimer_Tick(object sender, EventArgs e) {
            if (IsInTime()) {
                statusIndicator.BackColor = Color.Lime;
                if (!downloadWorker.IsBusy) {
                    StartDownload();
                }
            }
            else {
                statusIndicator.BackColor = Color.Red;
            }
        }

        private void onlineOfflineButton_Click(object sender, EventArgs e) {
            if (pollingTimer.Enabled) {
                pollingTimer.Stop();
                onlineController.Enabled = true;
                onlineController.Checked = false;
                onlineController.Text = "Offline";
                statusIndicator.BackColor = Color.Gray;
                interceptButton.Enabled = false;
                if (downloadWorker.IsBusy && downloadWorker.CancellationPending == false) {
                    cancelDLButton.Enabled = true;
                }
                else {
                    cancelDLButton.Enabled = false;
                }
            }
            else {
                pollingTimer.Start();
                onlineController.Enabled = true;
                onlineController.Text = "Online";
                onlineController.Checked = true;
                if (downloadWorker.IsBusy) {
                    interceptButton.Enabled = true;
                }
                else {
                    interceptButton.Enabled = false;
                }
                cancelDLButton.Enabled = false;
            }
        }

        private void interceptButton_Click(object sender, EventArgs e) {
            pollingTimer.Stop();

            if (downloadWorker.IsBusy) {
                string interraptedUrl = workingUrl.ToString();
                WithEditQueueingUrls(delegate(string[] currentLines) {
                    if (currentLines.Length >= 1) {
                        List<string> newLines = new List<string>(currentLines);
                        newLines.Insert(newLines.Count-1, interraptedUrl);
                        return newLines.ToArray();
                    }
                    else {
                        return currentLines;
                    }
                });
                interrapting = true;
                downloadWorker.CancelAsync();
            }
            else {
            }

            pollingTimer.Start();
        }

        private void cancelDLButton_Click(object sender, EventArgs e) {
            cancelDLButton.Enabled = false;
            downloadWorker.CancelAsync();
        }

        private void NicoCacheDriverForm_Resize(object sender, EventArgs e) {
            lastWindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal) {
                lastSize = this.Size;
            }
        }

        private void exitButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void reloadSettingsButton_Click(object sender, EventArgs e) {
            bool isOnline = pollingTimer.Enabled;
            if (isOnline) {
                pollingTimer.Stop();
            }
            Dictionary<Control, bool> enabledStateMap = SetAllControlEnabledStatus(false);

            OutputMessage("");
            OutputMessage(string.Format("Using : {0}", GetSettingsFilePath()));

            var taskFactory = new TaskFactory();

            var loadSettingsTask = taskFactory.StartNew(() => {
                SettingsManager newSmng = new SettingsManager();
                newSmng.ConverterMap.Add(typeof(Regex),
                                      new FlexibleConverter<Regex>(regex => regex.ToString(),
                                                                   str => new Regex(str)));
                settingsLoaded = false;
                string settingsFilePath = GetSettingsFilePath();
                newSmng.Load(settingsFilePath);

                string getnicovideousersessionfromchromium = newSmng.GetItem<string>(SettingsConstants.GETNICOVIDEOUSERSESSIONFROMCHROMIUM);
                smng.SetOrAddNewItem(SettingsConstants.GETNICOVIDEOUSERSESSIONFROMCHROMIUM, getnicovideousersessionfromchromium);

                foreach (SettingsItem<string> timerNameItem in smng.GetItems<string>(SettingsConstants.TIMER_NAME)) {
                    string timerName = timerNameItem.Value;
                    string timerIntervalName = string.Format("{0}_{1}", SettingsConstants.TIMER_INTERVAL_PREFIX, timerName);
                    smng.RemoveAll<TimeSpan>(timerIntervalName);
                    string timerPatternName = string.Format("{0}_{1}", SettingsConstants.TIMER_PATTERN_PREFIX, timerName);
                    smng.RemoveAll<Regex>(timerPatternName);
                }
                smng.RemoveAll<string>(SettingsConstants.TIMER_NAME);

                foreach (string timerName in newSmng.GetItems<string>(SettingsConstants.TIMER_NAME)) {
                    smng.AddItem(SettingsConstants.TIMER_NAME, timerName);
                    string timerIntervalName = string.Format("{0}_{1}", SettingsConstants.TIMER_INTERVAL_PREFIX, timerName);
                    TimeSpan timerInterval = newSmng.GetItem<TimeSpan>(timerIntervalName);
                    smng.AddItem(timerIntervalName, timerInterval);
                    string timerPatternName = string.Format("{0}_{1}", SettingsConstants.TIMER_PATTERN_PREFIX, timerName);
                    Regex timerPattern = newSmng.GetItem<Regex>(timerPatternName);
                    smng.AddItem(timerPatternName, timerPattern);
                }

                string userSession = GetUserSession(smng);
                downloadWorker.SetUserSession(userSession);

                NicoAccessTimer timer = downloadWorker.Timer;
                NicoAccessTimer newTimer = timer.DeriveNewTimer(smng);
                downloadWorker.Timer = newTimer;

                return userSession;
            });

            var applySettingsTask = loadSettingsTask.ContinueWith(precedentTask => {
                try {
                    // only for exception propagation
                    precedentTask.Wait();

                    var userSession = precedentTask.Result;
                    OutputMessage(string.Format("User session: {0}", userSession));
                    RestoreAllControlEnabledStatus(enabledStateMap);
                    if (isOnline) {
                        pollingTimer.Start();
                    }
                    settingsLoaded = true;
                }
                catch (AggregateException exception) {
                    Exception exp = exception;
                    if (exception.InnerExceptions.Count == 1) {
                        exp = exception.InnerExceptions[0];
                    }
                    onlineController.Text = "Offline";
                    onlineController.Checked = false;
                    statusIndicator.BackColor = Color.Gray;
                    splitContainer1.Enabled = true;
                    splitContainer1.Panel2.Enabled = true;
                    exitButton.Enabled = true;
                    outputTextBox.Enabled = true;
                    MessageBox.Show(exp.ToString());
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void queueingUrls_UpdateTimerEvent(object sender, EventArgs e) {
            NicoAccessTimer timer = downloadWorker.Timer;
            timer.UpdateLastAccess("urn:uuid:45755c6f-01e2-4a38-6f5c-7545e201384a");
        }

        private void clearButton_Click(object sender, EventArgs e) {
            outputTextBox.Clear();
        }

        #endregion

        #region Private methods

        private void LoadSettings() {
            try {
                string settingsFilePath = GetSettingsFilePath();
                smng = new SettingsManager();
                smng.ConverterMap.Add(typeof(Regex),
                                      new FlexibleConverter<Regex>(regex => regex.ToString(),
                                                                   str => new Regex(str)));
                smng.Load(settingsFilePath);

                uiContext.Send(_ => {
                    Size size;
                    if (smng.TryGetItem(SettingsConstants.SIZE, out size)) {
                        this.Size = size;
                    }

                    FormWindowState windowState;
                    if (smng.TryGetItem(SettingsConstants.WINDOW_STATE, out windowState)) {
                        this.WindowState = windowState;
                    }
                }, null);

                NicoAccessTimer timer = new NicoAccessTimer(smng);


                string userSession = GetUserSession(smng);

                string proxyHost;
                if (!smng.TryGetItem(SettingsConstants.PROXY_HOST, out proxyHost)) {
                    proxyHost = "localhost";
                }
                int proxyPort;
                if (!smng.TryGetItem(SettingsConstants.PROXY_PORT, out proxyPort)) {
                    proxyPort = 8080;
                }

                downloadWorker.Setup(userSession, proxyHost, proxyPort, timer);

                uiContext.Send(_ => {
                    WithEditQueueingUrls(delegate(string[] currentLines) {
                        return smng.GetItems<string>(SettingsConstants.URL)
                            .Select(x => x.Value)
                            .ToArray();
                    });
                    OutputMessage(string.Format("User session: {0}", userSession));
                }, null);

                settingsLoaded = true;
            }
            catch (Exception e) {
                MessageBox.Show(e.ToString());
            }
        }

        private string GetSettingsFilePath() {
            if (settingsFilePath != null) {
                return settingsFilePath;
            }
            else {
                Assembly myAssembly = Assembly.GetEntryAssembly();
                string path = myAssembly.Location;
                string execDir = Path.GetDirectoryName(path);
                string defaultSettingsFilePath = Path.Combine(execDir, "settings.txt");
                return defaultSettingsFilePath;
            }
        }

        private string GetUserSession(SettingsManager smng) {
            string userSession;
            string getnicovideousersessionfromchromium = smng.GetItem<string>(SettingsConstants.GETNICOVIDEOUSERSESSIONFROMCHROMIUM);

            if (!File.Exists(getnicovideousersessionfromchromium)) {
                throw new FileNotFoundException(string.Format("Not found \"{0}\"", getnicovideousersessionfromchromium));
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(getnicovideousersessionfromchromium);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            using (Process process = new Process()) {
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                using (StreamReader reader = process.StandardOutput) {
                    userSession = reader.ReadLine();
                }
            }
            return userSession;
        }

        private void GetDefaultRetrySetting(SettingsManager smng, out RetryType defaultRetryType, out int defaultRetryCount) {
            if (!smng.TryGetItem(SettingsConstants.DEFAULT_RETRY_TYPE, out defaultRetryType)) {
                defaultRetryType = RetryType.None;
            }
            if (!smng.TryGetItem(SettingsConstants.DEFAULT_RETRY_COUNT, out defaultRetryCount)) {
                defaultRetryCount = 0;
            }
        }

        private void StartDownload() {
            string nextUrl = null;

            if (!downloadWorker.Timer.CanAccess("urn:uuid:45755c6f-01e2-4a38-6f5c-7545e201384a")) {
                return;
            }

            string[] lines = WithEditQueueingUrls(delegate(string[] currentLines) {
                List<OnelineVideoInfo> newLines = new List<OnelineVideoInfo>();

                var query = currentLines
                    .SkipWhile(x => string.IsNullOrEmpty(x))
                    .Reverse()
                    .SkipWhile(x => string.IsNullOrEmpty(x))
                    .Select(x => OnelineVideoInfo.FromString(x));

                int index = 0;
                foreach (var line in query) {
                    if (nextUrl == null) {
                        if (line.IsValid) {
                            nextUrl = line.ToString();
                        }
                        else {
                            index++;
                            newLines.Insert(0, line);
                        }
                    }
                    else {
                        newLines.Insert(index, line);
                    }
                }

                return newLines
                    .Select(x => x.ToString())
                    .ToArray();
            });

            if (nextUrl != null) {
                workingUrl = OnelineVideoInfo.FromString(nextUrl);
            }
            else {
                queueingUrls.ReadOnly = false;
                onlineController.Enabled = true;
                return;
            }

            smng.RemoveAll<string>(SettingsConstants.URL);
            foreach (string line in lines) {
                smng.AddItem(SettingsConstants.URL, line);
            }

            reloadSettingsButton.Enabled = false;
            downloadWorker.WatchUrl = workingUrl.Url;
            downloadWorker.DownloadAsync(null);
            label1.Text = workingUrl.Id;
            progressBar1.Value = 0;
            interceptButton.Enabled = true;
        }

        private bool IsInTime() {
            bool isInTime;
            if (downloadableTimeEnabled.Checked == false) {
                isInTime = true;
            }
            else {
                string startTime = downloadableTimeStart.Value.ToString("HHmmss");
                string endTime = downloadableTimeEnd.Value.ToString("HHmmss");
                string nowTime = DateTime.Now.ToString("HHmmss");
                if (startTime.CompareTo(endTime) < 0) {
                    isInTime = ((startTime.CompareTo(nowTime) < 0) && (nowTime.CompareTo(endTime) < 0));
                }
                else {
                    isInTime = (((startTime.CompareTo(nowTime)  < 0) && (nowTime.CompareTo("24:00:00") < 0)) || 
                                (("00:00:00".CompareTo(nowTime) < 0) && (nowTime.CompareTo(endTime)    < 0)));
                }
            }

            return isInTime;
        }

        private void SaveSettings() {
            try {
                smng.SetOrAddNewItem(SettingsConstants.TIME_ENABLED, downloadableTimeEnabled.Checked);
                smng.SetOrAddNewItem(SettingsConstants.START, downloadableTimeStart.Value.ToUniversalTime());
                smng.SetOrAddNewItem(SettingsConstants.END, downloadableTimeEnd.Value.ToUniversalTime());
                smng.SetOrAddNewItem(SettingsConstants.WINDOW_STATE, lastWindowState);
                smng.SetOrAddNewItem(SettingsConstants.SIZE, lastSize);
                string tempPathForNewSettngsFile = Path.GetTempFileName();
                string tempPathForOldSettngsFile = Path.GetTempFileName();
                File.Delete(tempPathForNewSettngsFile);
                File.Delete(tempPathForOldSettngsFile);
                string settingsFilePath = GetSettingsFilePath();
                smng.Save(tempPathForNewSettngsFile, true);
                File.Move(settingsFilePath, tempPathForOldSettngsFile);
                File.Move(tempPathForNewSettngsFile, settingsFilePath);
            }
            catch (Exception e) {
                pollingTimer.Stop();
                label1.Enabled = false;
                queueingUrls.Enabled = false;
                onlineController.Enabled = false;
                interceptButton.Enabled = false;
                MessageBox.Show("Failed to save settings file");
                MessageBox.Show(e.ToString());
            }
        }

        private string[] WithEditQueueingUrls(Func<string[], string[]> editMethod) {
            //queueingUrls.Enabled = false;
            string textBefore = queueingUrls.Text;
            string[] currentLines = queueingUrls.Lines
                .Reverse()
                .SkipWhile(x => string.IsNullOrEmpty(x))
                .Reverse()
                .ToArray();
            string[] newLines = editMethod(currentLines);
            string textAfter = string.Format("{0}{1}", string.Join(NEWLINE, newLines), NEWLINE);
            if (textAfter == "\r\n") {
                textAfter = string.Empty;
            }
            if (!string.Equals(textBefore, textAfter, StringComparison.Ordinal)) {
                queueingUrls.Text = textAfter;
            }
            //queueingUrls.Enabled = true;
            return newLines;
        }

        private Dictionary<Control, bool> SetAllControlEnabledStatus(bool enabled) {
            ISet<Control> processedControls = new HashSet<Control>();
            Queue<Control> queue = new Queue<Control>();
            Dictionary<Control, bool> currentEnabledStateMap = new Dictionary<Control, bool>();
            processedControls.Add(this);
            queue.Enqueue(this);
            while (queue.Count > 0) {
                Control c = queue.Dequeue();
                foreach (Control child in c.Controls) {
                    if (processedControls.Add(child)) {
                        currentEnabledStateMap.Add(child, child.Enabled);
                        queue.Enqueue(child);
                    }
                }
            }

            processedControls = new HashSet<Control>();
            queue = new Queue<Control>();
            processedControls.Add(this);
            queue.Enqueue(this);
            while (queue.Count > 0) {
                Control c = queue.Dequeue();
                foreach (Control child in c.Controls) {
                    if (processedControls.Add(child)) {
                        child.Enabled = enabled;
                        queue.Enqueue(child);
                    }
                }
            }

            return currentEnabledStateMap;
        }

        private void RestoreAllControlEnabledStatus(Dictionary<Control, bool> enabledStateMap) {
            var query = enabledStateMap.Keys
                .Where(c => c != null)
                .Where(c => !c.IsDisposed);
            foreach (Control c in query) {
                bool enabled = enabledStateMap[c];
                c.Enabled = enabled;
            }
        }

        private void OutputMessage(string message) {
            outputTextBox.AppendText(message);
            outputTextBox.AppendText(NEWLINE);
            logWriter.WriteLine("{0} {1}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"), message);
            logWriter.Flush();
        }

        private void OutputLog(string message) {
            logWriter.WriteLine("{0} {1}", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"), message);
            logWriter.Flush();
        }

        #endregion

    }
}
