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
    public bool CheckSpawn(GameObject sender)
    {
        bool current = false;
        if(coolDown > 0)
        {
            Debug.Log("The spawn, " + gameObject.name + " is currently on cooldown. Please wait " + coolDown.ToString() + " more seconds.");
            current = false;
        }
        else
        {
                Debug.Log(sender.name + " is " + (sender.transform.position - gameObject.transform.position).magnitude + " units from this spawn point, " + gameObject.name);
                if(DeathmatchScript.teamDeathmatch)
                {
                    if(sender.GetComponent<TeamManager>().teamColor != gameObject.GetComponent<TeamManager>().teamColor && (sender.transform.position - gameObject.transform.position).magnitude <= spawnSafeZone)
                    {
                        current = false;
                    }

                }
                else if((sender.transform.position - gameObject.transform.position).magnitude <= spawnSafeZone)
                {
                    current = false;
                }
                else
                {
                    current = true;
                }
        }
        return current;
    }
    

}
