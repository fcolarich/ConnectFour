using UnityEngine;


public class AIPlayer : MonoBehaviour
{
    private BoardManager _boardManager;
    [SerializeField] public int thisPlayer = -1;
    [SerializeField] private int placePieceRetries = 10;


    public void InitAIPlayer(BoardManager boardManager, int playerNumber)
    {
        _boardManager = boardManager;
        thisPlayer = playerNumber;
    }

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
                    for (int i = 0; i < placePieceRetries; i++)
                    {
                        foreach (var columnHeight in _boardManager.CurrentBoard.ColumnHeight)
                        {
                            if (columnHeight < _boardManager.boardHeight -1 )
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