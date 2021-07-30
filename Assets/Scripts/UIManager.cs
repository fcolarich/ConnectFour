using System;
using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup rumbleLogos;
    [SerializeField] private CanvasGroup rumbleScreen;
    [SerializeField] private CanvasGroup mainScreen;
    [SerializeField] private CanvasGroup gameHud;
    [SerializeField] private CanvasGroup gameTitle;
    
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return ChangeCanvasAlphaInTime(3f, rumbleLogos);
        yield return ChangeCanvasAlphaInTime(-3, rumbleLogos);
        rumbleLogos.gameObject.SetActive(false);
        yield return ChangeCanvasAlphaInTime(-3, rumbleScreen);
        rumbleScreen.gameObject.SetActive(false);
        yield return ChangeCanvasAlphaInTime(3, mainScreen);
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
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-2, mainScreen));
        yield return StartCoroutine(ChangeCanvasAlphaInTime(-2, gameTitle));
        yield return StartCoroutine(ChangeCanvasAlphaInTime(3, gameHud));
    }

}
