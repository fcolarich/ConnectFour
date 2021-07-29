using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTokenButton : MonoBehaviour
{
    private int _column;
    private BoardManager _boardManager;


    public void SetUp(int column,BoardManager boardManager)
    {
        _column = column;
        _boardManager = boardManager;
    }
    
    private void OnMouseDown()
    {
        _boardManager.TryToAddToken(_column);
    }
}
