﻿using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class UIController : MonoBehaviour
{
    // BUTTON EVENTS
    public void ButtonPressed_Lap(int playerIdx)
    {
        gameController.Lap(playerIdx);
        if (gameController.isGameStarted == true)
            gameController.LapButtonPressed();      
    }

    public void ButtonPressed_NextRound()
    {
        HideScorePanel();

        StopAllCoroutines();
        StartCoroutine(
            WaitForAnims(1.2f, () => {
                RestartUI();
                gameController.RestartGame();
            })
        );
    }

    public void ButtonPressed_MainMenu()
    {
        HideScorePanel();

        StopAllCoroutines();
        StartCoroutine(
            WaitForAnims(1.2f, () => {
                RestartUI();
                ResetScores();
                panel_Game.SetActive(false);
                panel_Menu.SetActive(true);
            })
        );
    }

    public void ButtonPressed_GameInfoClosed(int playerIdx)
    {
        gameInfoClosedCounter++;
        panel_GameInfos[playerIdx].SetActive(false);

        if (gameInfoClosedCounter == 2)
            gameController.StartGame();
    }


    // RESTART UI
    private void RestartUI()
    {
        // Reset UI elements
        for (int i = 0; i < gameController.playerCount; i++)
        {
            text_times[i].color = new Color(text_times[i].color.r, text_times[i].color.g, text_times[i].color.b, 1);
            text_times[i].text = "0.000";
            text_times[i].gameObject.SetActive(false);
            winloseObjects[i].SetActive(false);
        }

        //reset Info images
        for (int i = 0; i < spriteObjectContainers.Length; i++)
        {
            Image[] images = spriteObjectContainers[i].GetComponentsInChildren<Image>();
            for (int j = 0; j < images.Length; j++)
            {
                images[j].sprite = nameToSpriteMap["check"];
            }
        }

        allTimersStarted = 0;
    }

    private void ResetScores()
    {
        for (int i = 0; i < gameController.playerCount; i++)
            text_scores[i].text = "0";
       
    }

    private void ShowScorePanel()
    {
        // Show score panel
        showScoreAnimator.SetBool("open", true);
    }

    private void HideScorePanel()
    {
        // Hide score panel
        showScoreAnimator.SetBool("open", false);
    }

    private void HideTimers()
    {
        for (int i = 0; i < gameController.playerCount; i++)
        {
            text_times[i].color = new Color(text_times[i].color.r, text_times[i].color.g, text_times[i].color.b, 0);
        }
    }

    private void HideIntervals()
    {
        for (int i = 0; i < gameController.playerCount; i++)
        {
            text_TimeIntervals[i].gameObject.SetActive(false);
        }
    }

    // UPDATE METHODS
    private void UpdateFailedText(object value, int playerIdx)
    {
        failedTextObjects[playerIdx].SetActive(true);
        failedTextObjects[playerIdx].GetComponentInChildren<TextMeshProUGUI>().text = StringLiterals.FailedObjectText;
    }

    private void UpdateWinLoseText(object value, int playerIdx)
    {
        int otherPlayer = (playerIdx + 1) % gameController.playerCount;
        text_winlose[playerIdx].text = StringLiterals.WinObjectText;
        text_winlose[otherPlayer].text = StringLiterals.LoseObjectText;

        for (int i = 0; i < gameController.playerCount; i++)
        {
            winloseObjects[i].SetActive(true);
            text_TimeIntervals[i].gameObject.SetActive(false);
        }

        text_losetext[playerIdx].gameObject.SetActive(false);
        text_losetext[otherPlayer].gameObject.SetActive(true);
        text_losetext[otherPlayer].text = StringLiterals.BetterLuckObjectText;
        text_scores[playerIdx].text = value.ToString();
    }

    private void UpdateCountDownText(object value, int playerIdx)
    {
        TextMeshProUGUI countdown = text_Countdowns[playerIdx];

        if (countdown.gameObject.activeSelf == false)
            countdown.gameObject.SetActive(true);

        countdown.text = value.ToString();
    }

    private void UpdateTimeInterval(object timeInterval, int playerIdx)
    {
        TextMeshProUGUI interval = text_TimeIntervals[playerIdx];
        interval.gameObject.SetActive(false);

        if (StringLiterals.language == Language.ENGLISH)
        {
            interval.text = StringLiterals.IntervalObjectText + " " + ((int)timeInterval / 1000).ToString();
        }
        else
        {
            interval.text = GetSuffixOfInterval((int)timeInterval / 1000) + StringLiterals.IntervalObjectText;
        }

        interval.gameObject.SetActive(true);
    }

    private string GetSuffixOfInterval(int interval)
    {
        string suffix = "";
        if(interval % 10 == 0)
        {
            switch (interval)
            {
                case 50:
                case 20:
                    suffix = "'ye";
                    break;
                case 40:
                case 30:
                case 10:
                    suffix = "'a";
                    break;
            }
        }
        else
        {
            switch (interval % 10)
            {
                case 9:
                    suffix = "'a";
                    break;
                case 8:
                case 7:
                case 5:
                case 4:
                case 3:
                case 1:
                    suffix = "'e";
                    break;
                case 6:
                    suffix = "'ya";
                    break;
                case 2:
                    suffix = "'ye";
                    break;
            }
        }

        return interval.ToString() + suffix + " ";
    }

    private void UpdateError(object error, int playerIdx)
    {
        GameObject go = Instantiate(animatedTextPrefab, errorTextContainer[playerIdx]);
        TextMeshProUGUI textError = go.GetComponentInChildren<TextMeshProUGUI>();
        go.name = "Text_Error";

        int seconds = Math.Abs((int)error / 1000);
        int millisec = Math.Abs((int)error % 1000);

        string secondStr = "";
        string millisecStr = "";

        if (seconds > 0 && seconds < 10)
            secondStr += "0";

        secondStr += seconds;

        if (millisec < 10)
            millisecStr += "00";
        else if (millisec < 100)
            millisecStr += "0";

        millisecStr += millisec;

        textError.text = (int)error > 0 ? "+" : "-";
        textError.text += secondStr + "." + millisecStr + "";
    }

    private void UpdateTime(object obj, int playerIdx)
    {
        TextMeshProUGUI t = text_times[playerIdx];
        long time = (long)obj;

        if (t.gameObject.activeSelf == false)
            t.gameObject.SetActive(true);

        int seconds = (int)(time / 1000);
        int millisecs = (int)(time - seconds * 1000);

        if (gameController.challenges[0].Type == ChallengeType.Kids)
            t.text = FormatTime(seconds, millisecs, 2);
        else
            t.text = FormatTime(seconds, millisecs, 3);

        if (gameController.challenges[0].Type != ChallengeType.Kids &&
                allTimersStarted < gameController.playerCount
        ){
            StartCoroutine(FadeTextToZeroAlpha(.7f, t));
        }

        allTimersStarted++;
    }

    // https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
    // I had the almost same idea but this looks way better than my idea
    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * t));
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

    }

    IEnumerator WaitForAnims(float time, Action func)
    {
        yield return new WaitForSeconds(time);

        if (func != null)
            func();

    }

    private string FormatTime(int secs, int millisecs, int leadingZeroMs)
    {
        string millisecStr = "";

        if(leadingZeroMs == 3)
        {
            if (millisecs < 10)
                millisecStr += "00";
            else if (millisecs < 100)
                millisecStr += "0";
        }
        else
        {
            millisecs /= 10;
            if (millisecs < 10)
                millisecStr += "0";
        }

        millisecStr += millisecs;

        return secs.ToString() + "." + millisecStr;
    }
}
