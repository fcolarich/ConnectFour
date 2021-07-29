using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class TestBoard
{
    // A Test behaves as an ordinary method
    [Test]
    public void BoardReturnsNullValueWhenAskingForInvalidPositionInColumn()
    {
        var board = new Board(5,5);
        var value = board.GetPositionOfChecker(6, 1);
        UnityEngine.Assertions.Assert.IsTrue(value == -1);
    }
    
    
    [Test]
    public void BoardReturnsNullValueWhenAskingForInvalidPositionInRow()
    {
        var board = new Board(5,5);
        var value = board.GetPositionOfChecker(1, 6);
        UnityEngine.Assertions.Assert.IsTrue(value == -1);
    }
    
    
    [Test]
    public void BoardReturnsCorrectValueWhenAskingForValidPosition()
    {
        var board = new Board(5,5);
        var value = board.GetPositionOfChecker(2, 4);
        UnityEngine.Assertions.Assert.IsTrue(value == 22);
    }
}
