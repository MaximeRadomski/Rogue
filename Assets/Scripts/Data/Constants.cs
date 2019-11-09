﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    //  TAGS  //
    public const string TagCell = "Cell";
    public const string TagButton = "Button";
    public const string TagGrabbableCard = "GrabbableCard";
    public const string TagSoundControler = "SoundControler";
    public const string TagPoppingText = "PoppingText";

    //  PLAYER PREFS  //
    public const string PpAudioLevel = "AudioLevel";
    public const float PpAudioLevelDefault = 1;
    public const string PpPlayer = "Player";
    public const string PpOpponent = "Opponent";
    public const string PpWeapon1 = "Weapon1";
    public const string PpWeapon2 = "Weapon2";
    public const string PpSkill1 = "Skill1";
    public const string PpSkill2 = "Skill2";
    public const string PpNbOpponents = "NbOpponents";
    public const string PpSerializeDefault = null;

    //  SCENES  //
    public const string RaceChoiceScene = "RaceChoiceScene";
    public const string SwipeScene = "SwipeScene";
    public const string FightScene = "FightScene";

    //  GAMEOBJECT NAMES  //
    public const string GoSceneBhvName = "SceneBhv";
    public const string GoPlayerName = "Player";
    public const string GoOpponentName = "Opponent";

    //  GAMEOBJECTS VALUES  //
    public const int GridMax = 7;
    public static Vector2 CardInitialPosition = new Vector2(0.0f, 0.5f);

    //  COLORS  //
    public static Color ColorNormal = new Color(0.83f, 0.83f, 0.83f, 1.0f);     //#d4d4d4
    public static Color ColorMagic = new Color(0.16f, 0.34f, 0.78f, 1.0f);      //#2557c7
    public static Color ColorRare = new Color(1.0f, 0.78f, 0.0f, 1.0f);         //#e1c700
    public static Color ColorLegendary = new Color(0.78f, 0.30f, 0.11f, 1.0f);  //#c74f1c
    public static Color ColorLife = new Color(0.66f, 0.09f, 0.09f, 1.0f);       //#a91717
    public static Color ColorPa = new Color(0.16f, 0.34f, 0.78f, 1.0f);         //#2557c7
    public static Color ColorPm = new Color(0.11f, 0.53f, 0.0f, 1.0f);          //#a91717
    public static Color ColorTransparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);   //#000000
}
