using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public float coolDown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void doCoolDown(float time)
    {
        coolDown = time;
        while(coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
    }
}
