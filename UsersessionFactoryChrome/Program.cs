using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace Hazychill.NicoCacheDriver {
    class Program {
        static void Main(string[] args) {
            var usersession = string.Empty;
            var cookiesFile = GetCookieFilePath(args);

            var connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = cookiesFile;
            connectionStringBuilder.ReadOnly = true;
            var connectionString = connectionStringBuilder.ConnectionString;
            using (var connection = new SQLiteConnection(connectionString)) {
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    command.CommandText = "select value from cookies where value like 'user_session%'";
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var usersessionFromDb = reader["value"] as string;
                            if (!string.IsNullOrEmpty(usersessionFromDb)) {
                                usersession = usersessionFromDb;
                            }
                        }
                    }
                }
            }

            Console.WriteLine(usersession);
        }

        private static string GetCookieFilePath(string[] args) {
            var cookiesFile = null as string;
            if (args.Length >= 1) {
                cookiesFile = args[0];
            }
            else {
                cookiesFile = string.Format(@"{0}\Chromium\User Data\Default\Cookies",
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            }
            return cookiesFile;
        }
    }
}
