﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeSceneBhv : MonoBehaviour
{
    private Character _playerCharacter;
    private Instantiator _instantiator;

    void Start()
    {
        Application.targetFrameRate = 60;
        SetPrivates();
        SetButtons();
    }

    private void SetPrivates()
    {
        _playerCharacter = JsonUtility.FromJson<Character>(PlayerPrefs.GetString(Constants.PpPlayer, Constants.PpSerializeDefault));
        _instantiator = GetComponent<Instantiator>();
        _instantiator.SetPrivates();
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToRaceChoiceScene;
        _instantiator.NewCard(1);
        _instantiator.NewCard(0);
        GameObject.Find("ButtonFloatingDislike").GetComponent<ButtonBhv>().EndActionDelegate = GameObject.Find("Card1").GetComponent<GrabbableCardBhv>().Dislike;
        GameObject.Find("ButtonFloatingLike").GetComponent<ButtonBhv>().EndActionDelegate = GameObject.Find("Card1").GetComponent<GrabbableCardBhv>().Like;
    }

    public void NewCard()
    {
        Destroy(GameObject.Find("Card1"));
        var backCard = GameObject.Find("Card0");
        backCard.GetComponent<GrabbableCardBhv>().BringToFront();
        GameObject.Find("ButtonFloatingDislike").GetComponent<ButtonBhv>().EndActionDelegate = backCard.GetComponent<GrabbableCardBhv>().Dislike;
        GameObject.Find("ButtonFloatingLike").GetComponent<ButtonBhv>().EndActionDelegate = backCard.GetComponent<GrabbableCardBhv>().Like;
        _instantiator.NewCard(0);
    }

    public void GoToRaceChoiceScene()
    {
        SceneManager.LoadScene(Constants.RaceChoiceScene);
    }

    public void GoToFightScene(List<Character> opponentCharacters)
    {
        for (int i = 0; i < opponentCharacters.Count; ++i)
        {
            PlayerPrefsHelper.SaveCharacter(Constants.PpOpponent + i, opponentCharacters[i]);
        }
        PlayerPrefs.SetInt(Constants.PpNbOpponents, opponentCharacters.Count);
        SceneManager.LoadScene(Constants.FightScene);
    }
}