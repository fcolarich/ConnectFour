using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TMP_Text fireText;
    [SerializeField] private TMP_Text iceText;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject inGameOptionsScreen;
    [SerializeField] private Slider lenghtSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider winThresholdSlider;
    [SerializeField] private TMP_Text iceTextHud;
    [SerializeField] private TMP_Text fireTextHud;
    [SerializeField] private AudioSource musicPlayer;
    [SerializeField] private GameObject buttonsContainer;
    [SerializeField] private UIManager uiManager;

    private int _firePlayer;
    private int _icePlayer;
    
    
    public void OnFireSelected()
    {
        _firePlayer = _icePlayer <= 0 ? 1 : -1;
        var playerText = $"PLAYER {(_firePlayer > 0 ? 1 : 2)}";
        fireText.text = playerText;
        fireTextHud.text = playerText;
    }

    public void OnIceSelected()
    {
        _icePlayer = _firePlayer <= 0 ? 1 : -1;
        var playerText = $"PLAYER {(_icePlayer > 0 ? 1 : 2)}";
        iceText.text = playerText;
        iceTextHud.text = playerText;
    }

    public void OnBeginSelected()
    {
        StartCoroutine(boardManager.StartGame(_firePlayer,_icePlayer, lenghtSlider.value, heightSlider.value, winThresholdSlider.value));
    }
    public void OnOptionsSelected()
    {
        optionsScreen.SetActive(!optionsScreen.activeSelf);
    }
    
    public void OnInGameOptionsSelected()
    {
        if (inGameOptionsScreen.activeSelf)
        {
            buttonsContainer.SetActive(true);
            inGameOptionsScreen.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            buttonsContainer.SetActive(false);
            inGameOptionsScreen.SetActive(true);
            Time.timeScale = 0;
        }
    }
    
    public void OnMusicToggle()
    {
        if (PlayerPrefs.GetInt("music",0) > 0)
            musicPlayer.Play();
        else
            musicPlayer.Pause();
    }

    public void OnResetSelected()
    {
        _icePlayer = 0;
        _firePlayer = 0;
        var playerText = "AI PLAYER";
        fireText.text = playerText;
        fireTextHud.text = playerText;
        iceText.text = playerText;
        iceTextHud.text = playerText;

        if (inGameOptionsScreen.activeSelf) OnInGameOptionsSelected();
        StartCoroutine(uiManager.RestartGameUI());
    }

    public void OnExitSelected()
    {
        Application.Quit();
    }
}
