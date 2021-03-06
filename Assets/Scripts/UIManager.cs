using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup rumbleLogos;
    [SerializeField] private CanvasGroup rumbleScreen;
    [SerializeField] private CanvasGroup mainScreen;
    [SerializeField] private CanvasGroup gameHud;
    [SerializeField] private CanvasGroup gameTitle;
    [SerializeField] private CanvasGroup beginText;
    [SerializeField] private CanvasGroup winText;
    [SerializeField] private CanvasGroup restartButton;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private MenuButton fireMenuButton;
    [SerializeField] private MenuButton iceMenuButton;
    private bool _alreadyInit;

    public IEnumerator Start()
    {
        if (!_alreadyInit)
        {
            rumbleLogos.gameObject.SetActive(true);
            yield return ChangeCanvasAlphaInTime(2, rumbleLogos);
            yield return ChangeCanvasAlphaInTime(-1, rumbleLogos);
            rumbleLogos.gameObject.SetActive(false);
            _alreadyInit = true;
        }
        rumbleScreen.gameObject.SetActive(true);
        yield return ChangeCanvasAlphaInTime(-2, rumbleScreen);
        gameTitle.gameObject.SetActive(true);
        
        StartCoroutine(ChangeCanvasAlphaInTime(1, gameTitle));
        mainScreen.gameObject.SetActive(true);
        yield return ChangeCanvasAlphaInTime(1, mainScreen);
        rumbleScreen.gameObject.SetActive(false);

    }


    //We use this method to fade in or fade out screens using their CanvasGroup.
    //If the Time value passed is positive it will fade in and if its negative, it will fade out.
    private IEnumerator ChangeCanvasAlphaInTime(float time, CanvasGroup canvasGroup)
    {
        float timePassed = 0;
        var speed = 1 / time;
        while (Math.Abs(time) > timePassed)
        {
            canvasGroup.alpha += speed * Time.deltaTime;
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnBeginButtonPressed()
    {
        StartCoroutine(FadeMainScreenAndLoadHud());
    }
    
    private IEnumerator FadeMainScreenAndLoadHud()
    {
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-1, mainScreen));
        fireMenuButton.ResetButton();
        iceMenuButton.ResetButton();
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-2, gameTitle));
        gameHud.gameObject.SetActive(true);
        yield return StartCoroutine(ChangeCanvasAlphaInTime(2, gameHud));
        mainScreen.gameObject.SetActive(false);
        gameTitle.gameObject.SetActive(false);
    }

    public IEnumerator ShowBeingText()
    {
        beginText.gameObject.SetActive(true);
        yield return StartCoroutine(ChangeCanvasAlphaInTime(1f, beginText));
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-1, beginText));
        beginText.gameObject.SetActive(false);
    }

    public IEnumerator ShowPlayerWinText(int player)
    {
        yield return new WaitForSeconds(2);
        restartButton.gameObject.SetActive(true);
        winText.gameObject.SetActive(true);
        var text = player == 0 ? "NO MORE MOVES!" : $"PLAYER {player} WINS!";

        winText.GetComponent<TMP_Text>().text = text;
        yield return StartCoroutine(ChangeCanvasAlphaInTime(0.5f, winText));
        yield return StartCoroutine(ChangeCanvasAlphaInTime(1, restartButton));
    }

    public IEnumerator RestartGameUI()
    {
        StartCoroutine(ChangeCanvasAlphaInTime(-1, restartButton));
        StartCoroutine(ChangeCanvasAlphaInTime(-1, gameHud));
        
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-0.5f, winText));
        
        rumbleScreen.gameObject.SetActive(true);
        yield return ChangeCanvasAlphaInTime(2, rumbleScreen);
        restartButton.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        gameHud.gameObject.SetActive(false);
        boardManager.CleanBoard();
    }

}
