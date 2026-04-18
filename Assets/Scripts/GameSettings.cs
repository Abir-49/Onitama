public enum GameMode { Random, Customized }
public enum FirstPlayerChoice { Random, MinimaxFirst, GreedyFirst }

public class CardDefinition
{
    public string DisplayName;
    public string MaterialName;

    public CardDefinition(string displayName, string materialName)
    {
        DisplayName = displayName;
        MaterialName = materialName;
    }
}

public static class GameSettings
{
    public static float DelayPerMove = 1.0f;
    public static GameMode CurrentMode = GameMode.Random;
    public static FirstPlayerChoice FirstPlayer = FirstPlayerChoice.Random;

    public static readonly CardDefinition[] AllCards = new CardDefinition[]
    {
        new CardDefinition("Boar", "CardBoar"),
        new CardDefinition("Cobra", "CardCobra"),
        new CardDefinition("Crab", "CardCrab"),
        new CardDefinition("Crane", "CardCrane"),
        new CardDefinition("Dragon", "CardDragon"),
        new CardDefinition("Eel", "CardEel"),
        new CardDefinition("Elephant", "CardElephant"),
        new CardDefinition("Frog", "CardFrog"),
        new CardDefinition("Goose", "CardGoose"),
        new CardDefinition("Horse", "CardHorse"),
        new CardDefinition("Mantis", "CardMantis"),
        new CardDefinition("Monkey", "CardMonkey"),
        new CardDefinition("Ox", "CardOx"),
        new CardDefinition("Rabbit", "CardRabbit"),
        new CardDefinition("Rooster", "CardRooster"),
        new CardDefinition("Tiger", "CardTiger"),
    };

    public static readonly CardDefinition[] PlayableCards = AllCards;
    
    // Customized mode logic. Defaults will be overwritten by the Main Menu
    public static readonly string[] DefaultMinimaxCards = new string[] { "Dragon", "Horse" };
    public static readonly string[] DefaultGreedyCards = new string[] { "Mantis", "Ox" };
    public static readonly string DefaultExtraCard = "Rabbit";

    public static string[] MinimaxCards = new string[] { "Dragon", "Horse" };
    public static string[] GreedyCards = new string[] { "Mantis", "Ox" };
    public static string ExtraCard = "Rabbit";

    public static string[] GetCardNames(CardDefinition[] cards)
    {
        string[] names = new string[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            names[i] = cards[i].DisplayName;
        }

        return names;
    }

    public static bool TryGetCardDefinition(string displayName, out CardDefinition card)
    {
        for (int i = 0; i < AllCards.Length; i++)
        {
            if (AllCards[i].DisplayName == displayName)
            {
                card = AllCards[i];
                return true;
            }
        }

        card = null;
        return false;
    }

    public static string GetMaterialName(string displayName)
    {
        if (TryGetCardDefinition(displayName, out CardDefinition card))
        {
            return card.MaterialName;
        }

        return displayName;
    }
}
