using System.Data;

namespace ThridParty.SQLite
{
    public interface IRecord
    {
        public void Fetch(IDataReader reader);
    }
}