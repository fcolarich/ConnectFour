using UnityEngine;

public class BoardManager : MonoBehaviour
{
    
    [SerializeField] public int boardLenght = 7;
    [SerializeField] public int boardHeight = 5;
    [SerializeField] public GameObject tilePrefab;
    [SerializeField] public GameObject buttonPrefab;
    [SerializeField] public GameObject player1Token;
    [SerializeField] public GameObject player2Token;
    
    private Board _currentBoard;
    
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
        _currentBoard = new Board(boardLenght, boardHeight);
        for (int i = 0; i < boardLenght; i++)
        {
            int j = 0;
            for (; j < boardHeight; j++)
            {
                Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity, transform);
            }
            Instantiate(buttonPrefab, new Vector3(i, j, -1), Quaternion.identity, transform);
        }
    }
    
    
    
}
