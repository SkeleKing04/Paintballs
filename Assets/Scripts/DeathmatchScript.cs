using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class DeathmatchScript : MonoBehaviour
{
    public enum gameState
    {
        setup,
        starting,
        loading,
        playing,
        ending,
        restarting,
        leaving
    };
    public gameState state;
    [Header("Players")]
    //public List<GameObject> players;
    [Header("Spawning")]
    //public Transform spawnArea;
    public float spawnSize, spawnSafeZone;
    public LayerMask combatantLayer;
    public Transform[] spawnPoints;
    [Header("Game Settings")]
    public bool teamDeathmatch;
    public int soloScoreCap, teamScoreCap,teamSize, botCount;
    public GameObject botPrefab;
    public bool fillRoomWithBots;
    public Color teamAColour, teamBColour;
    public float gameStartTime;
    [Header("Unsorted")]
    public Transform mapCenter;
    public new Transform camera;
    public GameObject[] gamePanel;
    public TextMeshProUGUI timerText;
    public GameObject playerScorecard;
    //private List<GameObject> scorecards;
    //private List<float> scores;
    public float timeTillStart;
    public List<playerData> data;
    [System.Serializable]
    public class playerData
    {
        public string playerName;
        public GameObject playerObject;
        public GameObject personalScorecard;
        public float personalScore;
    }

    // Start is called before the first frame update
    void Start()
    {   
        //data = new playerData[teamSize * 2];
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        state = gameState.setup;
        
    }
    private void addNewPlayer(playerData dataPoint, GameObject incomingObject)
    {
        dataPoint.playerName = "yooo";
        dataPoint.playerObject = incomingObject;
        dataPoint.personalScorecard = createScorecard(dataPoint.playerObject);
        dataPoint.personalScore = 0;
    }

    public static List<playerData> playerShuffle (List<playerData> incomingList)
    {
        System.Random rnd = new System.Random ();
 
        playerData dataPoint;
 
        int n = incomingList.Count;
        for (int i = 0; i < n; i++)
        {
            // NextDouble returns a random number between 0 and 1.
            // ... It is equivalent to Math.random() in Java.
            int r = i + (int)(rnd.NextDouble() * (n - i));
            dataPoint = incomingList[r];
            incomingList[r] = incomingList[i];
            incomingList[i] = dataPoint;
        }
 
        return incomingList;
    }
    public void resetPlayerCam(bool toggle, Transform posToMove)
    {
        camera.GetComponent<MoveCamera>().enabled = toggle;
        if(posToMove != null)
        {
            camera.GetComponent<Transform>().position = posToMove.position;
            camera.GetComponent<Transform>().rotation = posToMove.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        stateEventHandler();
    }
    private void stateEventHandler()
    {
        switch(state)
        {
            case gameState.setup:
                teamAColour = new Color(UnityEngine.Random.Range(0f, 256f)/255f,UnityEngine.Random.Range(0f, 256f)/255f,UnityEngine.Random.Range(0f, 256f)/255f,255f);
                teamBColour = new Color(1f - teamAColour.r,1f - teamAColour.g, 1f - teamAColour.b,255f);
                GameObject[] allCombatants = GameObject.FindGameObjectsWithTag("Combatant");
                Debug.Log("Found combantants length @ " + allCombatants.Length.ToString());
                for(int i = 0; i <= allCombatants.Length - 1; i++)
                {
                    Debug.Log(allCombatants[i].name.ToString());
                    data.Add(new playerData());
                    addNewPlayer(data[i], allCombatants[i]);
                }
                if(fillRoomWithBots)
                {
                    botCount = Mathf.FloorToInt(teamSize * 2 - data.Count);
                    for(int i = 0; i < botCount; i++)
                    {
                        GameObject newBot = Instantiate(botPrefab, new Vector3(0,0,0), Quaternion.identity);
                        data.Add(new playerData());
                        addNewPlayer(data[data.Count - 1], newBot);
                        newBot.SetActive(false);
                    }
                }
                resetPlayerCam(false, mapCenter);
                if(teamDeathmatch)
                {
                    data = playerShuffle(data);
                    for(int i = 0; i < data.Count; i += 2)
                    {
                        data[i].playerObject.GetComponent<TeamManager>().teamColor = teamAColour;
                        data[i].playerObject.GetComponent<TeamManager>().UpdateColour();
                        data[i + 1].playerObject.GetComponent<TeamManager>().teamColor = teamBColour;
                        data[i + 1].playerObject.GetComponent<TeamManager>().UpdateColour();
                    }
                    for(int i = 0; i < spawnPoints.Length / 2; i++)
                    {
                        spawnPoints[i].GetComponent<TeamManager>().teamColor = teamAColour;
                        spawnPoints[i + spawnPoints.Length / 2].GetComponent<TeamManager>().teamColor = teamBColour;
                    }
                }
                else
                {
                    foreach(playerData dataPoint in data)
                    {
                        dataPoint.playerObject.GetComponent<TeamManager>().teamColor = new Color(UnityEngine.Random.Range(0f, 256f)/255f,UnityEngine.Random.Range(0f, 256f)/255f,UnityEngine.Random.Range(0f, 256f)/255f,255f);
                        dataPoint.playerObject.GetComponent<TeamManager>().UpdateColour();
                        dataPoint.personalScorecard.transform.GetComponent<Image>().color = dataPoint.playerObject.GetComponent<TeamManager>().teamColor;
                        dataPoint.playerObject.SetActive(false);
                    }
                }
                timeTillStart = gameStartTime;
                state = gameState.starting;
                break;
            case gameState.starting:
                ResetUIPanels(0);
                if (timeTillStart >= 0)
                {
                    timeTillStart -= Time.deltaTime;
                    float minutes = Mathf.FloorToInt(timeTillStart / 60);
                    float seconds = Mathf.FloorToInt(timeTillStart % 60);
                    timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
                }
                else
                {
                    state = gameState.loading;
                }
                break;
            case gameState.loading:
                ResetUIPanels(1);
                for(int i = 0; i <= data.Count - 1; i++)
                {
                    StartCoroutine(spawnMe(data[i].playerObject, 0f));
                    data[i].personalScore = 0;
                }
                foreach(EnemyAI enemyAI in FindObjectsOfType<EnemyAI>())
                {
                    enemyAI.enemies.Clear();
                    enemyAI.findEnemies();
                }
                state = gameState.playing;
                break;
            case gameState.ending:
                foreach(playerData dataPoint in data)
                {
                    if(dataPoint.playerObject.GetComponent<MovementScript>())
                    {
                        dataPoint.playerObject.SetActive(true);
                    }
                    Debug.Log("despawning " + dataPoint.playerObject.name);
                    //deSpawnMe(dataPoint.playerObject, false, 0f);

                }
                state = gameState.restarting;
                break;
            case gameState.restarting:
                int dataLength = data.Count;
                for(int i = 0; i <= dataLength - 1; i++)
                {
                    Debug.Log("reseting " + data[0].playerObject.name);
                    Destroy(data[0].personalScorecard);
                    try
                    {
                        if(data[0].playerObject.GetComponent<EnemyAI>() == true)
                        {   
                            Debug.Log("destorying " + data[0].playerObject.name);
                            data[0].playerObject.SetActive(true);
                            Destroy(data[0].playerObject);
                        }
                        else Debug.Log("The player " + data[0].playerObject.name + " is real");
                    }
                    catch(Exception e)
                    {
                        Debug.Log("Failed to destroy object " + data[0].playerObject.name);
                    }
                    data.RemoveAt(0);
                }
                state = gameState.setup;
                break;
        }
    }
    private void ResetUIPanels(int index)
    {
        foreach(GameObject panel in gamePanel)
        {
            panel.SetActive(false);
        }
        gamePanel[index].SetActive(true);
    }
    public IEnumerator spawnMe(GameObject sender, float spawnWait)
    {
        //Debug.Log("SpawnMe called at " + Time.time + " and will wait " + spawnWait + " seconds.");
        yield return new WaitForSeconds(spawnWait);
        if(sender.active == false)
        {
            //Debug.Log("SpawnMe resumed");
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
                foreach(Transform spawnPoint in spawnPoints)
                {
                    if(spawnPoint.GetComponent<SpawnPoint>().coolDown <= 0)
                    {
                        possibleSpawns.Add(spawnPoint);
                    }
                }
            }
            bool isSpawned = false;
            int whileIteration = 0;
            sender.GetComponent<HealthHandler>().resetHealth();
            while(!isSpawned && whileIteration < 10)
            {
                int rnd = UnityEngine.Random.Range(0, possibleSpawns.Count);
                if(!Physics.CheckSphere(possibleSpawns[rnd].position, spawnSafeZone, combatantLayer))
                {
                    sender.SetActive(true);
                    if(sender.GetComponent<MovementScript>() == true)
                    {
                        resetPlayerCam(true, null);
                        sender.transform.position = possibleSpawns[rnd].position;
                    }
                    else if(GetComponent<EnemyAI>() == true)
                    {
                        sender.GetComponent<EnemyAI>().findEnemies();
                        sender.GetComponent<NavMeshAgent>().Warp(possibleSpawns[rnd].position);
                    }
                    isSpawned = true;
                }
                whileIteration++;
            }
        }
        else
        {
            Debug.Log(sender.name + " is already spawned");
        }
    }
    public void deSpawnMe(GameObject sender, bool doRespawn, float deathCooldown)
    {
        if(sender.GetComponent<MovementScript>() == true)
        {
            resetPlayerCam(false, mapCenter);
        }
        data[IndexPlayers(sender)].playerObject.GetComponent<HealthHandler>().despawned = true;
        data[IndexPlayers(sender)].playerObject.SetActive(false);
        if(doRespawn) StartCoroutine(spawnMe(sender, deathCooldown));
    }
    private GameObject createScorecard(GameObject player)
    {
        GameObject newScorecard = Instantiate<GameObject>(playerScorecard);
        //scorecards.Add(newScorecard);
        newScorecard.transform.SetParent(gamePanel[1].transform);
        newScorecard.transform.GetComponentInChildren<TextMeshProUGUI>().text = "0";
        newScorecard.transform.localScale = new Vector3(1,1,1);
        newScorecard.transform.GetComponent<Image>().color = player.GetComponent<TeamManager>().teamColor;
        newScorecard.SetActive(true);
        return newScorecard;
    }
    public void updatePlayerScore(GameObject player)
    {
        data[IndexPlayers(player)].personalScore++;
        data[IndexPlayers(player)].personalScorecard.transform.GetComponentInChildren<TextMeshProUGUI>().text = data[IndexPlayers(player)].personalScore.ToString();
        checkScores();
    }
    public void checkScores()
    {
        List<playerData> teamA = new List<playerData>();
        float teamAScore = 0;
        List<playerData> teamB = new List<playerData>();
        float teamBScore = 0;
        foreach(playerData dataPoint in data)
        {
            if(teamDeathmatch)
            {
                if(dataPoint.playerObject.GetComponent<TeamManager>().teamColor == teamAColour)
                {
                    teamAScore += dataPoint.personalScore;
                }
                else if(dataPoint.playerObject.GetComponent<TeamManager>().teamColor == teamBColour)
                {
                    teamBScore += dataPoint.personalScore;
                }
                else
                {
                    Debug.Log("ERROR - This player isn't on either team");
                }
                if(teamAScore >= teamScoreCap)
                {
                    state = gameState.ending;
                }
                if(teamBScore >= teamScoreCap)
                {
                    state = gameState.ending;
                }
            }
            else if (!teamDeathmatch)
            {
                if(dataPoint.personalScore >= soloScoreCap)
                {
                    state = gameState.ending;
                }
            }
        }
    }
    public int IndexPlayers(GameObject targetObject)
    {
        for(int i = 0; i <= data.Count - 1; i++)
        {
            if(data[i].playerObject == targetObject)
            {
                return i;
            }
        }
        return -1;
    }
}
