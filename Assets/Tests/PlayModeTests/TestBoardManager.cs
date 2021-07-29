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
        _boardManager.player1Token = new GameObject();
        _boardManager.player2Token = new GameObject();
    }

    [TearDown]
    public void Teardown()
    {
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
        _boardManager.player1Token = null;
        
        Assert.IsFalse(_boardManager.CheckCanInit());
        
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator WontInitWithInvalidPlayer2Prefab()
    {
        _boardManager.player2Token = null;
        
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
        Assert.IsFalse(_boardManager.TryToAddToken(column: 6));
        yield return null;
    }
    
}
