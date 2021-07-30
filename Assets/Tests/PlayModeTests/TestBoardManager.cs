using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestBoardManager
{
    private BoardManager _boardManager;
    
    [SetUp]
    public void Setup()
    {
        var boardManagerObject = new GameObject();
        _boardManager = boardManagerObject.AddComponent<BoardManager>();
        _boardManager.buttonPrefab = new GameObject();
        _boardManager.tilePrefab = new GameObject();
        _boardManager.firePlayerToken = new GameObject();
        _boardManager.icePlayerToken = new GameObject();
        _boardManager.buttonPrefab.AddComponent<AddTokenButton>();
        _boardManager.aiPlayer1 = new GameObject().AddComponent<AIPlayer>();
        _boardManager.aiPlayer2 = new GameObject().AddComponent<AIPlayer>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(_boardManager.buttonPrefab);
        Object.Destroy(_boardManager.tilePrefab);
        Object.Destroy(_boardManager.firePlayerToken);
        Object.Destroy(_boardManager.icePlayerToken);
        Object.Destroy(_boardManager.aiPlayer1.gameObject);
        Object.Destroy(_boardManager.aiPlayer2.gameObject);

        Object.Destroy(_boardManager.gameObject);
    }

    
    
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator WontInitWithInvalidHeight()
    {
        _boardManager.boardHeight = 0;
        
        Assert.IsFalse(_boardManager.CheckCanInit());
        
        yield return null;
    }
    
    
    [UnityTest]
    public IEnumerator WontInitWithInvalidLenght()
    {
        _boardManager.boardLenght = 0;
        
        Assert.IsFalse(_boardManager.CheckCanInit());
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator WontInitWithInvalidTilePrefab()
    {
        _boardManager.tilePrefab = null;
        
        Assert.IsFalse(_boardManager.CheckCanInit());
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator WontInitWithInvalidButtonPrefab()
    {
        _boardManager.buttonPrefab = null;
        
        Assert.IsFalse(_boardManager.CheckCanInit());
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator WontInitWithInvalidPlayer1Prefab()
    {
        _boardManager.firePlayerToken = null;
        
        Assert.IsFalse(_boardManager.CheckCanInit());
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator WontInitWithInvalidPlayer2Prefab()
    {
        _boardManager.icePlayerToken = null;
        
        Assert.IsFalse(_boardManager.CheckCanInit());
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator CantAddPiecesOverBoardHeight()
    {
        _boardManager.CurrentBoard.ColumnHeight[1] = _boardManager.boardHeight;
        
        Assert.IsFalse(_boardManager.TryToAddToken(column: 1));
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator CantAddPiecesOverBoardLenght()
    {
        Assert.IsFalse(_boardManager.TryToAddToken(column: 8));
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator CheckHorizontalMatchesGivesCorrectResult()
    {
        _boardManager.CurrentBoard.BoardContent[0] = 1;
        _boardManager.CurrentBoard.BoardContent[1] = 1;
        _boardManager.CurrentBoard.BoardContent[3] = 1;
        _boardManager.lastPlayer = 1;

        var horizontalMatches = _boardManager.CheckHorizontalMatches(2,2,0,1);
        
        Assert.IsTrue(horizontalMatches == 4);
        yield return null;
    }
    
    
    [UnityTest]
    public IEnumerator CheckVerticalMatchesGivesCorrectResult()
    {
        _boardManager.CurrentBoard.BoardContent[0] = 1;
        _boardManager.CurrentBoard.BoardContent[7] = 1;
        _boardManager.CurrentBoard.BoardContent[14] = 1;
        _boardManager.lastPlayer = 1;

        var verticalMatches = _boardManager.CheckVerticalMatches(21,0,3,1);
        
        Assert.IsTrue(verticalMatches == 4);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator CheckDiagonalAMatchesGivesCorrectResult()
    {
        _boardManager.CurrentBoard.BoardContent[6] = 1;
        _boardManager.CurrentBoard.BoardContent[12] = 1;
        _boardManager.CurrentBoard.BoardContent[24] = 1;
        _boardManager.lastPlayer = 1;

        var diagonalMatches = _boardManager.CheckDiagonalMatchesA(18,2,4,1);
        
        Assert.IsTrue(diagonalMatches == 4);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator CheckDiagonalBMatchesGivesCorrectResult()
    {
        _boardManager.CurrentBoard.BoardContent[0] = 1;
        _boardManager.CurrentBoard.BoardContent[8] = 1;
        _boardManager.CurrentBoard.BoardContent[24] = 1;
        _boardManager.lastPlayer = 1;

        var diagonalMatches = _boardManager.CheckDiagonalMatchesB(16,2,2,1);
        
        Assert.IsTrue(diagonalMatches == 4);
        yield return null;
    }
    
    
}
