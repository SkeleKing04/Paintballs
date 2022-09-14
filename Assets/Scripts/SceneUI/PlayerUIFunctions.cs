using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIFunctions : MonoBehaviour
{   
    public UIHandler mainHandler;
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
        mainHandler.updateTextBox(textBoxes[0], "Health: " + clientHealth.currentHealth.ToString());
        mainHandler.updateTextBox(textBoxes[1], "Paint: " + clientHealth.currentPaint.ToString());
    }
}
