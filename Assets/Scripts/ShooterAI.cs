using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShooterAI : MonoBehaviour
{
    // Players and Enemeies
    public List<GameObject> enemies;
    public TeamManager teamManager;
    public float AISightRange;
    public float AIStopRange;
    public enum actionState
    {
        idle,
        inChase,
        inCombat
    };
    public actionState state;
    public GameObject target;
    private Vector3 lastKnownTargetPos;
    // Navigation
    private NavMeshAgent Agent;
    public float randomMoveFloat;
    public float randomMoveWait;
    private bool canMove;
    // Shooting
    public GameObject Head;
    public Transform fireTransform;
    public Transform trailTransform;
    private AIShooting shooting;
    // Other
    private LayerMask layerMasks;
    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        layerMasks = 1 << 8;
        layerMasks = ~layerMasks;
        Agent = GetComponent<NavMeshAgent>();
        shooting = GetComponent<AIShooting>();
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
        switch(state)
        {
            case actionState.idle:
                if(canMove)
                {
                    Invoke(nameof(doIdle),randomMoveWait);
                    canMove = false;
                }
                if(target == null) trackEnemies();
                break;
            case actionState.inChase:
                break;
            case actionState.inCombat:
                int chooseAction = Random.Range(0, 4);
                if(chooseAction == 0 && shooting.state == AIShooting.gunState.ready)
                {
                    shooting.Shoot(fireTransform, trailTransform);
                }
                if(chooseAction == 1)
                {
                    //Not used ATM
                }
                if(shooting.state == AIShooting.gunState.ready)
                {
                    //Reload();
                }
                if(chooseAction == 2 && shooting.state == AIShooting.gunState.ready)
                {
                    //Switch to next weapon
                }
                break;
        }
        if (target != null) trackTarget();        
    }
    // IDLE
    private void doIdle()
    {
        Vector3 randomPos = new Vector3(Random.Range(transform.position.x - randomMoveFloat,transform.position.x + randomMoveFloat),Random.Range(transform.position.y - randomMoveFloat,transform.position.y + randomMoveFloat),Random.Range(transform.position.z - randomMoveFloat,transform.position.z + randomMoveFloat));
        Debug.Log(randomPos);
        Agent.SetDestination(randomPos);
        canMove = true;
    }
    // FOLLOWING AND TRACKING
    private void trackEnemies()
    {
        foreach(GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).magnitude;
            if(distance < AISightRange && !targetBlocked(enemy, enemy.transform.position))
            {
                Debug.Log(enemy.name + " can be seen");
                target = enemy;
                state = actionState.inChase;
                //inCombat = true;
            }
        }
    }
    private void trackTarget()
    {
        if((target.transform.position - transform.position).magnitude <= 10) state = actionState.inCombat;
        else state = actionState.inChase;
        Debug.DrawRay(lastKnownTargetPos, transform.up * 10, Color.yellow, 0.01f);
        float distance = (target.transform.position - transform.position).magnitude;
        if(distance < AISightRange && !targetBlocked(target, target.transform.position))
        {
            Agent.stoppingDistance = AIStopRange;
            if(target.transform.position != lastKnownTargetPos)
            {
                lastKnownTargetPos = target.transform.position;     
                Head.transform.LookAt(lastKnownTargetPos);
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
            state = actionState.idle;
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
    // COMBAT AND SHOOTING
    private void inputSim()
    {

    }
}
