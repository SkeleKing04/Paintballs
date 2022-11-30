using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    //public NavMeshAgent agent;
    public List<GameObject> allCombatants, enemies;
    public GameObject target;
    public float sightRange, attackRange;
    public Transform head,body;
    public Vector3 missLimits;
    private PlayerShooting shooting;
    private MovementScript movement;
    public float targetStopDist, jumpTreashhold, dashTreashhold;
    public bool[] pathBlocked = {false, false};

    void Start()
    {
        getAllCombantants();
        refreshEnemies();
        shooting = GetComponent<PlayerShooting>();
        movement = GetComponent<MovementScript>();
        //agent = GetComponent<NavMeshAgent>();
    }
    private void getAllCombantants()
    {
        allCombatants.AddRange(GameObject.FindGameObjectsWithTag("Combatant"));
        allCombatants.Remove(gameObject);
    }
    void Update()
    {
        if (target == null)
        //Wander
        {
            findTarget();
            refreshEnemies();
        }
        else if ((transform.position - target.transform.position).magnitude <= attackRange)
        //target in attack range
        {
            //Debug.Log("Target object is in attacking range");
            lookAtTarget();
            checkTarget();
            tryAttackTarget();
            moveSim();
        }
        else if ((transform.position - target.transform.position).magnitude <= sightRange)
        //target in sight
        {
            //Debug.Log("Target object is in sight");
            findTarget();
            lookAtTarget();
            checkTarget();
            moveSim();
        }
        else 
        {
            target = null;
            findTarget();
            refreshEnemies();
        }
    }
    public void refreshEnemies()
    {
        enemies.Clear();
        foreach(GameObject combatant in allCombatants)
        {
            if(combatant.GetComponent<TeamManager>().teamColor != GetComponent<TeamManager>().teamColor && combatant != gameObject && !enemies.Contains(combatant)) enemies.Add(combatant);
        }
    }
    private void findTarget()
    {
        foreach(GameObject enemy in enemies)
        {
            if(enemy.activeSelf == true)
            {
                float disToEnemy = (enemy.transform.position - transform.position).magnitude;
                float disToTarg = Mathf.Infinity;
                if (target != null) disToTarg = (target.transform.position - transform.position).magnitude;
                if(disToEnemy <= sightRange && disToTarg >= disToEnemy && enemy.GetComponent<MovementScript>().enabled) 
                {
                    target = enemy;
                }
            }
        }
    }
    private void checkTarget()
    {
        if(target != null)
        {
            if((transform.position - target.transform.position).magnitude >= sightRange ||
                !target.activeSelf ||
                target.GetComponent<TeamManager>().teamColor == GetComponent<TeamManager>().teamColor ||
                !target.GetComponent<MovementScript>().isActiveAndEnabled)
            {
                target = null;
                refreshEnemies();
            }
        }
    }
    private void lookAtTarget()
    {
        head.LookAt(target.transform.position + new Vector3(Random.Range(-missLimits.x, missLimits.x),Random.Range(-missLimits.y, missLimits.y),Random.Range(-missLimits.z, missLimits.z)) * (target.transform.position - transform.position).magnitude, Vector3.up);
        body.LookAt(target.transform, Vector3.up);
        body.transform.rotation = new Quaternion(0,body.rotation.y,0,body.rotation.w);
    }
    private void tryAttackTarget()
    {
        if(shooting.state == PlayerShooting.gunState.ready) shooting.Shoot();
    }
    private void moveSim()
    {
        //Debug.Log(Mathf.Abs((target.transform.position - transform.position).magnitude));
        if(target != null && (!pathBlocked[0] || !pathBlocked[1]))
        {
            if((target.transform.position - transform.position).magnitude > targetStopDist - (targetStopDist * 0.1) && (target.transform.position - transform.position).magnitude < targetStopDist) movement.verticalInput = 0;
            else if((target.transform.position - transform.position).magnitude > targetStopDist - (targetStopDist * 0.1)) movement.verticalInput = 1;
            else if((target.transform.position - transform.position).magnitude < targetStopDist + (targetStopDist * 0.1)) movement.verticalInput = -1;
            if(Mathf.Abs(target.transform.position.y - transform.position.y) > jumpTreashhold && movement.readyToJump && movement.grounded)
            {
                movement.doJump();
            }
            else if(Mathf.Abs((target.transform.position - transform.position).magnitude) > dashTreashhold && movement.dashCount > 0)
            {
                movement.doDash();
            }
        }
        if(target !=null)
        {
            WallCheck();
        }
        //movement.verticalInput = Input.GetAxisRaw("Vertical");

        
    }
    private void WallCheck()
    {
        Vector3[] wallCheckOrigin = { transform.position + transform.right * 0.4f, transform.position + transform.right * -0.4f};
        if(Physics.Linecast(wallCheckOrigin[0], wallCheckOrigin[0] + transform.forward * 1,LayerMask.GetMask("Wall")) && !pathBlocked[1])
        {
        Debug.DrawLine(wallCheckOrigin[0], wallCheckOrigin[0] + transform.forward * 1, Color.green, 0.1f);
            Debug.Log("Right Path is blocked");
            pathBlocked[0] = true;
            WalkAroundObject(0);
        }
        else
        {
            Debug.Log("Right Path is clear");
            pathBlocked[0] = false;
        }
        if(Physics.Linecast(wallCheckOrigin[1], wallCheckOrigin[1] + transform.forward * 1,LayerMask.GetMask("Wall")) && !pathBlocked[0])
        {
        Debug.DrawLine(wallCheckOrigin[1], wallCheckOrigin[1] + transform.forward * 1, Color.red, 0.1f);
            Debug.Log("Left Path is blocked");
            pathBlocked[1] = true;
            WalkAroundObject(1);
        }
        else
        {
            Debug.Log("Left Path is clear");
            pathBlocked[1] = false;
        }
        if (!pathBlocked[0] && !pathBlocked[1]) movement.horizontalInput = 0;
        else if(!pathBlocked[0] || !pathBlocked[1])
        {
            Debug.DrawLine(transform.position, transform.position + transform.right * movement.horizontalInput,Color.red,0.1f);
            if(Physics.Linecast(transform.position, transform.position + transform.right * movement.horizontalInput, LayerMask.GetMask("Wall")))
            {
                movement.horizontalInput *= -1;
                pathBlocked[0] = !pathBlocked[0];
                pathBlocked[1] = !pathBlocked[1];
            }
        }
    }
    private void WalkAroundObject(int moveDir)
    {
        movement.verticalInput = 0;
        switch(moveDir)
        {
            case 0:
                movement.horizontalInput = -1;
                break;
            case 1:
                movement.horizontalInput = 1;
                break;
        }
    }
}
