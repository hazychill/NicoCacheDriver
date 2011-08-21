using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Hazychill.Setting;
using System.Threading;

namespace Hazychill.NicoCacheDriver {
    class NicoAccessTimer {
        Dictionary<string, NicoWaitInfo> waitInfoMap;
        Dictionary<string, List<Regex>> patternMap;

        public NicoAccessTimer(ISettingsManager smng) : this(smng, null) {
        }

        private NicoAccessTimer(ISettingsManager smng, Dictionary<string, NicoWaitInfo> baseWaitInfoMap) {
            waitInfoMap = new Dictionary<string, NicoWaitInfo>();
            patternMap = new Dictionary<string, List<Regex>>();

            foreach (string timerName in smng.GetItems<string>(SettingsConstants.TIMER_NAME)) {
                string intervalKey = string.Format("{0}_{1}", SettingsConstants.TIMER_INTERVAL_PREFIX, timerName);
                TimeSpan interval = smng.GetItem<TimeSpan>(intervalKey);
                NicoWaitInfo waitInfo;
                NicoWaitInfo baseWaitInfo;
                if (baseWaitInfoMap != null && baseWaitInfoMap.TryGetValue(timerName, out baseWaitInfo)) {
                    waitInfo = new NicoWaitInfo(interval, baseWaitInfo.LastAccess);
                }
                else {
                    waitInfo = new NicoWaitInfo(interval);
                }
                waitInfoMap.Add(timerName, waitInfo);

                List<Regex> patternList = new List<Regex>();
                patternMap.Add(timerName, patternList);
                string patternKey = string.Format("{0}_{1}", SettingsConstants.TIMER_PATTERN_PREFIX, timerName);
                foreach (Regex pattern in smng.GetItems<Regex>(patternKey)) {
                    patternList.Add(pattern);
                }
            }
        }

        public int WaitBeforeAccess(string url) {
            int waitMillseconds;

            NicoWaitInfo waitInfo = GetWaitInfo(url);
            if (waitInfo == null) {
                waitMillseconds = 0;
            }
            else {
                DateTime now = DateTime.Now;
                DateTime last = waitInfo.LastAccess;
                TimeSpan accessInterval = waitInfo.Interval;

                TimeSpan timeAfterLastAccess = now - last;
                if (timeAfterLastAccess < accessInterval) {
                    TimeSpan wait = accessInterval - timeAfterLastAccess;
                    waitMillseconds = (int)wait.TotalMilliseconds;
                }
                else {
                    waitMillseconds = 0;
                }
                waitInfo.UpdateLastAccess();
            }

            return waitMillseconds;
        }

        public bool CanAccess(string url) {
            bool canAccess;
            int waitMillseconds;

            NicoWaitInfo waitInfo = GetWaitInfo(url);
            if (waitInfo == null) {
                canAccess = true;
            }
            else {
                DateTime now = DateTime.Now;
                DateTime last = waitInfo.LastAccess;
                TimeSpan accessInterval = waitInfo.Interval;

                TimeSpan timeAfterLastAccess = now - last;
                if (timeAfterLastAccess < accessInterval) {
                    TimeSpan wait = accessInterval - timeAfterLastAccess;
                    waitMillseconds = (int)wait.TotalMilliseconds;
                    canAccess = false;
                }
                else {
                    waitMillseconds = 0;
                    canAccess = true;
                    waitInfo.UpdateLastAccess();
                }
            }

            return canAccess;
        }

        public void UpdateLastAccess(string url) {
            NicoWaitInfo waitInfo = GetWaitInfo(url);
            if (waitInfo != null) {
                waitInfo.UpdateLastAccess();
            }
        }

        private NicoWaitInfo GetWaitInfo(string url) {
            return patternMap.Keys
              .Where(timerName => patternMap[timerName]
                                    .Where(pattern => pattern.IsMatch(url))
                                    .Any())
              .Select(timerName => waitInfoMap[timerName])
              .FirstOrDefault();
        }

        public NicoAccessTimer DeriveNewTimer(SettingsManager smng) {
            NicoAccessTimer timer = new NicoAccessTimer(smng, waitInfoMap);
            return timer;
        }
    }
}
