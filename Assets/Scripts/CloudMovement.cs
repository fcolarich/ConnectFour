using UnityEngine;
using UnityEngine.UI;

public class CloudMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0.05f;
    [SerializeField] private RectTransform screenRect;
    private RectTransform _thisRect;
    private Vector3 _originalPosition;

    void Start()
    {
        _thisRect = GetComponent<RectTransform>();
        _originalPosition = _thisRect.position;
    }

    private void Update()
    {
        _thisRect.position += _thisRect.right * (speed * Time.deltaTime);
    }

    
}
