using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public int BoardHeight { get; }
    public int BoardLenght{ get; }
    public int[] BoardContent;
    public int[] ColumnHeight;

    public Board(int boardLenght, int boardHeight)
    {
        BoardLenght = boardLenght;
        BoardHeight = boardHeight;
        BoardContent = new int[boardHeight * boardLenght];
        ColumnHeight = new int[boardLenght];
    }

    public int GetPositionOfChecker(int column, int row)
    {
        if (!(column > BoardLenght || row > BoardHeight))
            return column + (row * BoardLenght);
        return -1;
    }
}
