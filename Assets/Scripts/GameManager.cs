using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private TMP_Text fireText;
    [SerializeField] private TMP_Text iceText;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject inGameOptionsScreen;
    [SerializeField] private Slider lenghtSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private GameObject gameHud;
    [SerializeField] private TMP_Text iceTextHud;
    [SerializeField] private TMP_Text fireTextHud;

    private int _firePlayer;
    private int _icePlayer;
    
    
    public void OnFireSelected()
    {
        _firePlayer = _icePlayer == 0 ? 1 : -1;
        var playerNumber = _firePlayer > 0 ? 1 : 2;
        fireText.text = $"PLAYER {playerNumber}";

    }

    public void OnIceSelected()
    {
        _icePlayer = _firePlayer == 0 ? 1 : -1;
        var playerNumber = _icePlayer > 0 ? 1 : 2;
        iceText.text = $"PLAYER {playerNumber}";
    }

    public void OnBeginSelected()
    {
        boardManager.StartGame(_firePlayer,_icePlayer, lenghtSlider.value, heightSlider.value);
        welcomeScreen.SetActive(false);
        gameHud.SetActive(true);
    }
    public void OnOptionsSelected()
    {
        optionsScreen.SetActive(!optionsScreen.activeSelf);
    }
    
    public void OnInGameOptionsSelected()
    {
        inGameOptionsScreen.SetActive(!inGameOptionsScreen.activeSelf);
    }
    
    public void OnMusicToggle()
    {
        
    }

    public void OnResetSelected()
    {
        
    }

    public void OnExitSelected()
    {
        
    }
    

    
    
}
