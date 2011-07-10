using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Hazychill.NicoCacheDriver {
    class DownloadProgressChangedEventArgs : ProgressChangedEventArgs {
        public DownloadProgressChangedEventArgs(int progressPercentage, long bytesReceived, long totalBytesToReceive, object userState) : base(progressPercentage, userState) {
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
            WillWait = 0;
        }

        public DownloadProgressChangedEventArgs(int progressPercentage, long bytesReceived, long totalBytesToReceive, int willWait, object userState) : this(progressPercentage, bytesReceived, totalBytesToReceive, userState) {
            WillWait = willWait;
        }

        public long BytesReceived { get; private set; }
        public long TotalBytesToReceive { get; private set; }
        public int WillWait { get; private set; }
    }
}
