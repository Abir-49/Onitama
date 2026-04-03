public enum GameMode { Random, Customized }

public static class GameSettings
{
    public static float DelayPerMove = 1.0f;
    public static GameMode CurrentMode = GameMode.Random;
    
    // Customized mode logic. Defaults will be overwritten by the Main Menu
    public static string[] MinimaxCards = new string[] { "Dragon", "Horse" };
    public static string[] GreedyCards = new string[] { "Mantis", "Ox" };
    public static string ExtraCard = "Rabbit";
}
