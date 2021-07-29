using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerToken : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    
    
    private SpriteRenderer _spriteRenderer;
    
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = sprites[Random.Range(0,sprites.Length)];

    }

}
