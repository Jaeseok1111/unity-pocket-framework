using System;
using System.Collections.Generic;
using ThePocket.Utils.SQLite;

namespace ThePocket
{
    public partial class GameDataContext
    {
        private readonly DatabaseContext _database;
        private readonly DatabaseSyncronizer _syncronizer;

        private Dictionary<Type, Table> _tables = new();

        public GameDataContext()
        {
            _database = new DatabaseContext("GameData", autoConnect: true);
            _syncronizer = new DatabaseSyncronizer("GameData");
        }

        public void Syncronize()
        {
            _syncronizer.Syncronize();
        }

        public void AddTable<TRecord, TTable, TScheme>()
            where TRecord : IRecord
            where TTable : Table, new()
            where TScheme : Scheme, new()
        {
            TTable newTable = Table.Create<TTable>(_database);
            TScheme newScheme = new TScheme();

            _syncronizer.Fetch(newScheme);
            _tables.Add(typeof(TRecord), newTable);
        }

        public TTable GetTable<TTable>() where TTable : Table
        {
            return (TTable)_tables.GetValueOrDefault(typeof(TTable));
        }
    }
}