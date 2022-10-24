using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

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
        leaving,
        mainMenu
    };

    public static gameState state;

    [Header("Players")]
    //public List<GameObject> players;
    [Header("Spawning")]
    //public Transform spawnArea;
    public float spawnSize, spawnSafeZone;
    public LayerMask combatantLayer;
    public Transform[] spawnPoints;
    [Header("Game Settings")]
    public static bool teamDeathmatch;
    public static int scoreCap,teamSize;
    private int botCount;
    public GameObject botPrefab;
    static public bool fillRoomWithBots;
    public Color teamAColour, teamBColour;
    public float gameStartTime;
    [Header("Unsorted")]
    public Transform mapCenter;
    public new Transform camera;
    public GameObject playerScorecard;
    //private List<GameObject> scorecards;
    //private List<float> scores;
    public float timeTillStart;
    public List<playerData> data;
    private UIHandler UI;
    public string exitScene;
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
        Debug.Log("Starting DM\nIncoming settings are:\nScore Cap: " + scoreCap.ToString() + "\nBots?: " +fillRoomWithBots.ToString() + "\nTeamDM?: " + teamDeathmatch.ToString() + "\nTeam Size: " + teamSize.ToString());
        UI = FindObjectOfType<UIHandler>();
        //data = new playerData[teamSize * 2];
        if(state != gameState.mainMenu)
        {
            startGate();
        }
    }
    public static void startGate()
    {
        Debug.Log("Start Gate hit");
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
        camera.GetComponent<CameraController>().enabled = toggle;
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
                        data[i].personalScorecard.transform.GetComponent<Image>().color = data[i].playerObject.GetComponent<TeamManager>().teamColor;
                        data[i].playerObject.SetActive(false);
                        data[i + 1].playerObject.GetComponent<TeamManager>().teamColor = teamBColour;
                        data[i + 1].playerObject.GetComponent<TeamManager>().UpdateColour();
                        data[i + 1].personalScorecard.transform.GetComponent<Image>().color = data[i + 1].playerObject.GetComponent<TeamManager>().teamColor;
                        data[i + 1].playerObject.SetActive(false);
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
                //UI.ResetUIPanels(new int[] {0});
                if (timeTillStart >= 0)
                {
                    timeTillStart -= Time.deltaTime;
                    float minutes = Mathf.FloorToInt(timeTillStart / 60);
                    float seconds = Mathf.FloorToInt(timeTillStart % 60);
                    //UI.timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
                }
                else
                {
                    state = gameState.loading;
                }
                break;
            case gameState.loading:
                //UI.ResetUIPanels(new int[] {1,2});
                for(int i = 0; i <= data.Count - 1; i++)
                {
                    StartCoroutine(spawnMe(data[i].playerObject, 0f));
                    data[i].personalScore = 0;
                }
                foreach(EnemyAI enemyAI in FindObjectsOfType<EnemyAI>())
                {
                    enemyAI.enemies.Clear();
                    //enemyAI.findEnemies();
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
                SceneManager.LoadScene(exitScene);
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
                        Debug.Log("Failed to destroy object " + data[0].playerObject.name + "\nError " + e.ToString());
                    }
                    data.RemoveAt(0);
                }
                state = gameState.setup;
                break;
            case gameState.mainMenu:
                break;
        }
    }

    public IEnumerator spawnMe(GameObject sender, float spawnWait)
    {
        //Debug.Log("SpawnMe called at " + Time.time + " and will wait " + spawnWait + " seconds.");
        yield return new WaitForSeconds(spawnWait);
        if(sender.activeSelf == false)
        {
            //Debug.Log("SpawnMe resumed");
            List<Transform> possibleSpawns = new List<Transform>();
            for (int i = 0; i < 3; i++)
            {
                foreach(Transform spawnPoint in spawnPoints)
                {
                    if(spawnPoint.GetComponent<SpawnPoint>().CheckSpawn(sender))
                    {
                        Debug.Log("Added " + spawnPoint.name + " to possible spawns");
                        possibleSpawns.Add(spawnPoint);
                    }
                    else Debug.Log("Didnt add " + spawnPoint.name + " to possible spawns");
                }
                if(possibleSpawns.Count >= 1)
                {
                    Debug.Log(sender + " has found spawns");
                    i = 3;
                }
                else if(i == 2)
                {
                    Debug.Log("No spawns found after 3 loops, something went wrong");
                }
                else Debug.Log("No spawns found this loop");
            }
            bool isSpawned = false;
            int whileIteration = 0;
            sender.GetComponent<HealthHandler>().resetHealth();
            while(!isSpawned && whileIteration < 10)
            {
                int rnd = UnityEngine.Random.Range(0, possibleSpawns.Count);
                    Debug.Log("Gate A Hit");
                    sender.SetActive(true);
                    sender.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
                    if(sender.GetComponent<MovementScript>() == true)
                    {
                        resetPlayerCam(true, null);
                        sender.transform.position = possibleSpawns[rnd].position;
                    }
                    if(sender.GetComponent<EnemyAI>() == true)
                    {
                        Debug.Log("Gate B Hit");
                        //sender.GetComponent<EnemyAI>().findEnemies();
                        sender.GetComponent<NavMeshAgent>().Warp(possibleSpawns[rnd].position);
                    }
                    isSpawned = true;
                    possibleSpawns[rnd].GetComponent<SpawnPoint>().doCoolDown(10);
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
        newScorecard.transform.SetParent(UI.gamePanel[1].transform);
        newScorecard.transform.GetComponentInChildren<TextMeshProUGUI>().text = "0";
        newScorecard.transform.localPosition = new Vector3(newScorecard.transform.position.x, newScorecard.transform.position.y, 0);
        newScorecard.transform.localRotation = Quaternion.Euler(0,0,0);
        newScorecard.transform.localScale = UI.gamePanel[1].transform.localScale;
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
                if(teamAScore >= scoreCap * teamSize)
                {
                    state = gameState.ending;
                }
                if(teamBScore >= scoreCap * teamSize)
                {
                    state = gameState.ending;
                }
            }
            else if (!teamDeathmatch)
            {
                if(dataPoint.personalScore >= scoreCap)
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
