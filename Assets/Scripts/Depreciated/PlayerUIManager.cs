using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject player;
    private MovementScript movement;
    private HealthHandler health;
    private PlayerShooting shooting;
    public TextMeshProUGUI textBox;
    // Start is called before the first frame update
    void Start()
    {
        movement = player.GetComponent<MovementScript>();
        health = player.GetComponent<HealthHandler>();
        shooting = player.GetComponent<PlayerShooting>();
    }

    // Update is called once per frame
    void Update()
    {
        textBox.text = "HPT: " + health.currentHealth.ToString() + "\nPNT: " + health.currentPaint.ToString();
    }
}
