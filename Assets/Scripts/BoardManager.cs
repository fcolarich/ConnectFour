using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const float CAMERA_SIZE_RELATION = 4.5f / 7f;
    
    [SerializeField] public int boardLenght = 7;
    [SerializeField] public int boardHeight = 5;
    [SerializeField] public int victoryThreshold = 4;
    [SerializeField] public GameObject tilePrefab;
    [SerializeField] public GameObject buttonPrefab;
    [SerializeField] public GameObject firePlayerToken;
    [SerializeField] public GameObject icePlayerToken;
    [SerializeField] public GameObject aiPlayerPrefab;
    [SerializeField] public List<AIPlayer> aiPlayers = new List<AIPlayer>();
    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject leftPlatform;
    [SerializeField] public GameObject rightPlatform;
    [SerializeField] public UIManager uiManager;

    [SerializeField] public Transform tokenContainer;
    [SerializeField] public Transform buttonsContainer;
    [SerializeField] public Transform tilesContainer;

    [SerializeField] public GameObject fireAnimation;
    [SerializeField] public GameObject iceAnimation;
    
    
    public Board CurrentBoard;
    public int lastPlayer = -1;
    private int _firePlayer = 1;
    private WaitForSeconds _waitForSecondsBetweenTiles = new WaitForSeconds(0.07f);
    private WaitForSeconds _waitBeforeGameBegins = new WaitForSeconds(1);

    private Coroutine _activeCoroutine;
    private bool _playEnabled = true;


    public IEnumerator StartGame(int firePlayer, int icePlayer, float lenght, float height)
    {
        yield return _waitBeforeGameBegins;
        if (CheckCanInit())
        {
            boardLenght = (int)lenght;
            boardHeight = (int)height;
            _firePlayer = firePlayer > 0 ? 1 : -1;

            if ((firePlayer != 0 && icePlayer == 0) || 
                (icePlayer != 0 &&  firePlayer == 0))
            {
                aiPlayers.Add(CreateAIPlayer(-1));
            }
            else if (firePlayer == 0 && icePlayer == 0)
            {
                aiPlayers.Add( CreateAIPlayer(1));
                aiPlayers.Add( CreateAIPlayer(-1));
            }
            
            yield return InitBoard();

            _activeCoroutine = StartCoroutine(AIWaitBeforePlaying());
        }
    }

    public void CleanBoard()
    {
        if(_activeCoroutine!= null)
            StopCoroutine(_activeCoroutine);
        _activeCoroutine = null;
        CurrentBoard = null;
        lastPlayer = -1;
        _firePlayer = 1;
        _playEnabled = true;
        Destroy(tokenContainer.gameObject);
        tokenContainer = new GameObject("TokenContainer").transform;
        Destroy(tilesContainer.gameObject);
        tilesContainer = new GameObject("TilesContainer").transform;
        Destroy(buttonsContainer.gameObject);
        buttonsContainer = new GameObject("ButtonContainer").transform;
        StartCoroutine(uiManager.Start());
    }
    
    
    private AIPlayer CreateAIPlayer(int playerNumber)
    {
        var aiPlayer =  Instantiate(aiPlayerPrefab).GetComponent<AIPlayer>();
        aiPlayer.InitAIPlayer(this,playerNumber:playerNumber);
        return aiPlayer;
    }

    public bool CheckCanInit()
    {
        if (boardLenght > 0 && boardHeight > 0 && tilePrefab!= null && buttonPrefab != null && 
            firePlayerToken != null && icePlayerToken != null && aiPlayerPrefab!= null && 
            mainCamera!= null && leftPlatform != null && rightPlatform!= null && uiManager!=null) 
        {
            return true;
        }
        Debug.Log("Cannot Init Board because some conditions are not valid");
        return false;
    }

    private IEnumerator InitBoard()
    {
        mainCamera.transform.position = new Vector3(boardLenght / 2,   boardHeight / 3, -10);
        int additionalSize = 0;
        if (boardHeight / boardLenght > 6 / 7)
            additionalSize = 1;

        mainCamera.GetComponent<Camera>().orthographicSize = CAMERA_SIZE_RELATION * boardLenght + additionalSize;
        CurrentBoard = new Board(boardLenght, boardHeight);
        for (int i = 0; i < boardLenght; i++)
        {
            int j = 0;
            for (; j < boardHeight; j++)
            {
                yield return _waitForSecondsBetweenTiles;
                Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, tilesContainer);
            }

            //We only create the buttons for the player if there is any player actually playing. If there are two AIs we dont.
            if (aiPlayers.Count <= 1)
            {
                var button = Instantiate(buttonPrefab, new Vector3(i, boardHeight / 2, -2), Quaternion.identity,
                    buttonsContainer);
                button.transform.localScale = new Vector3(1, boardHeight, 0.1f);
                button.GetComponent<AddTokenButton>().SetUp(column: i, boardManager: this);
            }
        }
        yield return uiManager.ShowBeingText();
    }

    public bool TryToAddToken(int column)
    {
        if (!_playEnabled) return false;
        if (column < boardLenght)
        {
            var row = CurrentBoard.ColumnHeight[column];
            if (row < boardHeight)
            {
                _playEnabled = false;
                var tokenPosition = CurrentBoard.GetIndexOfToken(column, row);
                CurrentBoard.BoardContent[tokenPosition] = lastPlayer * -1;
                CurrentBoard.ColumnHeight[column]++;

                var token = Instantiate(lastPlayer == _firePlayer ? icePlayerToken : firePlayerToken, new Vector3(column, boardHeight, -1),
                    Quaternion.identity, tokenContainer);
                StartCoroutine(token.GetComponent<PlayerToken>().MoveToPosition(row));
                lastPlayer *= -1;

                if (CheckAllMatches(tokenPosition, column, row, victoryThreshold, lastPlayer))
                {
                    CheckAllMatches(tokenPosition, column, row, victoryThreshold, lastPlayer, highlight:true);
                    _activeCoroutine = StartCoroutine(uiManager.ShowPlayerWinText(lastPlayer == -1 ? 1 : 2));
                }
                else
                {
                    _activeCoroutine = StartCoroutine(AIWaitBeforePlaying());
                    StartCoroutine(WaitBetweenPlays());
                }
                return true;
            }
        }
        return false;    
    }

    private IEnumerator WaitBetweenPlays()
    {
        yield return new WaitForSeconds(1);
        _playEnabled = true;
    }
    
    private IEnumerator AIWaitBeforePlaying()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (var aiPlayer in aiPlayers.Where(aiPlayer => aiPlayer.thisPlayer != lastPlayer))
        {
            aiPlayer.TryToPlay();
            break;
        }
    }

    
    public bool CheckAllMatches(int position, int column, int row, int treshold, int player, bool highlight = false)
    {
        return (CheckHorizontalMatches(position, column, row, player,highlight)* player >= treshold ||
                CheckVerticalMatches(position, column, row, player, highlight) * player >= treshold ||
                CheckDiagonalMatchesA(position, column, row, player, highlight) * player >= treshold ||
                CheckDiagonalMatchesB(position, column, row, player, highlight) * player >= treshold);
    }

    public int CheckHorizontalMatches(int position, int column, int row, int player, bool highlight = false)
    {
        var matchHorizontalValue = player;
        if (column > 0) matchHorizontalValue += CheckMatchesValue(position, CurrentBoard.GetIndexOfToken(0,row), 1, player, highlight);
        if (column < boardLenght) matchHorizontalValue += CheckMatchesValue(position, CurrentBoard.GetIndexOfToken(boardLenght-1,row), 1, player, highlight);
        return matchHorizontalValue;
    }
    
    public int CheckVerticalMatches(int position, int column,int row,int player, bool highlight = false)
    {
        var matchVerticalValue = player;
        if (row > 0 && row < boardHeight) matchVerticalValue += CheckMatchesValue(position, CurrentBoard.GetIndexOfToken(column,0), boardLenght, player, highlight);
        return matchVerticalValue;
    }

    public int CheckDiagonalMatchesA(int position, int column, int row,int player, bool highlight = false)
    {
        var matchDiagonalValueA = player;
        var diagonalIndexChange = boardLenght - 1;

        //Checks diagonal Left Up
        if (column > 0 && row < boardHeight - 1) matchDiagonalValueA += CheckMatchesValue(position, 
            position + Math.Min(column,boardHeight - row - 1) * (diagonalIndexChange), diagonalIndexChange, player, highlight);
        
        //Checks Diagonal Right Down
        if (column < boardLenght && row > 0) matchDiagonalValueA += CheckMatchesValue(position, 
            position - Math.Min(boardLenght - column - 1,row) * (diagonalIndexChange), diagonalIndexChange, player, highlight);
        return matchDiagonalValueA;
    }
    
    public int CheckDiagonalMatchesB(int position, int column, int row, int player, bool highlight = false)
    {
        var matchDiagonalValue = player;
        var diagonalIndexChange = boardLenght + 1;

        //Checks Diagonal Right Up
        if (column > 0 && row > 0)
            matchDiagonalValue += CheckMatchesValue(position,
                position + Math.Min(boardLenght - column - 1,boardHeight - row - 1) * diagonalIndexChange,diagonalIndexChange, player, highlight);
        //Checks Diagonal Left Down
        if (column < boardLenght && row < boardHeight) matchDiagonalValue+= CheckMatchesValue(position,
            position - Math.Min(column,row) * diagonalIndexChange,diagonalIndexChange,player, highlight);
        return matchDiagonalValue;
    }
  
    
    private int CheckMatchesValue(int origin, int destination, int indexChange, int playerValue, bool highlight = false)
    {
        var value = playerValue;
        var sign = origin > destination ? -1 : 1;
        var i = origin + indexChange * sign;

        var conditional = origin > destination ? new Func<bool>(() => i >= destination) : (() => i <= destination);
        
        for (;conditional(); i += indexChange * sign)
        {                
            var newValue = CurrentBoard.BoardContent[i] + value;
            if (Math.Abs(newValue) > Math.Abs(value))
            {
                if (highlight) Instantiate(playerValue == _firePlayer ? fireAnimation : iceAnimation, CurrentBoard.GetPositionOfTokenAtIndex(i),Quaternion.identity,tokenContainer);
                value = newValue;
            }
            else
            {
                break;
            }
        }
        
        return value-playerValue;
    }
    

    
    
    
}
