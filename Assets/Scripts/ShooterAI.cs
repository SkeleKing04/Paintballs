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
    private NavMeshPath navPath;
    private Vector3[] travelPoints;
    private int travelPointIndex;
    private bool atPoint = false;
    private bool destinationReached = false;
    public float randomMoveFloat;
    public float randomMoveWait;
    private bool canIdleMove;
    private bool doStrafe;
    public float changeStrafeDirCooldown;
    // Shooting
    public GameObject Head;
    public Transform fireTransform;
    public Transform trailTransform;
    private AIShooting shooting;
    private AIMovementScript movementScript;
    // Other
    private LayerMask layerMasks;
    // Start is called before the first frame update
    void Start()
    {
        canIdleMove = true;
        doStrafe = true;
        layerMasks = 1 << 8;
        layerMasks = ~layerMasks;
        Agent = GetComponent<NavMeshAgent>();
        shooting = GetComponent<AIShooting>();
        movementScript = GetComponent<AIMovementScript>();
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
                if(canIdleMove)
                {
                    canIdleMove = false;
                    Vector3 randomPos = new Vector3(Random.Range(transform.position.x - randomMoveFloat,transform.position.x + randomMoveFloat),Random.Range(transform.position.y - randomMoveFloat,transform.position.y + randomMoveFloat),Random.Range(transform.position.z - randomMoveFloat,transform.position.z + randomMoveFloat));
                    //Debug.Log(gameObject.name + " is going to move to " + randomPos.ToString());
                    while(!Agent.CalculatePath(randomPos, Agent.path))
                    {
                        randomPos = new Vector3(Random.Range(transform.position.x - randomMoveFloat,transform.position.x + randomMoveFloat),Random.Range(transform.position.y - randomMoveFloat,transform.position.y + randomMoveFloat),Random.Range(transform.position.z - randomMoveFloat,transform.position.z + randomMoveFloat));
                        Debug.DrawLine(transform.position, randomPos, Color.black, 0.5f);
                        //Debug.LogWarning("Path cannot be crossed, retrying");
                        //return;
                    }           
                    movementScript.ObjOrient.LookAt(randomPos);
                    moveToTarget(randomPos);
                    Invoke(nameof(resetCanMove), randomMoveWait);
                }
                break;
            case actionState.inChase:
                chaseTarget();
                //movementScript.forwardInput = 1;
                //moveToTarget();
                break;
            //case actionState.inCombat:
                //trackTargetData();
                //if(doStrafe)
                //{
                //    doStrafe = false;
                //    movementScript.strafeInput = Random.Range(-1, 2);
                //    Invoke(nameof(ResetStrafe), Random.Range(1, changeStrafeDirCooldown));
                //}
                //movementScript.forwardInput = 0;
                //int chooseAction = Random.Range(0, 4);
                //if(chooseAction == 0 && shooting.state == AIShooting.gunState.ready)
                //{
                //    shooting.Shoot(fireTransform, trailTransform);
                //}
                //if(chooseAction == 1)
                //{
                //    //Not used ATM
                //}
                //if(shooting.state == AIShooting.gunState.ready)
                //{
                //    //Reload();
                //}
                //if(chooseAction == 2 && shooting.state == AIShooting.gunState.ready)
                //{
                //    //Switch to next weapon
                //}
                //break;
        }
        if(!destinationReached)
        {
            if(!atPoint)
            {
                //Debug.Log("The travel point index is " + travelPointIndex.ToString());
                Quaternion lookRot = Quaternion.LookRotation(travelPoints[travelPointIndex]);
                lookRot.x = 0; lookRot.z = 0;
                Head.transform.rotation = Quaternion.Slerp(Head.transform.rotation,lookRot, 100 * Time.deltaTime);
                movementScript.ObjOrient.LookAt(travelPoints[travelPointIndex]);
                Debug.DrawLine(transform.position, travelPoints[travelPointIndex], Color.cyan, 0.5f);
                movementScript.forwardInput = 1;
                float distanceToPoint = (transform.position - travelPoints[travelPointIndex]).magnitude;
                if(distanceToPoint < 1.1f)
                {
                    atPoint = true;
                }
            }
            if(atPoint)
            {
                movementScript.forwardInput = 0;
                if(travelPointIndex >= travelPoints.Length - 1)
                {
                    destinationReached = true;
                }
                else
                {
                    travelPointIndex++;
                    atPoint = false;
                }
            }
        }
        checkAllEnemies();
    }
    private void resetCanMove()
    {
        canIdleMove = true;
    }
    // FOLLOWING AND TRACKING
    private void moveToTarget(Vector3 position)
    {
        Debug.DrawLine(transform.position, position, Color.yellow, 0.5f);
        float distance = (transform.position - position).magnitude;
        if(distance < AISightRange)
        {
            travelPointIndex = 0;
            Agent.SetDestination(position);
            travelPoints = Agent.path.corners;
            Agent.Stop();
            destinationReached = false;
            atPoint = false;
            //movementScript.state = AIMovementScript.MovementStates.sprinting;
        }
    }
    private void checkAllEnemies()
    {
        foreach(GameObject enemy in enemies)
        {
            float distance = (enemy.transform.position - transform.position).magnitude;
            if(distance < AISightRange)
            {
                Debug.Log(enemy.name + " is in range of " + gameObject.name);
                if(!targetBlocked(enemy, enemy.transform.position))
                {
                    Debug.Log(enemy.name + " can be seen");
                    if(target != null)
                    {
                        if((target.transform.position - transform.position).magnitude > distance)
                        {
                            Debug.Log(gameObject.name + " is now targeting " + enemy.name + " instead of " + target.name);
                            target = enemy;
                            state = actionState.inChase;
                        }
                    }
                    else
                    {
                        Debug.Log(gameObject.name + " is now targeting " + enemy.name);
                        target = enemy;
                        state = actionState.inChase;
                    }
                    //inCombat = true;
                }
                else
                {
                    Debug.Log(gameObject.name + " cannot see " + enemy.name);
                }
            }
            else
            {
                Debug.Log(enemy.name + " is out of range of " + gameObject.name);
            }
        }
    }
    private void chaseTarget()
    {
        if(!targetBlocked(target, target.transform.position))
        moveToTarget(lastKnownTargetPos);
    }
    private void checkTarget()
    {
        if((lastKnownTargetPos - transform.position).magnitude <= 10 && !targetBlocked(target, target.transform.position))
        {
            state = actionState.inCombat;
            return;
        }
        else state = actionState.inChase;

    }
    private void ResetStrafe()
    {
        doStrafe = true;
    }
    private bool targetBlocked(GameObject objectToHit, Vector3 targetPos)
    {
        RaycastHit hit;
        bool blocked = !Physics.Linecast(transform.position, targetPos, out hit);
        Debug.DrawLine(transform.position, targetPos, Color.magenta, 0.1f);
        if(hit.collider.gameObject == objectToHit) 
        {
            //Debug.Log(gameObject.name + " can see " + objectToHit.gameObject.name);
            return false;
        }
        else
        {
            //Debug.Log(gameObject.name + " is blocked from seeing " + objectToHit.gameObject.name + " by " + hit.collider.name);
            return true;
        }
    }
    // COMBAT AND SHOOTING
    private void inputSim()
    {

    }
}
