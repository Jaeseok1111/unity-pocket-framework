using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePocket.Utils.SQLite
{
    public class Query<T> where T : IRecord, new()
    {
        private DatabaseContext _context;
        private string _tableName;

        private IEnumerable<string> _whereClauseList = null;
        private IEnumerable<string> _orderClauseList = null;

        private int _limit = 0;
        private int _page = 0;

        public Query(DatabaseContext context, string tableName)
        {
            _context = context;
            _tableName = tableName;
        }

        private Query<T> CopyWith(
            DatabaseContext context = null, string tableName = null,
            IEnumerable<string> whereClauseList = null, IEnumerable<string> orderClauseList = null,
            int? limit = null, int? page = null)
        {
            Query<T> newQuery = new(context ?? _context, tableName ?? _tableName);

            newQuery._whereClauseList = whereClauseList ?? _whereClauseList;
            newQuery._orderClauseList = orderClauseList ?? _orderClauseList;
            newQuery._limit = limit ?? _limit;
            newQuery._page = page ?? _page;

            return newQuery;
        }

        public Query<T> Where(FieldScheme field, object value)
        {
            string newCondition = $"({field.Name} = {value})";

            return CopyWith(whereClauseList: UnionClause(_whereClauseList, newCondition));
        }

        public Query<T> OrderByAsc(string key)
        {
            string newCondition = $"{key} asc";

            return CopyWith(orderClauseList: UnionClause(_orderClauseList, newCondition));
        }

        public Query<T> OrderByDesc(string key)
        {
            string newCondition = $"{key} desc";

            return CopyWith(orderClauseList: UnionClause(_orderClauseList, newCondition));
        }

        public Query<T> Limit(int limit)
        {
            return CopyWith(limit: limit);
        }

        public Query<T> SetPage(int page)
        {
            return CopyWith(page: page);
        }

        private IEnumerable<string> UnionClause(IEnumerable<string> clauseList, string newConnection)
        {
            if (clauseList == null)
            {
                return new string[] { newConnection };
            }

            return clauseList.Union(new string[] { newConnection });
        }

        public List<T> Select()
        {
            if (_context == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_tableName))
            {
                return null;
            }

            StringBuilder query = new();

            query.Append($"select * from {_tableName}");

            if (_whereClauseList.Any() == false)
            {
                query.Append(" where " + string.Join(" and ", _whereClauseList));
            }

            if (_orderClauseList.Any() == false)
            {
                query.Append(" order by " + string.Join(", ", _orderClauseList));
            }

            if (_limit > 0)
            {
                query.Append($" limit {_limit} offset {_page} * {_limit}");
            }

            return _context.ExecuteQuery<T>(query.ToString());
        }

        public void Update(IDictionary<FieldScheme, object> fields)
        {
            if (_context == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_tableName))
            {
                return;
            }

            StringBuilder query = new();

            query.Append($"update {_tableName} set ");
            query.Append(string.Join(",", fields.Select(field => $"{field.Key.Name} = {field.Value}")));

            if (_whereClauseList.Any() == false)
            {
                query.Append(" where " + string.Join(" and ", _whereClauseList));
            }

            _context.ExecuteNonQuery(query.ToString());
        }

        public void Delete()
        {
            if (_context == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_tableName))
            {
                return;
            }

            StringBuilder query = new();

            query.Append($"delete from {_tableName}");

            if (_whereClauseList.Any() == false)
            {
                query.Append(" where " + string.Join(" and ", _whereClauseList));
            }

            _context.ExecuteNonQuery(query.ToString());
        }
    }
}
