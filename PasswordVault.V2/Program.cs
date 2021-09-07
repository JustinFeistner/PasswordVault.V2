using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PasswordVault.V2
{
    class Program
    {
        static void Main()
        {
            Intro();
            using var connection = CreateConnection();

            OpenConnection(connection);

            var tableName = "justin";

            var stop = true;

            while (stop)
            {
                Console.WriteLine("What would you like to do?\n\t- (C)reate a new table\n\t- (A)dd a new record\n\t- (S)earch for a record\n\t- (U)pdate a record\n\t- (D)elete a record");
                var choice = Console.ReadLine().ToLower();

                switch (choice)
                {
                    case "c":
                        CreateTable(connection, tableName);
                        break;
                    case "a":
                        AddNewRecord(connection, tableName);
                        break;
                    case "s":
                        SearchRecords(connection, tableName);
                        break;
                    case "u":
                        UpdateRecord(connection, tableName);
                        break;
                    case "d":
                        DeleteRecord(connection, tableName);
                        break;
                }

                Console.WriteLine("Would you like to end this application (Y/N)?");
                var endApp = Console.ReadLine().ToLower();
                if (endApp == "y")
                {
                    stop = false;
                }
            }

            CloseConnection(connection);
        }

        private static void DeleteRecord(SQLiteConnection connection, string tableName)
        {
            SearchRecords(connection, tableName);

            Console.WriteLine("Please choose the ID for the record you would like to delete: ");
            var recordId = Int32.Parse(Console.ReadLine());

            var text = $"DELETE FROM {tableName} WHERE id = {recordId}";
            var cmd = new SQLiteCommand(text, connection);
            cmd.ExecuteNonQuery();
        }

        private static void UpdateRecord(SQLiteConnection connection, string tableName)
        {
            SearchRecords(connection, tableName);

            Console.WriteLine("Please choose the ID for the record you would like to modify: ");
            var recordId = Int32.Parse(Console.ReadLine());

            Console.WriteLine("What would you like to modify? \n\t- (S)ervice\n\t- (U)serName\n\t- (P)assword\n\t- (N)ote");
            var columnName = Console.ReadLine().ToLower();

            switch (columnName)
            {
                case "s":
                    columnName = "service";
                    break;
                case "u":
                    columnName = "userName";
                    break;
                case "p":
                    columnName = "password";
                    break;
                case "n":
                    columnName = "notes";
                    break;
            }

            Console.WriteLine("Please enter the new value:");
            var newValue = Console.ReadLine();

            var text = $"UPDATE {tableName} SET {columnName} = '{newValue}' WHERE id = {recordId}";
            var cmd = new SQLiteCommand(text, connection);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// https://zetcode.com/csharp/sqlite/
        /// https://www.csharp-console-examples.com/general/creating-sqlite-database-and-table-in-csharp-console/
        /// </summary>
        /// <param name="connection"></param>
        private static void SearchRecords(SQLiteConnection connection, string tableName)
        {
            Console.WriteLine("Do you want (A)ll records or search for (S)pecific record?");
            var decision = Console.ReadLine().ToLower();

            var text = string.Empty;

            if (decision == "a")
            {
                text = $"SELECT * FROM {tableName}";
            }
            else
            {
                Console.WriteLine("What is the name of the service you are looking for? ");
                var serviceName = Console.ReadLine();

                text = $"SELECT * FROM {tableName} WHERE service LIKE '%{serviceName}%'";
            }

            var cmd = new SQLiteCommand(text, connection);

            var dataReader = cmd.ExecuteReader();
            Console.WriteLine($"{dataReader.GetName(0), 3}{dataReader.GetName(1), 15}{dataReader.GetName(2), 15}{dataReader.GetName(3), 15}{dataReader.GetName(4), 30}");
            while (dataReader.Read())
            {
                Console.WriteLine($@"{dataReader.GetInt32(0), 3}{dataReader.GetString(1), 15}{dataReader.GetString(2), 15}{dataReader.GetString(3), 15}{dataReader.GetString(4), 30}");
            }
        }

        private static void AddNewRecord(SQLiteConnection connection, string tableName)
        {
            var cmd = new SQLiteCommand(connection);

            var newRecord = true;

            while (newRecord)
            {
                Console.WriteLine("Enter the service name: ");
                var service = Console.ReadLine();
                Console.WriteLine("Enter the username: ");
                var userName = Console.ReadLine();
                Console.WriteLine("Enter the password: ");
                var password = Console.ReadLine();
                Console.WriteLine("Enter any notes: ");
                var notes = Console.ReadLine();

                cmd.CommandText = $"INSERT INTO {tableName}(service, userName, password, notes) VALUES(@service, @userName, @password, @notes)";
                cmd.Parameters.AddWithValue("@service", $"{service}");
                cmd.Parameters.AddWithValue("@userName", $"{userName}");
                cmd.Parameters.AddWithValue("@password", $"{password}");
                cmd.Parameters.AddWithValue("@notes", $"{notes}");
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                Console.WriteLine("Would you like to add another record (Y/N)? ");
                var addRecord = Console.ReadLine();

                if (addRecord == "n")
                {
                    newRecord = false;
                }
            }
        }

        private static void CreateTable(SQLiteConnection connection, string tableName)
        {
            var cmd = new SQLiteCommand(connection);

            cmd.CommandText = $"DROP TABLE IF EXISTS {tableName}";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"CREATE TABLE {tableName}(id INTEGER PRIMARY KEY, service TEXT, userName TEXT, password TEXT, notes TEXT)";
            cmd.ExecuteNonQuery();
        }

        private static void CloseConnection(SQLiteConnection connection)
        {
            connection.Close();
        }

        private static void OpenConnection(SQLiteConnection connection)
        {
            connection.Open();
        }

        private static SQLiteConnection CreateConnection()
        {
            var connectionString = @"URI=file:C:\_repos\PasswordVault.V2\test.db";

            var connection = new SQLiteConnection(connectionString);

            return connection;
        }

        private static void Intro()
        {
            Console.WriteLine("Welcome to the Password Vault");
        }
    }
}
