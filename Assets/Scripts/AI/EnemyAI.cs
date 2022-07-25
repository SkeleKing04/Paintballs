using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    private AIShooting shooting;
    public List<GameObject> enemies;
    public Transform target;
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
    public bool enemyInSightRange, enemyInAttackRange;
    [Header("Shooting")]
    public Transform fireTransform;
    public Transform trailTransform;
    public Transform head;

    private void Awake()
    {
        foreach(GameObject combatant in GameObject.FindGameObjectsWithTag("Combatant"))
        {
            if(combatant.GetComponent<TeamManager>().teamColor != gameObject.GetComponent<TeamManager>().teamColor)
            {
                enemies.Add(combatant);
            }
        }
        shooting = GetComponent<AIShooting>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        enemyInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);

        if (target == null) Patorling();
        if (target != null && (transform.position - target.transform.position).magnitude >= attackRange) ChasePlayer();
        if(target != null && (transform.position - target.transform.position).magnitude <= attackRange) AttackPlayer();
    }
    private void detectTarget()
    {
        foreach(GameObject enemy in enemies)
        {
            if(target != null)
            {
                if((transform.position - enemy.transform.position).magnitude < (transform.position - target.transform.position).magnitude)
                {
                    if((transform.position - enemy.transform.position).magnitude <= sightRange) target = enemy.transform;
                }
            }
            else 
            {
                if((transform.position - enemy.transform.position).magnitude <= sightRange) target = enemy.transform;
                else target = null;
            }
        }
    }

    private void Patorling()
    {   
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
        agent.SetDestination(target.position);
    }
    private void AttackPlayer()
    {
        Vector3 strafePos = transform.right * Random.Range(-50, 50);
        agent.SetDestination(strafePos);

        head.LookAt(target);

        if(!alreadyAttacked)
        {
            shooting.Shoot(fireTransform, trailTransform);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
