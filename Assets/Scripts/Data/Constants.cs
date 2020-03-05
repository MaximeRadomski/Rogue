using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class Constants
{
    public const float Pixel = 0.0278f;

    //  TAGS  //
    public const string TagCell = "Cell";
    public const string TagButton = "Button";
    public const string TagGrabbableCard = "GrabbableCard";
    public const string TagSoundControler = "SoundControler";
    public const string TagPoppingText = "PoppingText";

    // UNITS OF MEASURE //
    public const string UnitWeight = "kg";
    public const string UnitGold = "©";
    public const string UnitXp = "®";
    public const int MaxGold = 9999;
    public const int HourInMinutes = 60;
    public const int DayInMinutes = 1440;
    public const int RegenerationHp = 1;
    public const int RegenerationTimeLaps = 5;

    // TEXTS //
    public const string YesNoTitle = "Caution!";
    public const string YesNoContent = "Are you sure you want to do this action? It's Repercussions are irreversible!";
    public const string Cancel = "Cancel";
    public const string Proceed = "Proceed";
    public const string InventoryItemPositiveAction = "Switch";
    public const string InventoryItemNegativeAction = "Discard";
    public const string CardPositiveAction = "Venture";

    //  PLAYER PREFS  //
    public const string PpScenePath = "ScenePath";
    public const string PpAudioLevel = "AudioLevel";
    public const string PpJourney = "Journey";
    public const string PpCurrentBiome = "CurrentBiome";
    public const string PpPlayer = "Player";
    public const string PpOpponent = "Opponent";
    public const string PpWeapon1 = "Weapon1";
    public const string PpWeapon2 = "Weapon2";
    public const string PpSkill1 = "Skill1";
    public const string PpSkill2 = "Skill2";
    public const string PpInventoryItem = "InventoryItem";
    public const string PpNbOpponents = "NbOpponents";
    public const string PpSerializeDefault = null;
    public const string PpFavKeyboardLayout = "FavKeyboard";
    public const string PpSoul = "Soul";
    public const string PpRun = "Run";
    public const float PpAudioLevelDefault = 1.0f;
    public const int PpFavKeyboardLayoutDefault = 0;

    //  SCENES  //
    public const string RaceChoiceScene = "RaceChoiceScene";
    public const string SwipeScene = "SwipeScene";
    public const string FightScene = "FightScene";

    //  GAMEOBJECT NAMES  //
    public const string GoSceneBhvName = "SceneBhv";
    public const string GoPlayerName = "Player";
    public const string GoOpponentName = "Opponent";
    public const string GoMatchPercentage = "MatchPercentage";

    //  GAMEOBJECTS VALUES  //
    public const int GridMax = 7;
    public const int UnlimitedPm = GridMax * GridMax;
    public const int VisitedPmValue = -1;
    public const int VisitedSkillValue = -2;
    public static Vector2 CardInitialPosition = new Vector2(0.0f, 0.083f);
    public const float KeyboardHeight = 3.1f;

    //  COLORS  //
    public static Color ColorNormal = new Color(0.83f, 0.83f, 0.83f, 1.0f);     //#d4d4d4
    public static Color ColorMagic = new Color(0.16f, 0.34f, 0.78f, 1.0f);      //#2557c7
    public static Color ColorRare = new Color(1.0f, 0.78f, 0.0f, 1.0f);         //#e1c700
    public static Color ColorLegendary = new Color(0.78f, 0.30f, 0.11f, 1.0f);  //#c74f1c
    public static Color ColorLife = new Color(0.66f, 0.09f, 0.09f, 1.0f);       //#a91717
    public static Color ColorPa = new Color(0.16f, 0.34f, 0.78f, 1.0f);         //#2557c7
    public static Color ColorPm = new Color(0.11f, 0.53f, 0.0f, 1.0f);          //#a91717
    public static Color ColorTransparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);   //#000000
    public static Color ColorPlain = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public static Color ColorPlainTransparent = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    public static Color ColorPlainSemiTransparent = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    public static Color ColorPlainQuarterTransparent = new Color(1.0f, 1.0f, 1.0f, 0.25f);

    // SORTING LAYERS //
    public const string SortingLayerCard = "Card";

    // SKIN CONTAINERS Y OFFSET //
    public const float SkinContainerYOffsetHuman = 0.498f;

    // TIPS AND TRICKS //
    public static string[] TipsAndTricks = {
        "Never underestimate your opponent. Except Gobelins.",
        "Size Matters. Women are huge in this world.",
        "You're nothing without money. Find money.",
        "Remember to reed tips!",
        "You look nice today.",
        "Make war, not love.\n- Famous Orc",
        "Orcs names are not gendered.",
        "Humans are omnivorous.",
        "Gobelins are carnivorous.",
        "Elves are omnivorous.",
        "Dwarves are omnivorous.",
        "Orcs are herbivorous."
    };

    // CACHE SAVES
    public static bool InputLocked = false;
    public static int InputLayer = 0;
    public static string LastEndActionClickedName = null;
    public static List<string> InputTopLayerNames = null;

    public static void IncreaseInputLayer(string name)
    {
        ++InputLayer;
        if (InputTopLayerNames == null)
            InputTopLayerNames = new List<string>();
        InputTopLayerNames.Add(name);
    }

    public static void DecreaseInputLayer()
    {
        --InputLayer;
        if (InputTopLayerNames == null)
            return;
        InputTopLayerNames.RemoveAt(InputTopLayerNames.Count - 1);
    }
}
