using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    
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

    
    public Board CurrentBoard;
    public int lastPlayer = -1;
    private int _firePlayer = 1;
    private WaitForSeconds _waitForSecondsBetweenTiles = new WaitForSeconds(0.1f);
    private WaitForSeconds _waitBeforeGameBegins = new WaitForSeconds(1);


    public IEnumerator StartGame(int firePlayer, int icePlayer, float lenght, float height)
    {
        yield return _waitBeforeGameBegins;
        if (CheckCanInit())
        {
            boardLenght = (int)lenght;
            boardHeight = (int)height;
            yield return InitBoard();

            _firePlayer = firePlayer > 0 ? 1 : -1;
            
                if ((firePlayer != 0 && icePlayer == 0) || 
                    (icePlayer != 0 &&  firePlayer == 0))
                {
                    aiPlayers.Add(CreateAIPlayer(-1));
                    StartCoroutine(AIWaitBeforePlaying());
                }
                else if (firePlayer == 0 && icePlayer == 0)
                {
                    aiPlayers.Add( CreateAIPlayer(1));
                    aiPlayers.Add( CreateAIPlayer(-1));
                    StartCoroutine(AIWaitBeforePlaying());
                }
        }
    }

    private AIPlayer CreateAIPlayer(int playerNumber)
    {
        var aiPlayer =  Instantiate(aiPlayerPrefab).GetComponent<AIPlayer>();
        aiPlayer.InitAIPlayer(this,playerNumber:-1);
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
        mainCamera.transform.position = new Vector3(boardLenght / 2,  boardHeight / 2, -10);
        rightPlatform.transform.position = new Vector3(boardLenght, 0,0);
        leftPlatform.transform.position = Vector3.zero;
        CurrentBoard = new Board(boardLenght, boardHeight);
        for (int i = 0; i < boardLenght; i++)
        {
            int j = 0;
            for (; j < boardHeight; j++)
            {
                yield return _waitForSecondsBetweenTiles;
                Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
            }

            //We only create the buttons for the player if there is any player actually playing. If there are two AIs we dont.
            if (aiPlayers.Count <= 1)
            {
                var button = Instantiate(buttonPrefab, new Vector3(i, boardHeight / 2, -2), Quaternion.identity,
                    transform);
                button.transform.localScale = new Vector3(1, boardHeight, 0.1f);
                button.GetComponent<AddTokenButton>().SetUp(column: i, boardManager: this);
            }
        }
        yield return uiManager.ShowBeingText();
    }

    public bool TryToAddToken(int column)
    {
        if (column < boardLenght)
        {
            var row = CurrentBoard.ColumnHeight[column];
            if (row < boardHeight)
            {
                var checkerPosition = CurrentBoard.GetPositionOfChecker(column, row);
                CurrentBoard.BoardContent[checkerPosition] = lastPlayer * -1;
                CurrentBoard.ColumnHeight[column]++;

                var token = Instantiate(lastPlayer == _firePlayer ? icePlayerToken : firePlayerToken, new Vector3(column, boardHeight, -1),
                    Quaternion.identity, this.transform);
                StartCoroutine(token.GetComponent<PlayerToken>().MoveToPosition(row));
                lastPlayer *= -1;
                if (CheckAllMatches(checkerPosition, column, row, victoryThreshold, lastPlayer))
                {
                    StartCoroutine(uiManager.ShowPlayerWinText(lastPlayer == -1 ? 1 : 2));
                }
                else
                {
                    StartCoroutine(AIWaitBeforePlaying());    
                }
                return true;
            }
        }
        return false;    
    }
    
    private IEnumerator AIWaitBeforePlaying()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (var aiPlayer in aiPlayers.Where(aiPlayer => aiPlayer.thisPlayer != lastPlayer))
        {
            aiPlayer.TryToPlay();
        }
    }

    
    public bool CheckAllMatches(int position, int column, int row, int treshold, int player)
    {
        return (CheckHorizontalMatches(position, column, row, player)* player >= treshold ||
                CheckVerticalMatches(position, column, row, player) * player >= treshold ||
                CheckDiagonalMatchesA(position, column, row, player) * player >= treshold ||
                CheckDiagonalMatchesB(position, column, row, player) * player >= treshold);
    }

    public int CheckHorizontalMatches(int position, int column, int row, int player)
    {
        var matchHorizontalValue = player;
        if (column > 0) matchHorizontalValue += CheckMatchesValue(position, CurrentBoard.GetPositionOfChecker(0,row), 1, lastPlayer);
        if (column < boardLenght) matchHorizontalValue += CheckMatchesValue(position, CurrentBoard.GetPositionOfChecker(boardLenght-1,row), 1, lastPlayer);
        return matchHorizontalValue;
    }
    
    public int CheckVerticalMatches(int position, int column,int row,int player)
    {
        var matchVerticalValue = player;
        if (row > 0 && row < boardHeight) matchVerticalValue += CheckMatchesValue(position, CurrentBoard.GetPositionOfChecker(column,0), boardLenght, lastPlayer);
        return matchVerticalValue;
    }

    public int CheckDiagonalMatchesA(int position, int column, int row,int player)
    {
        var matchDiagonalValueA = player;
        var diagonalIndexChange = boardLenght - 1;

        //Checks diagonal Left Up
        if (column > 0 && row < boardHeight - 1) matchDiagonalValueA += CheckMatchesValue(position, 
            position + Math.Min(column,boardHeight - row - 1) * (diagonalIndexChange), diagonalIndexChange, lastPlayer);
        
        //Checks Diagonal Right Down
        if (column < boardLenght && row > 0) matchDiagonalValueA += CheckMatchesValue(position, 
            position - Math.Min(boardLenght - column - 1,row) * (diagonalIndexChange), diagonalIndexChange, lastPlayer);
        return matchDiagonalValueA;
    }
    
    public int CheckDiagonalMatchesB(int position, int column, int row,int player)
    {
        var matchDiagonalValue = player;
        var diagonalIndexChange = boardLenght + 1;

        //Checks Diagonal Right Up
        if (column > 0 && row > 0)
            matchDiagonalValue += CheckMatchesValue(position,
                position + Math.Min(boardLenght - column - 1,boardHeight - row - 1) * diagonalIndexChange,diagonalIndexChange, lastPlayer);
        //Checks Diagonal Left Down
        if (column < boardLenght && row < boardHeight) matchDiagonalValue+= CheckMatchesValue(position,
            position - Math.Min(column,row) * diagonalIndexChange,diagonalIndexChange,lastPlayer);
        return matchDiagonalValue;
    }
  
    
    private int CheckMatchesValue(int origin, int destination, int indexChange, int playerValue)
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
