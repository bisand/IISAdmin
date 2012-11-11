using System.Data.SQLite;
using System.IO;

namespace IisAdmin.Database
{
    public class InternalStorage
    {
        private const string _connectionString = "Data Source=C:\\Temp\\iisadmin.db;Version=3;";
        private const string _databasePath = "C:\\Temp\\iisadmin.db";

        public InternalStorage()
        {
            if (!File.Exists(_databasePath))
                InitializeDatabase();
        }

        // DANGER!!! We should NOT store passwords in the database. 
        public void AddUser(string username, string password, string homeDirectory, string fqdn)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var sql =
                    string.Format(
                        "INSERT INTO Users(Username, Password, HomeDirectory, fqdn) Values('{0}', '{1}', '{2}', '{3}')",
                        username, password, homeDirectory, fqdn);

                ExecuteNonQuery(connection, sql);
            }
        }

        public void DeleteUser(string username)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var sql = string.Format("DELETE FROM Users WHERE Username='{0}'", username);

                ExecuteNonQuery(connection, sql);
            }
        }

        // DANGER!!! We should NOT store passwords in the database. 
        public void SetPassword(string username, string password)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var sql = string.Format("UPDATE Users SET Password='{0}' WHERE Username='{1}'", password, username);

                ExecuteNonQuery(connection, sql);
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            try
            {
                using (var trans = connection.BeginTransaction())
                {
                    // Create the command and execute query
                    var cmd = new SQLiteCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    // Commit the changes into the database
                    trans.Commit();
                }
            }
            finally
            {
                // Close the database connection. Uncommetted changes will be lost.
                connection.Close();
            }
        }

        private void InitializeDatabase()
        {
            var dir = Path.GetDirectoryName(_databasePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            SQLiteConnection.CreateFile(_databasePath);
            // Open connection to database
            var sqliteCon = new SQLiteConnection(_connectionString);
            sqliteCon.Open();

            // Define the SQL Create table statement
            const string sql = "CREATE TABLE [Users] (" +
                               "[Username] TEXT NULL," +
                               "[Password] TEXT  NULL," + // DANGER Will Robinson! DANGER!!!
                               "[HomeDirectory] TEXT  NULL," +
                               "[fqdn] TEXT  NULL" +
                               ")";

            using (var sqlTransaction = sqliteCon.BeginTransaction())
            {
                // Create the table
                var createCommand = new SQLiteCommand(sql, sqliteCon);
                createCommand.ExecuteNonQuery();
                createCommand.Dispose();

                // Commit the changes into the database
                sqlTransaction.Commit();
            } // end using

            // Close the database connection
            sqliteCon.Close();
        }
    }
}