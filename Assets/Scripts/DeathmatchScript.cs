using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathmatchScript : MonoBehaviour
{
    [Header("Players")]
    public List<GameObject> players;
    [Header("Spawning")]
    public Transform spawnArea;
    public float spawnSize;
    public float spawnSafeZone;
    public LayerMask combatantLayer;
    public Transform[] spawnPoints;
    [Header("Game Settings")]
    public bool teamDeathmatch;
    public int teamSize;
    public int botCount;
    public GameObject botPrefab;
    public bool fillRoomWithBots;
    public Color teamAColour, teamBColour;
    public float gameStartTime;
    [Header("Unsorted")]
    public Transform mapCenter;

    // Start is called before the first frame update
    void Start()
    {   
        teamAColour = new Color(Random.Range(0f, 256f)/255f,Random.Range(0f, 256f)/255f,Random.Range(0f, 256f)/255f,255f);
        teamBColour = new Color(1f - teamAColour.r,1f - teamAColour.g, 1f - teamAColour.b,255f);
        foreach(GameObject combatant in GameObject.FindGameObjectsWithTag("Combatant"))
        {
            players.Add(combatant);
        }
        if(fillRoomWithBots)
        {
            for(int i = botCount; i > 0; i--)
            {
                GameObject newBot = Instantiate(botPrefab, new Vector3(0,0,0), Quaternion.identity);
                players.Add(newBot);
                newBot.SetActive(false);
            }
        }
        sortTeams();
        startGame();
    }
    public static List<GameObject> playerShuffle (List<GameObject> incomingList)
    {
 
        System.Random rnd = new System.Random ();
 
        GameObject myGO;
 
        int n = incomingList.Count;
        for (int i = 0; i < n; i++)
        {
            // NextDouble returns a random number between 0 and 1.
            // ... It is equivalent to Math.random() in Java.
            int r = i + (int)(rnd.NextDouble() * (n - i));
            myGO = incomingList[r];
            incomingList[r] = incomingList[i];
            incomingList[i] = myGO;
        }
 
        return incomingList;
    }
    private void sortTeams()
    {
        players = playerShuffle(players);
        for(int i = 0; i < players.Count; i += 2)
        {
            players[i].GetComponent<TeamManager>().teamColor = teamAColour;
            players[i + 1].GetComponent<TeamManager>().teamColor = teamBColour;
        }
    }
    private void startGame()
    {
        Camera.main.GetComponentInParent<MoveCamera>().enabled = false;
        Camera.main.GetComponentInParent<Transform>().position = mapCenter.position;
        Camera.main.GetComponentInParent<Transform>().rotation = mapCenter.rotation;
        foreach(GameObject player in players)
        {
            player.SetActive(false);
        }
        float timeTillStart = gameStartTime;
        while (timeTillStart > 0)
        {
            timeTillStart -= Time.deltaTime;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void spawnMe(GameObject sender)
    {
        List<Transform> possibleSpawns = new List<Transform>();
        if(teamDeathmatch)
        {
            foreach(Transform spawnPoint in spawnPoints)
            {
                if(spawnPoint.GetComponent<TeamManager>().teamColor == sender.GetComponent<TeamManager>().teamColor && spawnPoint.GetComponent<SpawnPoint>().coolDown <= 0)
                {
                    possibleSpawns.Add(spawnPoint);
                }
            }
        }
        else
        {
             
        }
        
        int rnd = Random.Range(0, possibleSpawns.Count);
        if(Physics.CheckSphere(possibleSpawns[rnd].position, spawnSafeZone, combatantLayer))
        {
            sender.SetActive(true);
            sender.transform.position = possibleSpawns[rnd].position;
        }
    }
}
