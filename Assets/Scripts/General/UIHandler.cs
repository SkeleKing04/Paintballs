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
    public void updateTextBox(TextMeshProUGUI textBox, string text)
    {
        textBox.text = text;
    }
    public void updateTogglable(Toggle toggles, bool values, bool changeToggle)
    {
        if(changeToggle) toggles.isOn = values;
        else if(!changeToggle) values = toggles.isOn;
    }
    public void updateSlider(Slider slider, float value, bool changeSlider, bool phaseAsInt)
    {
        if(changeSlider)
        {
            if(value >= slider.minValue && value <= slider.maxValue)
            {
                if(!phaseAsInt)
                {
                    slider.value = value;
                }
                else if(phaseAsInt)
                {
                    slider.value = (int)value;
                }
            }
        }
        else if(!changeSlider) 
        {
            if(!phaseAsInt)
            {
                value = slider.value;
            }
            else if(phaseAsInt)
            {
                value = (int)slider.value;
            }
        }
    }
}
