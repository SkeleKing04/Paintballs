using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIHandler : MonoBehaviour
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
        mainHandler.updateTextBox(new TextMeshProUGUI[] {textBoxes[0],textBoxes[1]}, new string[] {"Health: " + clientHealth.currentHealth.ToString(), "Paint: " + clientHealth.currentPaint.ToString()});
    }
}
