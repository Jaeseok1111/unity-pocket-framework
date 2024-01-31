namespace ThePocket.Utils.SQLite
{
    public abstract class Table
    {
        private DatabaseContext _context;
        
        public static T Create<T>(DatabaseContext context) where T : Table, new()
        {
            T newTable = new T();
            newTable._context = context;
            return newTable;
        }

        protected Query<T> NewQuery<T>(string tableName) where T : IRecord, new()
        {
            return new Query<T>(_context, tableName);
        }
    }
}