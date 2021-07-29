using UnityEngine;

public class BoardManager : MonoBehaviour
{
    
    [SerializeField] public int boardLenght = 7;
    [SerializeField] public int boardHeight = 5;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject buttonPrefab;

    private Board _currentBoard;
    void Start()
    {
        SetupBoard();
    }

    private void SetupBoard()
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
