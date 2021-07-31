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
        _boardManager.iceAnimation = new GameObject();
        _boardManager.fireAnimation = new GameObject();
        _boardManager.aiPlayerPrefab = new GameObject();
        _boardManager.mainCamera = new GameObject();
        _boardManager.leftPlatform = new GameObject();
        _boardManager.rightPlatform = new GameObject();
        _boardManager.uiManager = new GameObject().AddComponent<UIManager>();

    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(_boardManager.iceAnimation);
        Object.Destroy(_boardManager.fireAnimation);
        Object.Destroy(_boardManager.aiPlayerPrefab);
        Object.Destroy(_boardManager.mainCamera);
        Object.Destroy(_boardManager.leftPlatform);
        Object.Destroy(_boardManager.rightPlatform);
        Object.Destroy(_boardManager.uiManager);
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
    public IEnumerator WontInitWithInvalidFireAnimation()
    {
        _boardManager.fireAnimation = null;
        
        Assert.IsFalse(_boardManager.CheckCanInit());
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator WontInitWithInvalidIceAnimation()
    {
        _boardManager.iceAnimation = null;
        
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
