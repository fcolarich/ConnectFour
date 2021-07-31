using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] public GameObject objectPrefab;

    private Queue<GameObject> _objectPool = new Queue<GameObject>();

    //We use this pool to store objects that will be reused when the game is restarted
    public GameObject GetObject(Vector3 position)
    {
        if (_objectPool.Count == 0) return Instantiate(objectPrefab, position, Quaternion.identity, transform);
        
        var poolObject = _objectPool.Dequeue();
        poolObject.transform.position = position;
        poolObject.SetActive(true);
        return poolObject;
    }

    //We store all objects and deactivate them, to be used again when the game needs them
    public void ReturnAllObjects()
    {
        foreach (Transform objects in transform)
        {
            _objectPool.Enqueue(objects.gameObject);
            objects.gameObject.SetActive(false);
        }
    }
    
}
