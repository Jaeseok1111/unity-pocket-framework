using Sample;
using ThePocket.Utils.SQLite;
using UnityEngine;
using Zenject;

namespace ThePocket
{
    public partial class GameDataContext : IInitializable
    {
        private readonly DatabaseContext _database;

        [Inject]
        public GameDataContext()
        {
            _database = new DatabaseContext("GameData");
            
            Construct();
        }

        partial void Construct();

        public void Initialize()
        {
            Debug.Log("GameDataContext Initialize");
        }
    }
}