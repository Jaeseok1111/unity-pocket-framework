using Zenject;

namespace ThePocket
{
    public class GameDataInstaller : Installer<GameDataInstaller.OnGameDataInit, GameDataInstaller>
    {
        public delegate void OnGameDataInit(GameDataContext context);

        private OnGameDataInit _onGameDataInit;

        public GameDataInstaller(OnGameDataInit onGameDataInit)
        {
            _onGameDataInit = onGameDataInit;
        }

        public override void InstallBindings()
        {
            GameDataContext context = new GameDataContext();

            _onGameDataInit.Invoke(context);

            context.Syncronize();

            Container
                .Bind<GameDataContext>()
                .FromInstance(context)
                .AsSingle();
        }
    }
}