using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hazychill.NicoCacheDriver {
    class NicoWaitInfo {

        public DateTime LastAccess { get; private set; }
        public TimeSpan Interval { get; private set; }

        public NicoWaitInfo(TimeSpan interval)
            : this(interval, DateTime.MinValue) {
        }

        public NicoWaitInfo(TimeSpan interval, DateTime lastAccess) {
            LastAccess = lastAccess;
            Interval = interval;
        }

        public void UpdateLastAccess() {
            LastAccess = DateTime.Now;
        }
    }
}
