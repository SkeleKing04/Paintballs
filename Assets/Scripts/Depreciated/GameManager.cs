using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class playerData
    {
        public GameObject playerObject;
        public GameObject scorecardObject;
        public float personalScore;
    }
    public playerData[] data;
    // Start is called before the first frame update
    void Start()
    {
        checkClassVars();
    }

    // Update is called once per frame
    void Update()
    {
        data[IndexPlayer(gameObject)].playerObject = null;
    }
    public void checkClassVars()
    {
        foreach(var value in data)
        {
            Debug.Log(value.ToString());
        }
    }
    public int IndexPlayer(GameObject incomingObject)
    {
        for(int i = 0; i <= data.Length - 1; i++)
        {
            if(data[i].playerObject == incomingObject)
            {
                return i;
            }
        }
        return -1;
    }
}
