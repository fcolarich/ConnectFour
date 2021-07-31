using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private GameObject particleObject;
    
    public void ResetButton()
    {
     GetComponent<Button>().enabled = true;
     particleObject.SetActive(false);
    }
}
