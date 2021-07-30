using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private GameObject particles;

    public void OnButtonPressed()
    {
     particles.SetActive(true);   
    }
}
