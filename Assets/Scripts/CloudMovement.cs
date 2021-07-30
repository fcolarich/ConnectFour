using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CloudMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0.05f;
    [SerializeField] private Camera mainCamera;
    private RectTransform _thisRect;
    private Bounds _canvasBounds;
    private Vector3 _originalPosition;
    private WaitForSeconds _waitForSeconds = new WaitForSeconds(1);
    

    void Start()
    {
        _thisRect = GetComponent<RectTransform>();
        _originalPosition = _thisRect.position;
        StartCoroutine(CheckIfVisible());
    }

    private void Update()
    {
        _thisRect.position += _thisRect.right * (speed * Time.deltaTime);
    }


    private IEnumerator CheckIfVisible()
    {
        float cameraPixelWidth = mainCamera.pixelWidth / 2;
        var rect = _thisRect.rect;


        var comparison = speed > 0 ? new Func<bool>(() => _thisRect.localPosition.x + rect.xMin > cameraPixelWidth) : 
            (() =>_thisRect.localPosition.x - _thisRect.rect.xMin < - cameraPixelWidth);
        
        while (true)
        {
            yield return _waitForSeconds;
            if (comparison())
            {
                _thisRect.localPosition = new Vector3(speed>0? rect.xMin-cameraPixelWidth:cameraPixelWidth-rect.xMin,_originalPosition.y+Random.Range(-50,50),_originalPosition.z);
            }
        }
    }
    


    
}
