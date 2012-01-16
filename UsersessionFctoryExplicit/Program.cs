using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hazychill.NicoCacheDriver {
    class Program {
        static void Main(string[] args) {
            string usersession;
            if (args.Length >= 1) {
                usersession = args[0];
            }
            else {
                usersession = string.Empty;
            }
            Console.WriteLine(usersession);
        }
    }
}
