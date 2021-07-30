using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlidersUpdate : MonoBehaviour
{
    
    private TMP_Text _text;
    private string _originalText;


    private void Start()
    {
        _text = GetComponentInChildren<TMP_Text>();
        _originalText = _text.text;
        _text.text = _originalText + GetComponentInChildren<Slider>().value;
    }

    public void OnSliderUpdate()
    {
        _text.text = _originalText + GetComponentInChildren<Slider>().value;
    }
    
}
