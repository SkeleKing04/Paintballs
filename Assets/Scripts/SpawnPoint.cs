using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public float coolDown, spawnSafeZone;
    public LayerMask combatantLayer;
    public bool spawnAvalible;
    private TeamManager teamManager;
    // Start is called before the first frame update
    void Start()
    {
        teamManager = GetComponent<TeamManager>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Combatant"))
        {
            if(Physics.CheckSphere(transform.position, spawnSafeZone, combatantLayer) && )
            {
                if(FindObjectOfType<DeathmatchScript>().teamDeathmatch && player.GetComponent<TeamManager>().teamColor != teamManager.teamColor)
                {
                    spawnAvalible = false;
                }
                else if(!FindObjectOfType<DeathmatchScript>().teamDeathmatch)
                {
                    spawnAvalible = false;
                }
            }
        }
    }
    public void doCoolDown(float time)
    {
        spawnAvalible = false;
        coolDown = time;
        while(coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }    
    }

}
