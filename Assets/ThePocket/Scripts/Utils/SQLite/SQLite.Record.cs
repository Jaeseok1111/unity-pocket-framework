using System.Data;

namespace ThePocket.Utils.SQLite
{
    public interface IRecord
    {
        public void Fetch(IDataReader reader);
    }
}