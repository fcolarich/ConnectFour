using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private GameObject particles;

    public void OnButtonPressed()
    {
     particles.SetActive(true);
     this.GetComponent<Button>().enabled = false;
    }
}
