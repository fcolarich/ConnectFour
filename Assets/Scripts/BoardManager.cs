using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const float CAMERA_SIZE_RELATION = 4.5f / 7f;
    private readonly WaitForSeconds WAIT_FOR_SECONDS_BETWEEN_TILES = new WaitForSeconds(0.07f);
    private readonly WaitForSeconds WAIT_BEFORE_GAME_BEGINS = new WaitForSeconds(1);

    
    [SerializeField] public int boardLenght = 7;
    [SerializeField] public int boardHeight = 5;
    [SerializeField] public int victoryThreshold = 4;
    [SerializeField] public GameObject aiPlayerPrefab;
    [SerializeField] public List<AIPlayer> aiPlayers = new List<AIPlayer>();
    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject leftPlatform;
    [SerializeField] public GameObject rightPlatform;
    [SerializeField] public UIManager uiManager;

    [SerializeField] public Transform animationsContainer;

    [SerializeField] public GameObject fireAnimation;
    [SerializeField] public GameObject iceAnimation;

    [SerializeField] public ObjectPool iceTokensPool;
    [SerializeField] public ObjectPool fireTokensPool;
    [SerializeField] public ObjectPool tilesPool;
    [SerializeField] public ObjectPool buttonsPool;

    [SerializeField] private GameObject iceIndicatorAnimation;
    [SerializeField] private GameObject fireIndicatorAnimation;
    [SerializeField] private GameObject mouseIceIndicatorAnimation;
    [SerializeField] private GameObject mouseFireIndicatorAnimation;

    
    public Board CurrentBoard;
    public int lastPlayer = -1;
    private int _firePlayer = 1;
    private Coroutine _activeCoroutine;
    private bool _playEnabled = false;

    
    //This is the method that starts the game with the cofiguration options set on the main menu
    //It will create AI players if no human player was assigned an element
    //And finally will start the AI coroutine so they can start playing.
    public IEnumerator StartGame(int firePlayer, int icePlayer, float lenght, float height, float winThreshold)
    {
        yield return WAIT_BEFORE_GAME_BEGINS;
        if (CheckCanInit())
        {
            boardLenght = (int)lenght;
            boardHeight = (int)height;
            _firePlayer = firePlayer > 0 ? 1 : -1;
            victoryThreshold = (int)winThreshold;

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

    //This method is used to clean the board after a game, to be able to restart without reloading the scene
    //Basically we restore variables to their starting value, clean lists and return objects to their pool.
    public void CleanBoard()
    {
        if(_activeCoroutine!= null)
            StopCoroutine(_activeCoroutine);
        _activeCoroutine = null;
        CurrentBoard = null;
        lastPlayer = -1;
        _firePlayer = 1;
        _playEnabled = false;
        foreach (var aiPlayer in aiPlayers)
        {
            Destroy(aiPlayer.gameObject);
        }
        TurnOffHUD();
        aiPlayers.Clear();
        fireTokensPool.ReturnAllObjects();
        iceTokensPool.ReturnAllObjects();
        tilesPool.ReturnAllObjects();
        buttonsPool.ReturnAllObjects();
        Destroy(animationsContainer.gameObject);
        animationsContainer = new GameObject("AnimationsContainer").transform;
        StartCoroutine(uiManager.Start());
    }
    
    //This method creates a new AI player that will play with the playerNumber assigned
    private AIPlayer CreateAIPlayer(int playerNumber)
    {
        var aiPlayer =  Instantiate(aiPlayerPrefab).GetComponent<AIPlayer>();
        aiPlayer.InitAIPlayer(this,playerNumber:playerNumber);
        return aiPlayer;
    }

    //This is the pre-init check to see if all the required components are present
    public bool CheckCanInit()
    {
        if (boardLenght > 0 && boardHeight > 0 && aiPlayerPrefab!= null && 
            mainCamera!= null && leftPlatform != null && rightPlatform!= null && uiManager!=null) 
        {
            return true;
        }
        Debug.Log("Cannot Init Board because some conditions are not valid");
        return false;
    }

    
    //With this method we initialize the board, that is, set the camera size according to the size of the board
    //Create the tiles in their correct positions
    //Create the buttons for the player to place their tokens (if there are any human players)
    //Finally the BEGIN text is shown and the game begins.
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
                yield return WAIT_FOR_SECONDS_BETWEEN_TILES;
                tilesPool.GetObject(new Vector3(i, j, 0));
            }

            //We only create the buttons for the player if there is any player actually playing. If there are two AIs we dont.
            //The buttons cover all the surface of the column, so the player can click anywhere and a token will appear in that column
            if (aiPlayers.Count <= 1)
            {
                var button = buttonsPool.GetObject(new Vector3(i, boardHeight / 2, -2));
                button.transform.localScale = new Vector3(1, boardHeight, 0.1f);
                button.GetComponent<AddTokenButton>().SetUp(column: i, boardManager: this);
            }
        }
        yield return uiManager.ShowBeingText();
        _playEnabled = true;
    }

    
    //This method tries to add a token in a given column, checking first if the play is enabled, if the column is valid, and if its not full.
    //It creates the correct token for each player element and after each token added, checks if the game was won or not.
    //If the game was won, it highlights the winner combination and shows the player win text and buttons
    //If it wasn't won, It will check if there are any available columns to drop new pieces, if there arent, it will annouce a DRAW
    //If there are, it will tell the AI to try to play and set the timer for the waitBetweenPlays.
    
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

                var token = lastPlayer == _firePlayer
                    ? iceTokensPool.GetObject(new Vector3(column, boardHeight, -1))
                    : fireTokensPool.GetObject(new Vector3(column, boardHeight, -1));
                StartCoroutine(token.GetComponent<PlayerToken>().MoveToPosition(row));
                lastPlayer *= -1;

                if (CheckAllMatches(tokenPosition, column, row, victoryThreshold, lastPlayer))
                {
                    CheckAllMatches(tokenPosition, column, row, victoryThreshold, lastPlayer, highlight:true);
                    Instantiate(lastPlayer == _firePlayer ? fireAnimation : iceAnimation, new Vector3(column,row),Quaternion.identity,animationsContainer);
                    _activeCoroutine = StartCoroutine(uiManager.ShowPlayerWinText(lastPlayer == -1 ? 2 : 1));
                }
                else
                {
                    if (!CurrentBoard.ColumnHeight.ToList().Exists(c => c < boardHeight))
                    {
                        _activeCoroutine = StartCoroutine(uiManager.ShowPlayerWinText(0));
                    }
                    else
                    {
                        _activeCoroutine = StartCoroutine(AIWaitBeforePlaying());
                        StartCoroutine(WaitBetweenPlays());    
                    }
                    
                }
                return true;
            }
        }
        return false;    
    }

    //We use this to give a little time between plays
    //We also call from here the method that lights up the HUD and the mouse when its each player turn.
    private IEnumerator WaitBetweenPlays()
    {
        yield return new WaitForSeconds(0.5f);
        _playEnabled = true;
        LightUpHUDOnPlayerTurn();
    }

    //This method turns on the animations in the HUD and the mouse to indicate which player turn is it.
    private void LightUpHUDOnPlayerTurn()
    {
            iceIndicatorAnimation.SetActive(lastPlayer == _firePlayer);
            mouseIceIndicatorAnimation.SetActive(lastPlayer == _firePlayer);
            fireIndicatorAnimation.SetActive(lastPlayer != _firePlayer);
            mouseFireIndicatorAnimation.SetActive(lastPlayer != _firePlayer);
    }

    private void TurnOffHUD()
    {
        iceIndicatorAnimation.SetActive(false);
        mouseIceIndicatorAnimation.SetActive(false);
        fireIndicatorAnimation.SetActive(false);
        mouseFireIndicatorAnimation.SetActive(false);

    }
    
    
    //This method makes AIs seem like they are thinking before playing, giving a delay between plays.
    private IEnumerator AIWaitBeforePlaying()
    {
        
        yield return new WaitForSeconds(1.5f);
        foreach (var aiPlayer in aiPlayers.Where(aiPlayer => aiPlayer.thisPlayer != lastPlayer))
        {
            aiPlayer.TryToPlay();
            break;
        }
    }

    //This method will check if any of the matches in each direction is equal or over the win treshold sent to it.
    public bool CheckAllMatches(int position, int column, int row, int treshold, int player, bool highlight = false)
    {
        return (CheckHorizontalMatches(position, column, row, player,highlight)* player >= treshold ||
                CheckVerticalMatches(position, column, row, player, highlight) * player >= treshold ||
                CheckDiagonalMatchesA(position, column, row, player, highlight) * player >= treshold ||
                CheckDiagonalMatchesB(position, column, row, player, highlight) * player >= treshold);
    }

    //The methods below will try to check for matches in every direction, but only if its needed. They will send the origin and destination, as well as the change in the index to check.
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
  
    //This method will check for matches in the direction indicated, starting from the origin and adding the index change sent and checking if the value stored
    //in the board added to the player number increases or not the match value. If it increases means there was a token of the same player in that tile and
    //that there is a match. If it doesn't increase or decreases, means that there was not a token or a token from the other player. In that case the lookup finishes and
    //the value so far is returned.
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
                if (highlight) Instantiate(playerValue == _firePlayer ? fireAnimation : iceAnimation, CurrentBoard.GetPositionOfTokenAtIndex(i),Quaternion.identity,animationsContainer);
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
