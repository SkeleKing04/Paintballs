using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIFunctions : MonoBehaviour
{   
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
        textBoxes[0].text = "Health: " + clientHealth.currentHealth.ToString();
        textBoxes[1].text = "Paint: " + clientHealth.currentPaint.ToString();
    }
}
