using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Hazychill.NicoCacheDriver {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string settingsFilePath = null;
            if (args.Length >= 1) {
                settingsFilePath = args[0];
            }
            Application.Run(new NicoCacheDriverForm(settingsFilePath));
        }
    }
}
