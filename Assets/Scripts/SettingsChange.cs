using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsChange : MonoBehaviour
{
    public Slider slider;
    public GameObject t;
    public GameObject[] playerObjects;
    public string intro;

    public void SliderChange()
    {
        t.GetComponent<TextMeshProUGUI>().text = intro + slider.value.ToString();
    }

    public void PlayerSliderChange()
    {
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].SetActive(i < slider.value);
        }
    }
}
