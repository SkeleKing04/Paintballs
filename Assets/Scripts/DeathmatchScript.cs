using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.UI;

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
    public List<GameObject> players;
    [Header("Spawning")]
    public Transform spawnArea;
    public float spawnSize, spawnSafeZone;
    public LayerMask combatantLayer;
    public Transform[] spawnPoints;
    [Header("Game Settings")]
    public bool teamDeathmatch;
    public int teamSize, botCount;
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
    private List<GameObject> scorecards;
    private List<float> scores;
    public float timeTillStart;

    // Start is called before the first frame update
    void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        state = gameState.setup;
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
        if(teamDeathmatch)
        {
            players = playerShuffle(players);
            for(int i = 0; i < players.Count; i += 2)
            {
                players[i].GetComponent<TeamManager>().teamColor = teamAColour;
                players[i].GetComponent<TeamManager>().UpdateColour();
                players[i + 1].GetComponent<TeamManager>().teamColor = teamBColour;
                players[i + 1].GetComponent<TeamManager>().UpdateColour();
            }
            for(int i = 0; i < spawnPoints.Length / 2; i++)
            {
                Debug.Log("index is " + i + ", therefore, other point is " + (i + spawnPoints.Length / 2));
                spawnPoints[i].GetComponent<TeamManager>().teamColor = teamAColour;
                spawnPoints[i + spawnPoints.Length / 2].GetComponent<TeamManager>().teamColor = teamBColour;
            }
        }
        else
        {
            foreach(GameObject player in players)
            {
                player.GetComponent<TeamManager>().teamColor = new Color(Random.Range(0f, 256f)/255f,Random.Range(0f, 256f)/255f,Random.Range(0f, 256f)/255f,255f);
                player.GetComponent<TeamManager>().UpdateColour();
            }
        }
    }
    private void startGame()
    {
        resetPlayerCam(false, mapCenter);
        foreach(GameObject player in players)
        {
            player.SetActive(false);
        }
        timeTillStart = gameStartTime;
        state = gameState.starting;

    }
    private void resetPlayerCam(bool toggle, Transform posToMove)
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
            case gameState.starting:
                ResetUIPanels(0);
                if (timeTillStart > 0)
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
                scores.Add(0);
                scores.Clear();
                for(int i = 0; i <= players.Count - 1; i++)
                {
                    spawnMe(players[i]);
                    createScorecard(players[i]);
                    int score = 0;
                    scores.Add(score);                    
                }
                foreach(EnemyAI enemyAI in FindObjectsOfType<EnemyAI>())
                {
                    enemyAI.enemies.Clear();
                    enemyAI.findEnemies();
                }
                state = gameState.playing;
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
        while(!isSpawned && whileIteration < 10)
        {
            int rnd = Random.Range(0, possibleSpawns.Count);
            if(!Physics.CheckSphere(possibleSpawns[rnd].position, spawnSafeZone, combatantLayer))
            {
                sender.SetActive(true);
                if(sender.GetComponent<MovementScript>() == true)
                {
                    resetPlayerCam(true, null);
                    sender.transform.position = possibleSpawns[rnd].position;
                }
                else if(sender.GetComponent<EnemyAI>() == true)
                {
                    sender.GetComponent<EnemyAI>().findEnemies();
                    sender.GetComponent<NavMeshAgent>().Warp(possibleSpawns[rnd].position);
                }
                isSpawned = true;
            }
            whileIteration++;
        }
    }
    private void createScorecard(GameObject player)
    {
        GameObject newScorecard = Instantiate<GameObject>(playerScorecard);
        scorecards.Add(newScorecard);
        newScorecard.transform.SetParent(gamePanel[1].transform);
        newScorecard.transform.GetComponentInChildren<TextMeshProUGUI>().text = "0";
        newScorecard.transform.localScale = new Vector3(1,1,1);
        newScorecard.transform.GetComponent<Image>().color = player.GetComponent<TeamManager>().teamColor;
        newScorecard.SetActive(true);
    }
    public void updatePlayerScore(GameObject player)
    {
        scores[players.IndexOf(player)]++;
        scorecards[players.IndexOf(player)].transform.GetComponentInChildren<TextMeshProUGUI>().text = scores[players.IndexOf(player)].ToString();

    }
}
