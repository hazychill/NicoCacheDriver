using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Hazychill.NicoCacheDriver {
    class DownloadProgressChangedEventArgs : ProgressChangedEventArgs {
        public DownloadProgressChangedEventArgs(int progressPercentage, long bytesReceived, long totalBytesToReceive, int willWait, string title, object userState)
            : base(progressPercentage, userState) {

            this.BytesReceived = bytesReceived;
            this.TotalBytesToReceive = totalBytesToReceive;
            this.WillWait = willWait;
            this.Title = title;
        }
        public DownloadProgressChangedEventArgs(int progressPercentage, long bytesReceived, long totalBytesToReceive, string title, object userState)
            : this(progressPercentage, bytesReceived, totalBytesToReceive, 0, title, userState) {
        }

        public DownloadProgressChangedEventArgs(int progressPercentage, long bytesReceived, long totalBytesToReceive, object userState)
            : this(progressPercentage, bytesReceived, totalBytesToReceive, 0, null, userState) {
        }

        public DownloadProgressChangedEventArgs(int progressPercentage, long bytesReceived, long totalBytesToReceive, int willWait, object userState)
            : this(progressPercentage, bytesReceived, totalBytesToReceive, willWait, null, userState) {
        }

        public long BytesReceived { get; private set; }
        public long TotalBytesToReceive { get; private set; }
        public int WillWait { get; private set; }
        public string Title { get; private set; }
    }
}
