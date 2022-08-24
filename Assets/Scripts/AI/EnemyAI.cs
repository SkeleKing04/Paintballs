using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    private AIShooting shooting;
    public List<GameObject> enemies;
    public GameObject target;
    public LayerMask whatIsGround, whatIsEnemy;
    [Header("Patroling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    [Header("Attacking")]
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    [Header("States")]
    public float sightRange, attackRange;
    public bool enemyInSightRange, enemyInAttackRange, targetVisable;
    [Header("Shooting")]
    public Transform trueFireTransform, falseFireTransform;
    public Transform head;

    private void Awake()
    {
        findEnemies();
        shooting = GetComponent<AIShooting>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        checkTarget();
        enemyInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
        enemyInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);
        if (target == null) Patorling();
        if (target != null && (transform.position - target.transform.position).magnitude <= attackRange)
        {   
            Debug.Log(gameObject.name + " is attacking target " + target.name);
            AttackPlayer();
        }
        else if (target != null && (transform.position - target.transform.position).magnitude <= sightRange)
        {
            Debug.Log(gameObject.name + " is chasing target " + target.name);
            ChasePlayer();
        }
        else
        {
            target = null;
        }
    }
    private void checkTarget()
    {
        if(target != null)
        {
            if((transform.position - target.transform.position).magnitude >= sightRange || !target.activeSelf)
            {
                target = null;
            }
        }
    }
    public void findEnemies()
    {
        foreach(GameObject combatant in GameObject.FindGameObjectsWithTag("Combatant"))
        {
            if(combatant.GetComponent<TeamManager>().teamColor != gameObject.GetComponent<TeamManager>().teamColor)
            {
                enemies.Add(combatant);
            }
        }
    }
    private void detectTarget()
    {   
        foreach(GameObject enemy in enemies)
        {
            if(target != null)
            {
                if((transform.position - enemy.transform.position).magnitude < (transform.position - target.transform.position).magnitude)
                {
                    if((transform.position - enemy.transform.position).magnitude <= sightRange && enemy.activeSelf) target = enemy;
                }
            }
            else 
            {
                if((transform.position - enemy.transform.position).magnitude <= sightRange && enemy.activeSelf) target = enemy;
                else target = null;
            }
        }
    }

    private void Patorling()
    {   
        //findEnemies();
        detectTarget();
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z +randomZ);
        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }
    private void ChasePlayer()
    {
        agent.SetDestination(target.transform.position);
    }
    private void AttackPlayer()
    {
        Vector3 strafePos = transform.right * Random.Range(-50, 50);
        agent.SetDestination(strafePos);

        head.LookAt(target.transform);

        if(shooting.state == AIShooting.gunState.ready)
        {
            shooting.Shoot(trueFireTransform, falseFireTransform);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
