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
    public partial class nicoCacheDriverForm : Form {
        bool settingsLoaded;
        string workingUrl;
        SettingsManager smng;

        public nicoCacheDriverForm() {
            InitializeComponent();
            settingsLoaded = false;
        }

        private void LoadSettings() {
            try {
                string settingsFilePath = GetSettingsFilePath();
                smng = new SettingsManager();
                smng.ConverterMap.Add(typeof(Regex),
                                      new FlexibleConverter<Regex>(regex => regex.ToString(),
                                                                   str => new Regex(str)));
                smng.Load(settingsFilePath);

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
                    queueingUrls.Text = string.Join("\r\n", smng.GetItems<string>("url"));
                };
                this.Invoke(action);

                settingsLoaded = true;
            }
            catch (Exception e) {
                MessageBox.Show(e.ToString());
            }
        }

        private static string GetSettingsFilePath() {
            Assembly myAssembly = Assembly.GetEntryAssembly();
            string path = myAssembly.Location;
            string execDir = Path.GetDirectoryName(path);
            string settingsFilePath = Path.Combine(execDir, "settings.txt");
            return settingsFilePath;
        }

        private static string GetUserSession(SettingsManager smng) {
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

        private void Form1_Shown(object sender, EventArgs e) {
            foreach (Control c in Controls) {
                c.Enabled = false;
            }
            Text = "NicoCacheDriver (Loading settings...)";
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
                    if (timeEnabled) {
                        downloadableTimeStart.Enabled = true;
                        downloadableTimeEnd.Enabled = true;
                    }
                    else {
                        downloadableTimeStart.Enabled = false;
                        downloadableTimeEnd.Enabled = false;
                    }
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
                if (autoStart) {
                    Action startTimer = delegate {
                        pollingTimer.Start();
                        startButton.Text = "Stop";
                        label1.Enabled = true;
                        progressBar1.Enabled = true;
                    };
                    this.Invoke(startTimer);
                }

                action.EndInvoke(result);
            }, null);
        }

        private void downloadWorker1_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            if (progressBar1.Maximum != e.TotalBytesToReceive) {
                progressBar1.Maximum = (int)e.TotalBytesToReceive;
            }
            progressBar1.Value = (int)e.BytesReceived;
            if (e.WillWait > 0) {
                label1.Text = string.Format("{0} (waiting {1}s)", workingUrl, e.WillWait / 1000.0);
            }
            else {
                label1.Text = string.Format("{0} ({1}/{2})", workingUrl, e.BytesReceived, e.TotalBytesToReceive);
            }
        }

        private void downloadWorker1_DownloadCompleted(object sender, AsyncCompletedEventArgs e) {
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

            if (e.Cancelled) {
                queueingUrls.AppendText(string.Format("{0}\r\n", workingUrl));
            }
            outputTextBox.AppendText(string.Format("{0}{1}\r\n", msg, workingUrl));
        }

        private void button1_Click(object sender, EventArgs e) {
            if (pollingTimer.Enabled == false) {
                startButton.Text = "Stop";
                label1.Enabled = true;
                progressBar1.Enabled = true;
                pollingTimer.Start();
            }
            else {
                startButton.Text = "Start";
                label1.Enabled = false;
                progressBar1.Enabled = false;
                pollingTimer.Stop();
                downloadWorker.CancelAsync();
                statusIndicator.BackColor = Color.Gray;
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (!downloadWorker.IsBusy) {
                StartDownload();
            }
        }

        private void StartDownload() {
            if (!IsInTime()) {
                statusIndicator.BackColor = Color.Red;
                return;
            }
            else {
                statusIndicator.BackColor = Color.Green;
            }
            queueingUrls.ReadOnly = true;
            startButton.Enabled = false;
            interceptButton.Enabled = false;
            // ^\s*(http://www\.nicovideo\.jp/watch/(?:[a-z][a-z])?\d+)\s*$
            string[] lines = queueingUrls.Lines
                .Where(x => Regex.IsMatch(x, "^\\s*(http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+)\\s*$"))
                .Select(x => Regex.Match(x, "^\\s*(http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+)\\s*$").Groups[1].Value)
                .ToArray();
            if (lines.Length == 0) {
                queueingUrls.ReadOnly = false;
                interceptButton.Enabled = true;
                startButton.Enabled = true;
                return;
            }
            workingUrl = lines[lines.Length-1];

            queueingUrls.Clear();
            smng.RemoveAll<string>("url");
            for (int i = 0; i < lines.Length-1; i++) {
                string url = lines[i];
                queueingUrls.AppendText(string.Format("{0}\r\n", url));
                smng.AddItem("url", url);
            }

            // SaveSettings();
            

            queueingUrls.ReadOnly = false;
            interceptButton.Enabled = true;
            startButton.Enabled = true;

            downloadWorker.WatchUrl = workingUrl;
            downloadWorker.DownloadAsync(null);
            label1.Text = workingUrl;
            progressBar1.Value = 0;
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
                startButton.Enabled = false;
                interceptButton.Enabled = false;
                MessageBox.Show("Failed to save settings file");
                MessageBox.Show(e.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            pollingTimer.Stop();

            if (downloadWorker.IsBusy) {
                string interraptedUrl = workingUrl;
                // ^\s*(http://www\.nicovideo\.jp/watch/(?:[a-z][a-z])?\d+)\s*$
                string[] lines = queueingUrls.Lines
                    .Where(x => Regex.IsMatch(x, "^\\s*(http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+)\\s*$"))
                    .Select(x => Regex.Match(x, "^\\s*(http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+)\\s*$").Groups[1].Value)
                    .ToArray();

                queueingUrls.Clear();
                for (int i = 0; i < lines.Length-1; i++) {
                    queueingUrls.AppendText(string.Format("{0}\r\n", lines[i]));
                }
                queueingUrls.AppendText(string.Format("{0}\r\n", interraptedUrl));
                if (lines.Length-1 >= 0) {
                    queueingUrls.AppendText(string.Format("{0}\r\n", lines[lines.Length-1]));
                }

                downloadWorker.CancelAsync();
            }
            else { 
            }

            pollingTimer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (downloadWorker.IsBusy || pollingTimer.Enabled == true) {
                MessageBox.Show("Stop downloading before exit.");
                e.Cancel = true;
            }
            else {
                smng.RemoveAll<string>("url");
                var lineQuery = queueingUrls.Lines
                    .Where(x => Regex.IsMatch(x, "^\\s*(http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+)\\s*$"))
                    .Select(x => Regex.Match(x, "^\\s*(http://www\\.nicovideo\\.jp/watch/(?:[a-z][a-z])?\\d+)\\s*$").Groups[1].Value);
                foreach (string line in lineQuery) {
                    smng.AddItem("url", line);
                }
                SaveSettings();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (downloadableTimeEnabled.Checked) {
                downloadableTimeStart.Enabled = true;
                downloadableTimeEnd.Enabled = true;
            }
            else {
                downloadableTimeStart.Enabled = false;
                downloadableTimeEnd.Enabled = false;
            }
        }
    }
}
