using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShooterAI : MonoBehaviour
{
    public List<GameObject> enemies;
    public TeamManager teamManager;
    public float AISightRange;
    public float AIStopRange;
    private bool inCombat;
    public GameObject target;
    private Vector3 lastKnownTargetPos;
    private NavMeshAgent Agent;
    private LayerMask layerMasks;
    // Start is called before the first frame update
    void Start()
    {
        layerMasks = 1 << 8;
        layerMasks = ~layerMasks;
        Agent = GetComponent<NavMeshAgent>();
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Combatant"))
        {
            if(player.GetComponent<TeamManager>().teamColor != teamManager.teamColor)
            {
                enemies.Add(player);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(lastKnownTargetPos, transform.up * 10, Color.yellow, 0.01f);
        if(target == null) trackEnemies();
        else trackTarget();
        
    }
    private void trackEnemies()
    {
        foreach(GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).magnitude;
            if(distance < AISightRange && !targetBlocked(enemy, enemy.transform.position))
            {
                Debug.Log(enemy.name + " can be seen");
                target = enemy;
                //inCombat = true;
            }
        }
    }
    private void trackTarget()
    {
        float distance = (target.transform.position - transform.position).magnitude;
        if(distance < AISightRange && !targetBlocked(target, target.transform.position))
        {
            Agent.stoppingDistance = AIStopRange;
            if(target.transform.position != lastKnownTargetPos)
            {
                lastKnownTargetPos = target.transform.position;     
                //transform.LookAt(lastKnownTargetPos);
                Agent.SetDestination(lastKnownTargetPos);
            }     
        }
        else if (Agent.remainingDistance > 0)
        {
            Agent.stoppingDistance = 0;
        }
        else
        {
            target = null;
            //inCombat = false;
        }
    }
    private bool targetBlocked(GameObject objectToHit, Vector3 targetPos)
    {
        RaycastHit hit;
        bool blocked = !Physics.Linecast(transform.position, targetPos, out hit);
        if(hit.collider.gameObject == objectToHit) return false;
        else if(blocked && hit.collider.gameObject != objectToHit) return true;
        else return true;
    }
}
