using System;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Texture2D mouseTexture;

    private void Start()
    {
        Cursor.SetCursor(mouseTexture,Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
            var mousePosition = Input.mousePosition;
            mousePosition = camera.ScreenToWorldPoint(mousePosition);
            mousePosition.z = 10;
            transform.position = mousePosition;
    }
}

