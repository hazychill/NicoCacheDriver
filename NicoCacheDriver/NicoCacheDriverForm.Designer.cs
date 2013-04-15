namespace Hazychill.NicoCacheDriver {
    partial class NicoCacheDriverForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NicoCacheDriverForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.etaLabel = new System.Windows.Forms.Label();
            this.onlineController = new System.Windows.Forms.CheckBox();
            this.cancelDLButton = new System.Windows.Forms.Button();
            this.interceptButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.downloadableTimeEnabled = new System.Windows.Forms.CheckBox();
            this.downloadableTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.downloadableTimeStart = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.queueingUrls = new System.Windows.Forms.TextBox();
            this.statusIndicator = new System.Windows.Forms.Panel();
            this.pasteToBottomButton = new System.Windows.Forms.Button();
            this.pasteTopButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.reloadSettingsButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.pollingTimer = new System.Windows.Forms.Timer(this.components);
            this.downloadWorker = new Hazychill.NicoCacheDriver.DownloadWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.etaLabel);
            this.splitContainer1.Panel1.Controls.Add(this.onlineController);
            this.splitContainer1.Panel1.Controls.Add(this.cancelDLButton);
            this.splitContainer1.Panel1.Controls.Add(this.interceptButton);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.downloadableTimeEnabled);
            this.splitContainer1.Panel1.Controls.Add(this.downloadableTimeEnd);
            this.splitContainer1.Panel1.Controls.Add(this.downloadableTimeStart);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.progressBar1);
            this.splitContainer1.Panel1.Controls.Add(this.queueingUrls);
            this.splitContainer1.Panel1.Controls.Add(this.statusIndicator);
            this.splitContainer1.Panel1.Controls.Add(this.pasteToBottomButton);
            this.splitContainer1.Panel1.Controls.Add(this.pasteTopButton);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.clearButton);
            this.splitContainer1.Panel2.Controls.Add(this.reloadSettingsButton);
            this.splitContainer1.Panel2.Controls.Add(this.exitButton);
            this.splitContainer1.Panel2.Controls.Add(this.outputTextBox);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(580, 564);
            this.splitContainer1.SplitterDistance = 290;
            this.splitContainer1.TabIndex = 4;
            this.splitContainer1.TabStop = false;
            // 
            // etaLabel
            // 
            this.etaLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.etaLabel.Location = new System.Drawing.Point(512, 243);
            this.etaLabel.Name = "etaLabel";
            this.etaLabel.Size = new System.Drawing.Size(55, 12);
            this.etaLabel.TabIndex = 61;
            this.etaLabel.Text = " (-000:00)";
            this.etaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // onlineController
            // 
            this.onlineController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.onlineController.Appearance = System.Windows.Forms.Appearance.Button;
            this.onlineController.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.onlineController.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.onlineController.Location = new System.Drawing.Point(48, 206);
            this.onlineController.Name = "onlineController";
            this.onlineController.Size = new System.Drawing.Size(75, 23);
            this.onlineController.TabIndex = 10;
            this.onlineController.Text = "Online";
            this.onlineController.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.onlineController.UseVisualStyleBackColor = true;
            this.onlineController.Click += new System.EventHandler(this.onlineOfflineButton_Click);
            // 
            // cancelDLButton
            // 
            this.cancelDLButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelDLButton.Enabled = false;
            this.cancelDLButton.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.cancelDLButton.Location = new System.Drawing.Point(210, 206);
            this.cancelDLButton.Name = "cancelDLButton";
            this.cancelDLButton.Size = new System.Drawing.Size(75, 23);
            this.cancelDLButton.TabIndex = 30;
            this.cancelDLButton.Text = "Cancel";
            this.cancelDLButton.UseVisualStyleBackColor = true;
            this.cancelDLButton.Click += new System.EventHandler(this.cancelDLButton_Click);
            // 
            // interceptButton
            // 
            this.interceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.interceptButton.Enabled = false;
            this.interceptButton.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.interceptButton.Location = new System.Drawing.Point(129, 206);
            this.interceptButton.Name = "interceptButton";
            this.interceptButton.Size = new System.Drawing.Size(75, 23);
            this.interceptButton.TabIndex = 20;
            this.interceptButton.Text = "Interrupt";
            this.interceptButton.UseVisualStyleBackColor = true;
            this.interceptButton.Click += new System.EventHandler(this.interceptButton_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(483, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "-";
            // 
            // downloadableTimeEnabled
            // 
            this.downloadableTimeEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadableTimeEnabled.AutoSize = true;
            this.downloadableTimeEnabled.Location = new System.Drawing.Point(392, 213);
            this.downloadableTimeEnabled.Name = "downloadableTimeEnabled";
            this.downloadableTimeEnabled.Size = new System.Drawing.Size(15, 14);
            this.downloadableTimeEnabled.TabIndex = 40;
            this.downloadableTimeEnabled.UseVisualStyleBackColor = true;
            // 
            // downloadableTimeEnd
            // 
            this.downloadableTimeEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadableTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.downloadableTimeEnd.Location = new System.Drawing.Point(496, 210);
            this.downloadableTimeEnd.Name = "downloadableTimeEnd";
            this.downloadableTimeEnd.ShowUpDown = true;
            this.downloadableTimeEnd.Size = new System.Drawing.Size(71, 19);
            this.downloadableTimeEnd.TabIndex = 60;
            this.downloadableTimeEnd.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // downloadableTimeStart
            // 
            this.downloadableTimeStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadableTimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.downloadableTimeStart.Location = new System.Drawing.Point(410, 210);
            this.downloadableTimeStart.Name = "downloadableTimeStart";
            this.downloadableTimeStart.ShowUpDown = true;
            this.downloadableTimeStart.Size = new System.Drawing.Size(71, 19);
            this.downloadableTimeStart.TabIndex = 50;
            this.downloadableTimeStart.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 242);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 2;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Enabled = false;
            this.progressBar1.Location = new System.Drawing.Point(12, 258);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(555, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // queueingUrls
            // 
            this.queueingUrls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.queueingUrls.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.queueingUrls.Location = new System.Drawing.Point(12, 12);
            this.queueingUrls.Multiline = true;
            this.queueingUrls.Name = "queueingUrls";
            this.queueingUrls.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.queueingUrls.Size = new System.Drawing.Size(555, 188);
            this.queueingUrls.TabIndex = 0;
            this.queueingUrls.WordWrap = false;
            this.queueingUrls.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.queueingUrls_UpdateTimerEvent);
            // 
            // statusIndicator
            // 
            this.statusIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusIndicator.BackColor = System.Drawing.Color.Gray;
            this.statusIndicator.Location = new System.Drawing.Point(13, 207);
            this.statusIndicator.Name = "statusIndicator";
            this.statusIndicator.Size = new System.Drawing.Size(20, 20);
            this.statusIndicator.TabIndex = 10;
            // 
            // pasteToBottomButton
            // 
            this.pasteToBottomButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pasteToBottomButton.Image = global::NicoCacheDriver.Properties.Resources.downarrow;
            this.pasteToBottomButton.Location = new System.Drawing.Point(349, 206);
            this.pasteToBottomButton.Name = "pasteToBottomButton";
            this.pasteToBottomButton.Size = new System.Drawing.Size(23, 23);
            this.pasteToBottomButton.TabIndex = 63;
            this.pasteToBottomButton.UseVisualStyleBackColor = true;
            this.pasteToBottomButton.Click += new System.EventHandler(this.pasteToBottomButton_Click);
            // 
            // pasteTopButton
            // 
            this.pasteTopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pasteTopButton.Image = global::NicoCacheDriver.Properties.Resources.uparrow;
            this.pasteTopButton.Location = new System.Drawing.Point(327, 206);
            this.pasteTopButton.Name = "pasteTopButton";
            this.pasteTopButton.Size = new System.Drawing.Size(23, 23);
            this.pasteTopButton.TabIndex = 62;
            this.pasteTopButton.UseVisualStyleBackColor = true;
            this.pasteTopButton.Click += new System.EventHandler(this.pasteTopButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.clearButton.Location = new System.Drawing.Point(92, 236);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 90;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // reloadSettingsButton
            // 
            this.reloadSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reloadSettingsButton.Location = new System.Drawing.Point(11, 236);
            this.reloadSettingsButton.Name = "reloadSettingsButton";
            this.reloadSettingsButton.Size = new System.Drawing.Size(75, 23);
            this.reloadSettingsButton.TabIndex = 80;
            this.reloadSettingsButton.Text = "Reload";
            this.reloadSettingsButton.UseVisualStyleBackColor = true;
            this.reloadSettingsButton.Click += new System.EventHandler(this.reloadSettingsButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.Location = new System.Drawing.Point(492, 236);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 100;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputTextBox.Location = new System.Drawing.Point(12, 12);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(555, 218);
            this.outputTextBox.TabIndex = 70;
            this.outputTextBox.WordWrap = false;
            // 
            // pollingTimer
            // 
            this.pollingTimer.Interval = 1000;
            this.pollingTimer.Tick += new System.EventHandler(this.pollingTimer_Tick);
            // 
            // downloadWorker
            // 
            this.downloadWorker.Timer = null;
            this.downloadWorker.WatchUrl = null;
            this.downloadWorker.DownloadProgressChanged += new Hazychill.NicoCacheDriver.DownloadProgressChangedEventHandler(this.downloadWorker1_DownloadProgressChanged);
            this.downloadWorker.DownloadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.downloadWorker1_DownloadCompleted);
            // 
            // NicoCacheDriverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 564);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NicoCacheDriverForm";
            this.Text = "NicoCacheDriver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Resize += new System.EventHandler(this.NicoCacheDriverForm_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button interceptButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox queueingUrls;
        private System.Windows.Forms.TextBox outputTextBox;
        private DownloadWorker downloadWorker;
        private System.Windows.Forms.Timer pollingTimer;
        private System.Windows.Forms.DateTimePicker downloadableTimeStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox downloadableTimeEnabled;
        private System.Windows.Forms.DateTimePicker downloadableTimeEnd;
        private System.Windows.Forms.Panel statusIndicator;
        private System.Windows.Forms.Button cancelDLButton;
        private System.Windows.Forms.CheckBox onlineController;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button reloadSettingsButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label etaLabel;
        private System.Windows.Forms.Button pasteTopButton;
        private System.Windows.Forms.Button pasteToBottomButton;
    }
}

