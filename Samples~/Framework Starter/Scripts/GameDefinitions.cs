namespace GameLecture.Framework.Samples
{
    public enum GameEvent
    {
        GoldChanged,
        LanguageChanged
    }

    public enum GameLanguage
    {
        Korean,
        English
    }

    public enum GameScene
    {
        LoadingScene,
        MainScene,
        GameScene
    }

    public static class GameSceneExtensions
    {
        public static void Load(this GameScene scene) =>
            SceneManager.Instance.LoadScene(scene.ToString());
    }
}
