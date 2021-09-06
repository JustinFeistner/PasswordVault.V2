using Microsoft.VisualBasic.FileIO;
using System;
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

            var stop = true;

            while (stop)
            {
                Console.WriteLine("What would you like to do?\n\t- (C)reate a new table\n\t- (A)dd a new record\n\t- (S)earch for a record\n\t- (U)pdate a record\n\t- (D)elete a record");
                var choice = Console.ReadLine();

                if (choice == "c")
                {
                    CreateTable(connection);
                }
                else if (choice == "a")
                {
                    AddNewRecord(connection);
                }
                else if (choice == "s")
                {
                    SearchRecords(connection);
                }
                Console.WriteLine("Would you like to end this application (Y/N)?");
                var endApp = Console.ReadLine();
                if (endApp == "y")
                {
                    stop = false;
                }
            }

            CloseConnection(connection);
        }

        /// <summary>
        /// https://www.csharp-console-examples.com/general/creating-sqlite-database-and-table-in-csharp-console/
        /// </summary>
        /// <param name="connection"></param>
        private static void SearchRecords(SQLiteConnection connection)
        {
            Console.WriteLine("Which table do you want? ");
            var tableName = Console.ReadLine();

            Console.WriteLine("What is the name of the service you are looking for? ");
            var serviceName = Console.ReadLine();

            //var text = $"SELECT * FROM {tableName} WHERE service = '{serviceName}'";
            var text = $"SELECT * FROM {tableName} WHERE service LIKE '%{serviceName}%'";
            var cmd = new SQLiteCommand(text, connection);

            var dataReader = cmd.ExecuteReader();
            //int counter = 0;

            while (dataReader.Read())
            {
                //counter++;
                Console.WriteLine(dataReader[0] + " : " + dataReader[1] + " " + dataReader[2] + " " + dataReader[3] + " " + dataReader[4]);
            }
        }

        private static void AddNewRecord(SQLiteConnection connection)
        {
            var cmd = new SQLiteCommand(connection);
            Console.WriteLine("Which table do you want? ");

            var tableName = Console.ReadLine();

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

        private static void CreateTable(SQLiteConnection connection)
        {
            Console.WriteLine("Please choose a table name: ");
            var tableName = Console.ReadLine();

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
