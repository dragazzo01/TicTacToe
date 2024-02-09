using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    public Slider dimNumSlide;
    public Slider gridNumSlide;
    public Slider playerNumSlide;
    public TextMeshProUGUI[] playNames;
    public TMP_Dropdown[] playTimes;

    public void GameCreation()
    {
        TransferData.dimNum = (int) dimNumSlide.value;
        TransferData.gridNum = (int) gridNumSlide.value;
        TransferData.playerNum = (int) playerNumSlide.value;
        TransferData.playerNames = new string[TransferData.playerNum];
        TransferData.playerTimes = new float[TransferData.playerNum];
        for (int i = 0; i < TransferData.playerNum; i++)
        {
            TransferData.playerNames[i] = playNames[i].text;
            TransferData.playerTimes[i] = float.Parse(playTimes[i].options[playTimes[i].value].text);
        }
        SceneManager.LoadScene("Main");
    }
}
