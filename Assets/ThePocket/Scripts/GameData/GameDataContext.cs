using ThePocket.Utils.SQLite;
using UnityEngine;
using Zenject;

namespace ThePocket
{
    public partial class GameDataContext : DatabaseContext, IInitializable
    {
        [Inject]
        public GameDataContext()
            : base("GameData")
        {
        }

        public void Initialize()
        {
            Debug.Log("GameDataContext Initialize");
        }
    }
}