using ThridParty.SQLite;
using UnityEngine;
using Zenject;

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
