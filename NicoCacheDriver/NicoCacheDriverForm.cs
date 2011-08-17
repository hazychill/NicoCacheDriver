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

namespace Hazychill.NicoCacheDriver {
    public partial class NicoCacheDriverForm : Form {
        private const string NEWLINE = "\r\n";

        bool settingsLoaded;
        string workingUrl;
        string workingTitle;
        SettingsManager smng;
        bool interrapting;
        bool isClosing;
        string settingsFilePath;
        FormWindowState lastWindowState;
        Size lastSize;

        public NicoCacheDriverForm(string settingsFilePath) {
            InitializeComponent();
            settingsLoaded = false;
            isClosing = false;
            this.settingsFilePath = settingsFilePath;
            lastWindowState = this.WindowState;
            lastSize = this.Size;
        }

        #region Event handlers

        private void Form1_Shown(object sender, EventArgs e) {
            foreach (Control c in Controls) {
                c.Enabled = false;
            }
            Text = "NicoCacheDriver (Loading settings...)";
            outputTextBox.AppendText(string.Format("Using : {0}\r\n", GetSettingsFilePath()));
            Action action = LoadSettings;
            action.BeginInvoke(delegate(IAsyncResult result) {
                Action action2 = delegate {
                    if (settingsLoaded) {
                        foreach (Control c in Controls) {
                            c.Enabled = true;
                        }
                        Text = "NicoCacheDriver";
                    }
                    else {
                        Text = "NicoCacheDriver (Loading settings failed!)";
                    }
                    bool timeEnabled;
                    if (!smng.TryGetItem("timeEnabled", out timeEnabled)) {
                        timeEnabled = false;
                    }
                    downloadableTimeEnabled.Checked = timeEnabled;
                    DateTime start;
                    if (smng.TryGetItem("start", out start)) {
                        downloadableTimeStart.Value = start;
                    }
                    DateTime end;
                    if (smng.TryGetItem("end", out end)) {
                        downloadableTimeEnd.Value = end;
                    }
                };
                this.Invoke(action2);
                bool autoStart;
                if (!smng.TryGetItem<bool>("autoStart", out autoStart)) {
                    autoStart = false;
                }
                Action startTimer = delegate {
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
                };
                this.Invoke(startTimer);

                action.EndInvoke(result);
            }, null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            isClosing = true;
            this.Hide();

            smng.RemoveAll<string>("url");
            var lineQuery = queueingUrls.Lines
                .Where(x => Regex.IsMatch(x, "^\\s*(http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+)\\s*$"))
                .Select(x => Regex.Match(x, "^\\s*(http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+)\\s*$").Groups[1].Value);
            foreach (string line in lineQuery) {
                smng.AddItem("url", line);
            }

            pollingTimer.Stop();
            if (downloadWorker.IsBusy) {
                smng.AddItem("url", downloadWorker.WatchUrl);
                SaveSettings();
                downloadWorker.CancelAsync();
            }
            else {
                SaveSettings();
            }
            while (downloadWorker.IsBusy) {
                Thread.Sleep(1 * 1000);
            }
        }

        private void downloadWorker1_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            if (progressBar1.Maximum != e.TotalBytesToReceive) {
                progressBar1.Maximum = (int)e.TotalBytesToReceive;
            }
            progressBar1.Value = (int)e.BytesReceived;
            string id = workingUrl.Replace("http://www.nicovideo.jp/watch/", string.Empty);
            if (e.WillWait > 0) {
                label1.Text = string.Format("{0} (waiting {1}s)", id, e.WillWait / 1000.0);
            }
            else if (e.Title != null) {
                label1.Text = string.Format("{0} ({1}/{2}) {3}", id, e.BytesReceived, e.TotalBytesToReceive, e.Title);
            }
            else {
                label1.Text = string.Format("{0} ({1}/{2})", id, e.BytesReceived, e.TotalBytesToReceive);
            }
            workingTitle = e.Title;
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
                    return currentLines.Concat(Enumerable.Repeat(workingUrl, 1)).ToArray();
                });
                label1.Text = string.Empty;
                progressBar1.Value = 0;
            }
            outputTextBox.AppendText(string.Format("{0}{1}\r\n", msg, workingUrl));
            if (workingTitle != null) {
                outputTextBox.AppendText(string.Format("          {0}\r\n", workingTitle));
            }
            label1.Text = string.Empty;
            progressBar1.Value = 0; ;
            interrapting = false;

            interceptButton.Enabled = false;
            cancelDLButton.Enabled = false;
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
                if (downloadWorker.IsBusy) {
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
                string interraptedUrl = workingUrl;
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

        private void downloadableTimeEnabled_CheckedChanged(object sender, EventArgs e) {
        }

        private void NicoCacheDriverForm_Resize(object sender, EventArgs e) {
            lastWindowState = this.WindowState;
            if (this.WindowState == FormWindowState.Normal) {
                lastSize = this.Size;
            }
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

                this.Invoke(new Action(delegate {
                    Size size;
                    if (smng.TryGetItem("size", out size)) {
                        this.Size = size;
                    }

                    FormWindowState windowState;
                    if (smng.TryGetItem("windowState", out windowState)) {
                        this.WindowState = windowState;
                    }
                }));

                NicoAccessTimer timer = new NicoAccessTimer(smng);


                string userSession = GetUserSession(smng);

                string proxyHost;
                if (!smng.TryGetItem("proxyHost", out proxyHost)) {
                    proxyHost = "localhost";
                }
                int proxyPort;
                if (!smng.TryGetItem("proxyPort", out proxyPort)) {
                    proxyPort = 8080;
                }

                downloadWorker.Setup(userSession, proxyHost, proxyPort, timer);

                Action action = delegate() {
                    WithEditQueueingUrls(delegate(string[] currentLines) {
                        return smng.GetItems<string>("url")
                            .Select(x => x.Value)
                            .ToArray();
                    });
                    outputTextBox.AppendText(string.Format("User session: {0}\r\n", userSession));

                };
                this.Invoke(action);

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
            string getnicovideousersessionfromchromium = smng.GetItem<string>("getnicovideousersessionfromchromium");
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

        private void StartDownload() {
            workingUrl = null;
            string[] lines = WithEditQueueingUrls(delegate(string[] currentLines) {
                List<string> newLines = new List<string>();
                int index = currentLines.Length - 1;
                for (; index >= 0; index--) {
                    string line = currentLines[index];
                    if (IsValidWatchUrl(line)) {
                        workingUrl = line;
                        break;
                    }
                    else {
                        newLines.Insert(0, line);
                    }
                }
                for (int i = 0; i <= index-1; i++) {
                    newLines.Add(currentLines[i]);
                }
                return newLines.ToArray();
            });

            if (workingUrl == null) {
                queueingUrls.ReadOnly = false;
                onlineController.Enabled = true;
                return;
            }

            smng.RemoveAll<string>("url");
            foreach (string line in lines) {
                smng.AddItem("url", line);
            }

            downloadWorker.WatchUrl = workingUrl;
            downloadWorker.DownloadAsync(null);
            label1.Text = workingUrl.Replace("http://www.nicovideo.jp/watch/", string.Empty);
            progressBar1.Value = 0;
            interceptButton.Enabled = true;
        }

        private bool IsValidWatchUrl(string line) {
            // ^\s*(http://www\.nicovideo\.jp/watch/(?:[a-z][a-z])?\d+)\s*$
            return Regex.IsMatch(line, "^http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+$");
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
                smng.SetOrAddNewItem("timeEnabled", downloadableTimeEnabled.Checked);
                smng.SetOrAddNewItem("start", downloadableTimeStart.Value.ToUniversalTime());
                smng.SetOrAddNewItem("end", downloadableTimeEnd.Value.ToUniversalTime());
                smng.SetOrAddNewItem("windowState", lastWindowState);
                smng.SetOrAddNewItem("size", lastSize);
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

        #endregion

    }
}
