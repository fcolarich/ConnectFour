using System;
using System.Collections;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    
    [SerializeField] public int boardLenght = 7;
    [SerializeField] public int boardHeight = 5;
    [SerializeField] public int victoryThreshold = 4;
    [SerializeField] public GameObject tilePrefab;
    [SerializeField] public GameObject buttonPrefab;
    [SerializeField] public GameObject player1Token;
    [SerializeField] public GameObject player2Token;
    [SerializeField] public AIPlayer aiPlayer1;
    [SerializeField] public AIPlayer aiPlayer2;

    
    public Board CurrentBoard;
    public int LastPlayer = -1;
    
    void Start()
    {
        if (CheckCanInit())
        {
            InitBoard();
            aiPlayer1.InitAIPlayer(this,playerNumber:1);
            aiPlayer2.InitAIPlayer(this,playerNumber:-1);
            aiPlayer2.gameObject.SetActive(false);
            StartCoroutine(AIWaitBeforePlaying());
        }
    }

    public bool CheckCanInit()
    {
        if (boardLenght > 0 && boardHeight > 0 && tilePrefab!= null && buttonPrefab != null && player1Token != null && player2Token != null && aiPlayer1!= null && aiPlayer2 != null) 
        {
            return true;
        }

        Debug.Log("Cannot Init Board because some conditions are not valid");
        return false;
    }

    private void InitBoard()
    {
        CurrentBoard = new Board(boardLenght, boardHeight);
        for (int i = 0; i < boardLenght; i++)
        {
            int j = 0;
            for (; j < boardHeight; j++)
            {
                Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
            }
            var button = Instantiate(buttonPrefab, new Vector3(i, boardHeight/2, -2), Quaternion.identity, transform);
            button.transform.localScale = new Vector3(1, boardHeight, 0.1f);
            button.GetComponent<AddTokenButton>().SetUp(column:i, boardManager:this);
        }
    }
    
    
    public bool TryToAddToken(int column)
    {
        if (column < boardLenght)
        {
            var row = CurrentBoard.ColumnHeight[column];
            if (row < boardHeight)
            {
                var checkerPosition = CurrentBoard.GetPositionOfChecker(column, row);
                CurrentBoard.BoardContent[checkerPosition] = LastPlayer * -1;
                CurrentBoard.ColumnHeight[column]++;

                Instantiate(LastPlayer == 1 ? player2Token : player1Token, new Vector3(column, row, -1),
                    Quaternion.identity, this.transform);
                LastPlayer *= -1;
                if (CheckAllMatches(checkerPosition, column, row, victoryThreshold, LastPlayer))
                {
                    Debug.Log($"Player {(LastPlayer == -1? 1 : 2 )} has WON!!");
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
        yield return new WaitForSeconds(0.5f);
        if (LastPlayer == 1)
        {
            if (aiPlayer2.gameObject.activeSelf)
                aiPlayer2.TryToPlay();
        }
        else if (aiPlayer1.gameObject.activeSelf)
        {
            aiPlayer1.TryToPlay();
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
        if (column > 0) matchHorizontalValue += CheckMatchesValue(position, CurrentBoard.GetPositionOfChecker(0,row), 1, LastPlayer);
        if (column < boardLenght) matchHorizontalValue += CheckMatchesValue(position, CurrentBoard.GetPositionOfChecker(boardLenght-1,row), 1, LastPlayer);
        return matchHorizontalValue;
    }
    
    public int CheckVerticalMatches(int position, int column,int row,int player)
    {
        var matchVerticalValue = player;
        if (row > 0 && row < boardHeight) matchVerticalValue += CheckMatchesValue(position, CurrentBoard.GetPositionOfChecker(column,0), boardLenght, LastPlayer);
        return matchVerticalValue;
    }

    public int CheckDiagonalMatchesA(int position, int column, int row,int player)
    {
        var matchDiagonalValueA = player;
        var diagonalIndexChange = boardLenght - 1;

        //Checks diagonal Left Up
        if (column > 0 && row < boardHeight - 1) matchDiagonalValueA += CheckMatchesValue(position, 
            position + Math.Min(column,boardHeight - row - 1) * (diagonalIndexChange), diagonalIndexChange, LastPlayer);
        
        //Checks Diagonal Right Down
        if (column < boardLenght && row > 0) matchDiagonalValueA += CheckMatchesValue(position, 
            position - Math.Min(boardLenght - column - 1,row) * (diagonalIndexChange), diagonalIndexChange, LastPlayer);
        return matchDiagonalValueA;
    }
    
    public int CheckDiagonalMatchesB(int position, int column, int row,int player)
    {
        var matchDiagonalValue = player;
        var diagonalIndexChange = boardLenght + 1;

        //Checks Diagonal Right Up
        if (column > 0 && row > 0)
            matchDiagonalValue += CheckMatchesValue(position,
                position + Math.Min(boardLenght - column - 1,boardHeight - row - 1) * diagonalIndexChange,diagonalIndexChange, LastPlayer);
        //Checks Diagonal Left Down
        if (column < boardLenght && row < boardHeight) matchDiagonalValue+= CheckMatchesValue(position,
            position - Math.Min(column,row) * diagonalIndexChange,diagonalIndexChange,LastPlayer);
        return matchDiagonalValue;
    }
  
    
    private int CheckMatchesValue(int origin, int destination, int indexChange, int playerValue)
    {
        var value = playerValue;
        var sign = origin > destination ? -1 : 1;
        int i = origin + indexChange * sign;

        Func<bool> conditional = origin > destination ? new Func<bool>(() => i >= destination) : (() => i <= destination);
        
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
