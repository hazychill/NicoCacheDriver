namespace Hazychill.NicoCacheDriver {
    partial class nicoCacheDriverForm {
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.downloadableTimeEnabled = new System.Windows.Forms.CheckBox();
            this.downloadableTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.downloadableTimeStart = new System.Windows.Forms.DateTimePicker();
            this.interceptButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.queueingUrls = new System.Windows.Forms.TextBox();
            this.statusIndicator = new System.Windows.Forms.Panel();
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
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.downloadableTimeEnabled);
            this.splitContainer1.Panel1.Controls.Add(this.downloadableTimeEnd);
            this.splitContainer1.Panel1.Controls.Add(this.downloadableTimeStart);
            this.splitContainer1.Panel1.Controls.Add(this.interceptButton);
            this.splitContainer1.Panel1.Controls.Add(this.startButton);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.progressBar1);
            this.splitContainer1.Panel1.Controls.Add(this.queueingUrls);
            this.splitContainer1.Panel1.Controls.Add(this.statusIndicator);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputTextBox);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(540, 543);
            this.splitContainer1.SplitterDistance = 319;
            this.splitContainer1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(414, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "-";
            // 
            // downloadableTimeEnabled
            // 
            this.downloadableTimeEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadableTimeEnabled.AutoSize = true;
            this.downloadableTimeEnabled.Location = new System.Drawing.Point(298, 236);
            this.downloadableTimeEnabled.Name = "downloadableTimeEnabled";
            this.downloadableTimeEnabled.Size = new System.Drawing.Size(15, 14);
            this.downloadableTimeEnabled.TabIndex = 8;
            this.downloadableTimeEnabled.UseVisualStyleBackColor = true;
            this.downloadableTimeEnabled.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // downloadableTimeEnd
            // 
            this.downloadableTimeEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadableTimeEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.downloadableTimeEnd.Location = new System.Drawing.Point(431, 233);
            this.downloadableTimeEnd.Name = "downloadableTimeEnd";
            this.downloadableTimeEnd.ShowUpDown = true;
            this.downloadableTimeEnd.Size = new System.Drawing.Size(89, 19);
            this.downloadableTimeEnd.TabIndex = 7;
            this.downloadableTimeEnd.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // downloadableTimeStart
            // 
            this.downloadableTimeStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadableTimeStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.downloadableTimeStart.Location = new System.Drawing.Point(319, 233);
            this.downloadableTimeStart.Name = "downloadableTimeStart";
            this.downloadableTimeStart.ShowUpDown = true;
            this.downloadableTimeStart.Size = new System.Drawing.Size(89, 19);
            this.downloadableTimeStart.TabIndex = 6;
            this.downloadableTimeStart.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // interceptButton
            // 
            this.interceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.interceptButton.Location = new System.Drawing.Point(129, 231);
            this.interceptButton.Name = "interceptButton";
            this.interceptButton.Size = new System.Drawing.Size(75, 23);
            this.interceptButton.TabIndex = 5;
            this.interceptButton.Text = "Intercept";
            this.interceptButton.UseVisualStyleBackColor = true;
            this.interceptButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // startButton
            // 
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startButton.Location = new System.Drawing.Point(48, 231);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 267);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 2;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Enabled = false;
            this.progressBar1.Location = new System.Drawing.Point(12, 283);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(514, 23);
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
            this.queueingUrls.Size = new System.Drawing.Size(514, 213);
            this.queueingUrls.TabIndex = 0;
            // 
            // statusIndicator
            // 
            this.statusIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusIndicator.BackColor = System.Drawing.Color.Gray;
            this.statusIndicator.Location = new System.Drawing.Point(13, 232);
            this.statusIndicator.Name = "statusIndicator";
            this.statusIndicator.Size = new System.Drawing.Size(20, 20);
            this.statusIndicator.TabIndex = 10;
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
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(514, 194);
            this.outputTextBox.TabIndex = 0;
            // 
            // pollingTimer
            // 
            this.pollingTimer.Interval = 1000;
            this.pollingTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // downloadWorker
            // 
            this.downloadWorker.WatchUrl = null;
            this.downloadWorker.DownloadProgressChanged += new Hazychill.NicoCacheDriver.DownloadProgressChangedEventHandler(this.downloadWorker1_DownloadProgressChanged);
            this.downloadWorker.DownloadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.downloadWorker1_DownloadCompleted);
            // 
            // nicoCacheDriverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 543);
            this.Controls.Add(this.splitContainer1);
            this.Name = "nicoCacheDriverForm";
            this.Text = "NicoCacheDriver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
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
        private System.Windows.Forms.Button startButton;
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
    }
}

