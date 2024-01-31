using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace ThePocket.Utils.SQLite
{
    public sealed class DatabaseSyncronizer
    {
        private readonly DatabaseContext _context;

        private List<Scheme> _fetches = new();

        public DatabaseSyncronizer(string databaseName)
        {
            _context = new(databaseName, autoConnect: false);
        }

        public void Fetch(Scheme scheme)
        {
            _fetches.Add(scheme);
        }

        public void Syncronize()
        {
            Debug.Log("Start of database synchronization");

            _context.NewConnection();
            {
                DatabaseSyncQueryGenerator generator = new(_context);

                foreach (var query in generator.GenerateSyncQueries(_fetches))
                {
                    _context.ExecuteNonQuery(query);
                }
            }
            _context.Close();
        }
    }

    public sealed class DatabaseSyncQueryGenerator
    {
        private enum UpdateStep
        {
            DropIndex,
            AlterColumn,
            AddColumn,
            CreateTable,
            UpdateDefaultConstraint,
            CreateIndex,
            DropColumn,
            DropTable,
        }

        private DatabaseContext _context;
        private Dictionary<UpdateStep, ICollection<string>> _queries;

        public DatabaseSyncQueryGenerator(DatabaseContext context)
        {
            _context = context;
            _queries = new();

            foreach (UpdateStep step in Enum.GetValues(typeof(UpdateStep)))
            {
                _queries.Add(step, new List<string>());
            }
        }

        public IEnumerable<string> GenerateSyncQueries(List<Scheme> fetches)
        {
            foreach (ICollection<String> queries in _queries.Values)
            {
                queries.Clear();
            }

            GatherQuery(fetches);

            return _queries
                .OrderBy(kvp => kvp.Key)
                .SelectMany(kvp => kvp.Value);
        }

        private void GatherQuery(List<Scheme> codebase)
        {
            List<Scheme> database = GatherDatabaseScheme(codebase.Select(x => x.TableName));

            foreach (Scheme scheme in database)
            {
                Scheme codeScheme = codebase.Find(x => x.TableName == scheme.TableName);
                if (codeScheme == null)
                {
                    continue;
                }

                if (scheme.Equals(codeScheme))
                {
                    continue;
                }

                AlterTable(scheme, codeScheme);
            }

            foreach (Scheme fetch in codebase)
            {
                if (database.Any(x => x.TableName == fetch.TableName))
                {
                    continue;
                }

                CreateTable(fetch);
            }
        }

        private List<Scheme> GatherDatabaseScheme(IEnumerable<string> fetchTableNames)
        {
            List<Scheme> schemes = new List<Scheme>();

            foreach (string tableName in fetchTableNames)
            {
                var command = _context.CreateCommand($"SELECT sql FROM sqlite_schema WHERE name = '{tableName}';");
                if (command == null)
                {
                    continue;
                }

                IDataReader reader = command.ExecuteReader();
                if (reader.Read() == false)
                {
                    continue;
                }

                var sql = reader.GetString(0);
                var scheme = Scheme.Parse(sql);

                schemes.Add(scheme);
            }

            return schemes;
        }

        private void CreateTable(Scheme scheme)
        {
            AddQuery(UpdateStep.CreateTable, scheme.ToQuery());
        }

        private void AlterTable(Scheme database, Scheme codebase)
        {
            Debug.Log($"Alter Table {database.TableName}");

            List<Scheme.Field> databaseFields = database.GetFields();
            List<Scheme.Field> codebaseFields = codebase.GetFields();

            foreach (var field in databaseFields)
            {
                Scheme.Field codebaseField = codebaseFields.FirstOrDefault(x => x.Name == field.Name);

                if (codebaseField == null)
                {
                    DropColumn(database, field);
                }
                else
                {
                    if (field.Equals(codebaseField) == false)
                    {
                        AlterColumn(database, field, codebaseField);
                    }

                    codebaseFields.Remove(codebaseField);
                }
            }

            foreach (var field in codebaseFields)
            {
                AddColumn(database, field);
            }
        }

        private void DropColumn(Scheme scheme, Scheme.Field field)
        {
            Debug.Log($"Drop Column {scheme.TableName}.{field.Name}");

            AddQuery(UpdateStep.DropColumn, $"ALTER TABLE {scheme.TableName} DROP COLUMN {field.Name}");
        }

        // SQLite의 경우 컬럼의 변경이 지원되지 않는다
        // 따라서, 변경을 하려면 제거하고 다시 생성하는 방법 밖에 없다
        private void AlterColumn(Scheme scheme, Scheme.Field before, Scheme.Field after)
        {
            Debug.Log($"Alter Column {scheme.TableName}.{before.Name}");

            bool updateType = before.Type != after.Type;
            bool updateNotNull = before.NotNull != after.NotNull;
            bool updateUnique = before.Unique != after.Unique;

            if (updateType ||  updateNotNull || updateUnique)
            {
                AddQuery(UpdateStep.AlterColumn, $"ALTER TABLE {scheme.TableName} DROP COLUMN {before.Name}");
                AddQuery(UpdateStep.AlterColumn, $"ALTER TABLE {scheme.TableName} ADD COLUMN {after.ToQuery()}");
            }
        }

        private void AddColumn(Scheme scheme, Scheme.Field field)
        {
            Debug.Log($"Add Column {scheme.TableName}.{field.Name}");

            AddQuery(UpdateStep.AddColumn, $"ALTER TABLE {scheme.TableName} ADD COLUMN {field.ToQuery()}");
        }

        private void AddQuery(UpdateStep step, string query)
        {
            _queries[step].Add(query);
        }
    }
}