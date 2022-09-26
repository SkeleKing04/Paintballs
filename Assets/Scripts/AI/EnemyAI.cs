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

    void Start()
    {
        getAllCombantants();
        refreshEnemies();
        shooting = GetComponent<PlayerShooting>();
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
            Debug.Log("Target object is in attacking range");
            lookAtTarget();
            checkTarget();
            attackTarget();
        }
        else if ((transform.position - target.transform.position).magnitude <= sightRange)
        //target in sight
        {
            Debug.Log("Target object is in sight");
            findTarget();
            lookAtTarget();
            checkTarget();
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
    private void attackTarget()
    {
        shooting.Shoot();
    }
}
