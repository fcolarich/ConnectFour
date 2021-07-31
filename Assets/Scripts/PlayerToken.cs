using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerToken : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private GameObject impactParticleObject;
    
    private SpriteRenderer _spriteRenderer;
    private WaitForEndOfFrame _waitForEndOfFrame;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = sprites[Random.Range(0,sprites.Length)];
    }

    public IEnumerator MoveToPosition(int position)
    {
        while (transform.position.y > position)
        {
            transform.position += Vector3.down * (speed * Time.deltaTime);
            yield return _waitForEndOfFrame;
        }
        impactParticleObject.SetActive(true);
        

    }
    

}
