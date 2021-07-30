using TMPro;
using UnityEngine;

public class MusicToggleText : MonoBehaviour
{
   private TMP_Text _text;

   private void Start()
   {
      _text = GetComponentInChildren<TMP_Text>();
   }
   
   public void OnToggle()
   {
      var music = PlayerPrefs.GetInt("music", 1);
      music = -music;
      PlayerPrefs.SetInt("music", music);
      _text.text = $"MUSIC IS {(music > 0 ? "ON" : "OFF")}";
   }
}
