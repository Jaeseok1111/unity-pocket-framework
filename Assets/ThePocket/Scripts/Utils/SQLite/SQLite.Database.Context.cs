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

        private readonly bool _autoConnect = false;

        private bool _isConnected = false;
        private SqliteConnection _connection;

        public DatabaseContext(string databaseName, bool autoConnect)
        {
            _name = $"{databaseName}.sqlite";
            _autoConnect = autoConnect;

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

            if (System.IO.File.Exists($"{_path}/{_name}") == false)
            {
                SqliteConnection.CreateFile($"{_path}/{_name}");
            }
        }

        public void NewConnection()
        {
            if (_isConnected)
            {
                return;
            }

            _isConnected = true;
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
        }

        public void Close()
        {
            if (_connection == null)
            {
                return;
            }

            _isConnected = false;

            _connection.Close();
            _connection = null;
        }

        public IDbCommand CreateCommand(string query)
        {
            if (_connection == null)
            {
                return null;
            }

            IDbCommand command = _connection.CreateCommand();
            command.CommandText = query;

            return command;
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

        private void ExecuteCommand(string query, Action<IDbCommand> executeCommand)
        {
            try
            {
                if (_autoConnect) NewConnection();
                {
                    IDbCommand command = _connection.CreateCommand();
                    command.CommandText = query;

                    executeCommand.Invoke(command);
                }
                if (_autoConnect) Close();
            } 
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public override string ToString()
        {
            return $"SQLite Database Context " +
            $"[\n" +
                $"Version={SqliteConnection.SQLiteVersion}\n" +
                $"Name={_name}\n" +
                $"Path={_path}\n" +
                $"ConnectionString={_connectionString}\n" +
            $"]";
        }
    }
}
