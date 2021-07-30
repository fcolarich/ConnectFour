using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject rumbleScreen;
    [SerializeField] private TMP_Text fireText;
    [SerializeField] private TMP_Text iceText;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject inGameOptionsScreen;
    [SerializeField] private Slider lenghtSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private GameObject gameHud;
    [SerializeField] private TMP_Text iceTextHud;
    [SerializeField] private TMP_Text fireTextHud;
    [SerializeField] private AudioSource musicPlayer;

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
        StartCoroutine(boardManager.StartGame(_firePlayer,_icePlayer, lenghtSlider.value, heightSlider.value));
    }
    public void OnOptionsSelected()
    {
        optionsScreen.SetActive(!optionsScreen.activeSelf);
    }
    
    public void OnInGameOptionsSelected()
    {
        if (inGameOptionsScreen.activeSelf)
        {
            inGameOptionsScreen.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnExitSelected()
    {
        Application.Quit();
    }
}
