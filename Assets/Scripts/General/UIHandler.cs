using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject[] gamePanel;
    public void selectUIPanels(int[] indexes)
    {
        foreach(GameObject panel in gamePanel)
        {
            panel.SetActive(false);
        }
        foreach(int index in indexes)
        {
            if(index < gamePanel.Length)
            {
                gamePanel[index].SetActive(true);
            }
        }
    }
    public void updateTextBox(TextMeshProUGUI[] textBoxes, string[] texts)
    {
        for (int i = 0; i < textBoxes.Length; i++)
        {
            textBoxes[i].text = texts[i];
        }
    }
    public void updateTogglable(Toggle[] toggles, bool[] values, bool[] changeToggle)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if(changeToggle[i]) toggles[i].isOn = values[i];
            else if(!changeToggle[i]) values[i] = toggles[i].isOn;
        }
    }
    public void updateSlider(Slider[] sliders, float[] values, bool[] changeSlider, bool[] phaseAsInt)
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            if(changeSlider[i])
            {
                if(values[i] >= sliders[i].minValue && values[i] <= sliders[i].maxValue) sliders[i].value = values[i];
            }
            else if(!changeSlider[i]) values[i] = sliders[i].value;
        }
    }
    
}
