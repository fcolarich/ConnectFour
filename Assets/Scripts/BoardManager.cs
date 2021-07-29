using UnityEngine;

public class BoardManager : MonoBehaviour
{
    
    [SerializeField] public int boardLenght = 7;
    [SerializeField] public int boardHeight = 5;
    [SerializeField] public GameObject tilePrefab;
    [SerializeField] public GameObject buttonPrefab;
    [SerializeField] public GameObject player1Token;
    [SerializeField] public GameObject player2Token;
    
    public Board CurrentBoard;
    public int LastPlayer = -1;
    
    void Start()
    {
        if (CheckCanInit()) InitBoard();
    }

    public bool CheckCanInit()
    {
        if (boardLenght > 0 && boardHeight > 0 && tilePrefab!= null && buttonPrefab != null && player1Token != null && player2Token != null) 
        {
            return true;
        }

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
                CurrentBoard.BoardContent[CurrentBoard.GetPositionOfChecker(column, row)] = LastPlayer * -1;
                CurrentBoard.ColumnHeight[column]++;

                Instantiate(LastPlayer == 1 ? player2Token : player1Token, new Vector3(column, row, -1),
                    Quaternion.identity, this.transform);
                LastPlayer *= -1;
                return true;
            }
        }

        return false;    
    }

    
    
    
}
