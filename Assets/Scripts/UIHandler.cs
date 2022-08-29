using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public GameObject[] gamePanel;
    public GameObject clientGameObject;
    private HealthHandler clientHealth;
    public TextMeshProUGUI[] textBoxes;
    public TextMeshProUGUI timerText;
    // Start is called before the first frame update
    void Start()
    {
        clientHealth = clientGameObject.GetComponent<HealthHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        updateTextBox(new int[] {0,1}, new string[] {"Health: " + clientHealth.currentHealth.ToString(), "Paint: " + clientHealth.currentPaint.ToString()});
    }
    public void updateTextBox(int[] indexes, string[] text)
    {
        for(int i = 0; i < indexes.Length; i++)
        {
            if(indexes[i] < textBoxes.Length)
            {
                textBoxes[indexes[i]].text = text[i];
            }

        }
    }
    public void ResetUIPanels(int[] indexes)
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
}
