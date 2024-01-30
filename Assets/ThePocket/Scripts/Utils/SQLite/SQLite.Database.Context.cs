using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace ThePocket.Utils.SQLite
{
    public class DatabaseContext
    {
        private readonly string _name;
        private readonly string _path;
        private readonly string _connectionString;
        
        public DatabaseContext(string databaseName)
        {
            _name = $"{databaseName}.sqlite";

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    _path = Application.persistentDataPath;
                    break;
                default:
                    _path = Application.dataPath;
                    break;
            }

            _connectionString = $"Data Source={_path}/{_name};Version=3;";

            Debug.Log($"SQLite Database Context " +
                $"[\n" +
                $"Version={SqliteConnection.SQLiteVersion}\n" +
                $"Name={_name}\n" +
                $"Path={_path}\n" +
                $"ConnectionString={_connectionString}\n" +
                $"]");

            if (System.IO.File.Exists($"{_path}/{_name}") == false)
            {
                SqliteConnection.CreateFile($"{_path}/{_name}");
            }
        }

        public void ExecuteNonQuery(string query)
        {
            ExecuteCommand(query, (command) =>
            {
                command.ExecuteNonQuery();
            });
        }

        public List<T> ExecuteQuery<T>(string query) where T : IRecord, new()
        {
            List<T> result = new List<T>();

            ExecuteCommand(query, (command) =>
            {
                IDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    T item = new();
                    item.Fetch(reader);

                    result.Add(item);
                }
            });

            return result;
        }

        private void ExecuteCommand(string query, System.Action<IDbCommand> executeCommand)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = query;

                    executeCommand.Invoke(command);
                }
            } 
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
