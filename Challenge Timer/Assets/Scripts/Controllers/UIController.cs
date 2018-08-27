﻿using System;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class UIController : MonoBehaviour
{
    public static UIController Instance;
    private GameController gameController;
    public Animator showScoreAnimator;

   // Dictionary<string, Sprite> challengeTypeToSprite;

    public GameObject pageMediumPrefab;
    public GameObject pageSmallPrefab;
    public GameObject animatedTextPrefab;

    public Transform challengeTypeContent;
    public Transform[] errorTextContainer;
    public GameObject panel_Menu;
    public GameObject panel_Game;
    public GameObject panel_gameEnd;
    public GameObject[] winloseObjects;
    public GameObject[] failedTextObjects;

    public TextMeshProUGUI[] text_TimeIntervals;
    public TextMeshProUGUI[] text_Countdowns;
    public TextMeshProUGUI[] text_times;
    public TextMeshProUGUI[] text_winlose;
    public TextMeshProUGUI[] text_losetext;
    public TextMeshProUGUI[] text_scores;

    public GameObject[] spriteObjectContainers;
    Dictionary<string, Sprite> nameToSpriteMap;
    // We should reset this variable when the game is restarted.
    int allTimersStarted;

    /*
     Game Screen Color
     FF006C => pinkish
         */


    // SETUP GAME
    private void Start()
    {
        if (Instance != null)
            return;

        LoadSprites();
        Instance = this;
        gameController = GameController.Instance;
        gameController.UpdateTimeInterval += UpdateTimeInterval;
        gameController.UpdateError += UpdateError;
        gameController.UpdateCountDownText += UpdateCountDownText;
        gameController.UpdateTimeText += UpdateTime;
        gameController.UpdateWinLoseText += UpdateWinLoseText;
        gameController.UpdateFailedText += UpdateFailedText;
        gameController.HideScorePanel += HideScorePanel;
        gameController.RestartUI += RestartUI;
        gameController.ShowScorePanel += ShowScorePanel;
        gameController.UpdateInfoSprites += UpdateInfoSprites;
        gameController.HideTimers += HideTimers;
        FillPickerLists();
    }

    

    void LoadSprites()
    {
        nameToSpriteMap = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Icons");
        for (int i = 0; i < sprites.Length; i++)
        {
            nameToSpriteMap.Add(sprites[i].name, sprites[i]);
        }
    }

    private void FillPickerLists()
    {
        // We don't use this. VerticalScrollSnap.RemoveAllChildren need it.
        GameObject[] removed;

        // Fill challenge types.
        string[] challengeTypes = gameController.challengeTypes;
        VerticalScrollSnap challengeTypeSnap = challengeTypeContent.parent.gameObject.GetComponent<VerticalScrollSnap>();
        challengeTypeSnap.RemoveAllChildren(out removed);
        foreach (GameObject go in removed)
            Destroy(go);

        for (int i = 0; i < challengeTypes.Length; i++)
        {
            GameObject page = Instantiate(pageMediumPrefab, challengeTypeContent);
            page.GetComponentInChildren<TextMeshProUGUI>().text = challengeTypes[i];

            // There was a bug before when we use i variable in delegate.
            // I don't know if it's still exist. 
            int index = i;
            page.GetComponent<Button>().onClick.AddListener(delegate
            {
                challengeTypeSnap.ChangePage(index);
                challengeTypeContent.parent.parent.gameObject.GetComponent<ScrollView>().ToggleListSize();
            });
        }
        challengeTypeSnap.UpdateLayout();
    }

    public void ScrollViewChallengeType(int selectedPage)
    {
        for (int i = 0; i < gameController.playerCount; i++)
        {
            gameController.challenges[i].Type = (ChallengeType)(Enum.Parse(typeof(ChallengeType), gameController.challengeTypes[selectedPage]));

            switch (gameController.challenges[i].Type)
            {
                case ChallengeType.Infinite:
                case ChallengeType.Kids:
                    gameController.SetChallengeSettings(gameController.challenges[i].Type, 2000, 500, 3);
                    break;
                case ChallengeType.Random:
                    gameController.SetChallengeSettings(gameController.challenges[i].Type, -1, 400, -1, 1, 5);
                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateInfoSprites(object value, int playerIdx)
    {
        int index = (int)value;
        Image image = spriteObjectContainers[playerIdx].GetComponentsInChildren<Image>()[index];

        image.sprite = nameToSpriteMap["wrong"];
    }


    // BUTTON EVENTS
    public void ButtonPressed_Challenge()
    {
        panel_Menu.SetActive(false);
        panel_Game.SetActive(true);
        RestartUI();
        gameController.StartGame();
    }

    public void ButtonPressed_Github()
    {
        Application.OpenURL("https://github.com/hundredmsgames/challenge-timer");
    }

    public void ButtonPressed_FollowTheNumbers()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.HundredMsGameS.followTheNumbers");
    }
}
