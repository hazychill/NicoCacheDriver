using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Hazychill.NicoCacheDriver {
    class NicoCacheDriverProgressChangedEventArgs : ProgressChangedEventArgs {
        public NicoCacheDriverProgressChangedEventArgs(int percentage, object userState) : base(percentage, userState) { }

        public string Message { get; set; }
        public NicoCacheDriverChangedProgressType Type { get; set; }
    }

    enum NicoCacheDriverChangedProgressType {
        None = 0,
        MiddleOfDownload,
        FileCompleted,
        Retry,
        Error,
        Wait
    }
}
