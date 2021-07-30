using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private TMP_Text fireText;
    [SerializeField] private TMP_Text iceText;
    
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
        boardManager.StartGame(_firePlayer,_icePlayer);
        welcomeScreen.SetActive(false);
    }
}
