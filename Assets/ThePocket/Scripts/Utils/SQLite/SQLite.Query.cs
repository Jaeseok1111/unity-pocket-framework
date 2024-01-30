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

        private Query(
            DatabaseContext context, string tableName,
            IEnumerable<string> whereClauseList, IEnumerable<string> orderClauseList, 
            int limit, int page)
        {
            _context = context;
            _tableName = tableName;

            _whereClauseList = whereClauseList;
            _orderClauseList = orderClauseList;

            _limit = limit;
            _page = page;
        }

        public Query<T> Where(FieldScheme field, object value)
        {
            string newCondition = $"({field.Name} = {value})";

            return new Query<T>(
                context: _context,
                tableName: _tableName,
                whereClauseList: UnionClause(_whereClauseList, newCondition), 
                orderClauseList: _orderClauseList,
                limit: _limit,
                page: _page);
        }

        public Query<T> Limit(int limit)
        {
            return new Query<T>(
                context: _context,
                tableName: _tableName,
                whereClauseList: _whereClauseList,
                orderClauseList: _orderClauseList,
                limit: limit,
                page: _page);
        }

        public Query<T> SetPage(int page)
        {
            return new Query<T>(
                context: _context,
                tableName: _tableName,
                whereClauseList: _whereClauseList,
                orderClauseList: _orderClauseList,
                limit: _limit,
                page: page);
        }

        public Query<T> OrderByAsc(string key)
        {
            string newCondition = $"{key} asc";

            return new Query<T>(
                context: _context,
                tableName: _tableName,
                whereClauseList: _whereClauseList,
                orderClauseList: UnionClause(_orderClauseList, newCondition),
                limit: _limit,
                page: _page);
        }

        public Query<T> OrderByDesc(string key)
        {
            string newCondition = $"{key} desc";

            return new Query<T>(
                context: _context,
                tableName: _tableName,
                whereClauseList: _whereClauseList,
                orderClauseList: UnionClause(_orderClauseList, newCondition),
                limit: _limit,
                page: _page);
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

            StringBuilder query = new StringBuilder();

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

            StringBuilder query = new StringBuilder();

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

            StringBuilder query = new StringBuilder();

            query.Append($"delete from {_tableName}");

            if (_whereClauseList.Any() == false)
            {
                query.Append(" where " + string.Join(" and ", _whereClauseList));
            }

            _context.ExecuteNonQuery(query.ToString());
        }
    }
}
