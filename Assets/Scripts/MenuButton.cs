using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public void ResetButton()
    {
     GetComponent<Button>().enabled = true;
     GetComponentInChildren<ParticleSystem>().gameObject.SetActive(true);
    }
}
