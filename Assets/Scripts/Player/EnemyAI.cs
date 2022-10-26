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
                if(disToEnemy <= sightRange && disToTarg >= disToEnemy) 
                {
                    target = enemy;
                }
            }
        }
    }
    private void checkTarget()
    {
        if((transform.position - target.transform.position).magnitude >= sightRange ||
            !target.activeSelf ||
            target.GetComponent<TeamManager>().teamColor == GetComponent<TeamManager>().teamColor)
        {
            target = null;
            refreshEnemies();
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
        if(target != null)
        {
            if((target.transform.position - transform.position).magnitude > targetStopDist - (targetStopDist * 0.1) && (target.transform.position - transform.position).magnitude < targetStopDist) movement.verticalInput = 0;
            else if((target.transform.position - transform.position).magnitude > targetStopDist - (targetStopDist * 0.1)) movement.verticalInput = 1;
            else if((target.transform.position - transform.position).magnitude < targetStopDist + (targetStopDist * 0.1)) movement.verticalInput = -1;
        }    
        //movement.verticalInput = Input.GetAxisRaw("Vertical");

        if(Mathf.Abs(target.transform.position.y - transform.position.y) > jumpTreashhold && movement.readyToJump && movement.grounded)
        {
            movement.doJump();
        }
        else if(Mathf.Abs((target.transform.position - transform.position).magnitude) > dashTreashhold && movement.dashCount > 0)
        {
            movement.doDash();
        }
        
    }
}
