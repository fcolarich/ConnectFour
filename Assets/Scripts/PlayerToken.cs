using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerToken : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private GameObject impactParticleObject;
    [SerializeField] private AudioClip[] audios;
    [SerializeField] private AudioClip[] impactAudios;
    
    private SpriteRenderer _spriteRenderer;
    private WaitForEndOfFrame _waitForEndOfFrame;
    private AudioSource _audioSource;

    private void Awake()
    {       
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        impactParticleObject.SetActive(false);
    }
    
    
    private void OnEnable()
    {
        _spriteRenderer.sprite = sprites[Random.Range(0,sprites.Length)];
        _audioSource.clip = audios[Random.Range(0,audios.Length)];
        _audioSource.Play();
    }

    public IEnumerator MoveToPosition(int position)
    {
        while (transform.position.y > position)
        {
            transform.position += Vector3.down * (speed * Time.deltaTime);
            yield return _waitForEndOfFrame;
        }
        impactParticleObject.SetActive(true);
        _audioSource.clip = impactAudios[Random.Range(0,impactAudios.Length)];
        _audioSource.Play();
    }


}
