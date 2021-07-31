using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Camera camera;
    
    private void Update()
    {
            var mousePosition = Input.mousePosition;
            mousePosition = camera.ScreenToWorldPoint(mousePosition);
            mousePosition.z = 10;
            transform.position = mousePosition;
    }
}

