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
        rumbleScreen.gameObject.SetActive(false);

        mainScreen.gameObject.SetActive(true);
        gameTitle.gameObject.SetActive(true);
        yield return ChangeCanvasAlphaInTime(1, mainScreen);
        
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
        foreach (var menuButton in mainScreen.GetComponentsInChildren<MenuButton>())
        {
            menuButton.ResetButton();
        }

        mainScreen.gameObject.SetActive(false);
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-2, gameTitle));
        gameTitle.gameObject.SetActive(false);
        gameHud.gameObject.SetActive(true);
        yield return StartCoroutine(ChangeCanvasAlphaInTime(2, gameHud));
        
    }

    public IEnumerator ShowBeingText()
    {
        beginText.gameObject.SetActive(true);
        yield return StartCoroutine(ChangeCanvasAlphaInTime(0.5f, beginText));
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-1, beginText));
        beginText.gameObject.SetActive(false);
    }

    public IEnumerator ShowPlayerWinText(int player)
    {
        restartButton.gameObject.SetActive(true);
        winText.gameObject.SetActive(true);
        winText.GetComponent<TMP_Text>().text = $"PLAYER {player} WINS!";
        yield return StartCoroutine(ChangeCanvasAlphaInTime(0.5f, winText));
        yield return StartCoroutine(ChangeCanvasAlphaInTime(1, restartButton));
    }

    public IEnumerator RestartGameUI()
    {
        StartCoroutine(ChangeCanvasAlphaInTime(-1, restartButton));
        StartCoroutine(ChangeCanvasAlphaInTime(-1, gameHud));
        
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-0.5f, winText));
        
        restartButton.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        gameHud.gameObject.SetActive(false);
        rumbleScreen.gameObject.SetActive(true);
        yield return ChangeCanvasAlphaInTime(2, rumbleScreen);
        boardManager.CleanBoard();
    }

}
