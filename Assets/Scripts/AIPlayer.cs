using UnityEngine;


public class AIPlayer : MonoBehaviour
{
    private BoardManager _boardManager;
    [SerializeField] public int thisPlayer = -1;

    public void InitAIPlayer(BoardManager boardManager, int playerNumber)
    {
        _boardManager = boardManager;
        thisPlayer = playerNumber;
    }

    //This is the core of the AI. It will try to do several kinds of plays before just doing a random one.
    //First, it will try to Win, adding the last remaining token to go over the threshold.
    //If it cant win, it will try to stop the other player from winning
    //If it cant do that, it will try to increase its own tiles, adding one more to get to threshold-1
    //If it cant do that, it will try to stop the other player from doing the same
    //If it cant do that it will play the first available tile it finds.
    public void TryToPlay()
    {
        var column = TryToWin();
        var placedPiece = false;
        if (column > 0) _boardManager.TryToAddToken(column);
        else
        {
            column = TryToStopOpponentFromWinning();
            if (column > 0) _boardManager.TryToAddToken(column);
            else
            {
                column = TryToIncreaseOwnTiles();
                if (column > 0) _boardManager.TryToAddToken(column);
                else if (!_boardManager.TryToAddToken(TryToInterfereWithOpponent()))
                {
                    for (int i = 0; i < _boardManager.boardLenght; i++)
                    {
                        foreach (var columnHeight in _boardManager.CurrentBoard.ColumnHeight)
                        {
                            if (columnHeight < _boardManager.boardHeight )
                            {
                                placedPiece = _boardManager.TryToAddToken(i);
                                break;
                            }
                        }
                        if (placedPiece) break;
                    }

                    if (!placedPiece)
                    {
                        Debug.Log("NO MORE EMPTY SPACES");
                    }
                }
            }
        }
    }

    private int TryToWin()
    {
        return SimulatePlay(playerSign: 1, winConditionModifier: 0, defaultReturnValue:-1);
    }

    private int TryToStopOpponentFromWinning()
    {
        return SimulatePlay(playerSign: -1, winConditionModifier: 0, defaultReturnValue:-1);
    }

    private int TryToInterfereWithOpponent()
    {
        return SimulatePlay(playerSign: -1, winConditionModifier: 1, Random.Range(0, _boardManager.boardLenght));
    }
    
    private int TryToIncreaseOwnTiles()
    {
        return SimulatePlay(playerSign: 1, winConditionModifier: 1, -1);
    }

    //This method will call CheckAllMatches for each column that has space left, using the modifiers sent to see if a match can be made.
    //This is used to check if this player can win as well as if the other player can win, just changing the player sent to the method.
    private int SimulatePlay(int playerSign, int winConditionModifier, int defaultReturnValue)
    {
        var columnHeights = _boardManager.CurrentBoard.ColumnHeight;
        
        for (int i = 0; i < _boardManager.boardLenght; i++)
        {
            if (columnHeights[i] < _boardManager.boardHeight)
            {
                if (_boardManager.CheckAllMatches(_boardManager.CurrentBoard.GetIndexOfToken(i,columnHeights[i]), i, columnHeights[i], _boardManager.victoryThreshold-winConditionModifier, thisPlayer * playerSign))
                {
                    return i;
                }  
            }
        }
        return defaultReturnValue;
    }
    
}