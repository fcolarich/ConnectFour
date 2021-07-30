using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private GameObject particles;

    public void OnButtonPressed()
    {
     particles.SetActive(true);
     GetComponent<Button>().enabled = false;
    }
}
