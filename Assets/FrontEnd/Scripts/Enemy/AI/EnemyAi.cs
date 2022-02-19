using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] float health;
    private float _health;

    [Header("Patrolling")]
    [SerializeField] Vector3 walkPoint;
    bool walkPointSet;
    [SerializeField] float walkPointRange;

    [Header("Attacking")]
    [SerializeField] float timeBetweenAttacks;
    bool alreadyAttacked;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform attackPoint;

    [Header("States")]
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] bool playerInSightRange;
    [SerializeField] bool playerInAttackRange;
    private float _sightRange;

    [Header("Enemy Dead")]
    [SerializeField] ParticleSystem enemyDeathEffect;
    [SerializeField] Renderer enemyHealthIndicator;
    [SerializeField] Color fullHealthColor;
    [SerializeField] Color lowHealthColor;

    private void Awake()
    {
        _sightRange = sightRange;
        _health = health;

        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().AddEnemyToList(this.gameObject);
        enemyHealthIndicator = transform.GetChild(0).GetComponent<MeshRenderer>();
        enemyHealthIndicator.material.color = Color.red;

    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        agent.transform.rotation = transform.rotation;

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        sightRange = _sightRange;
    }

    void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        sightRange = attackRange;
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesnt move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        sightRange = attackRange;

        if (!alreadyAttacked) 
        {
            /// Attack code here
            Rigidbody rb = Instantiate(projectile, attackPoint.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.GetComponent<CustomBullet>().sourceTag = "Enemy";

            rb.AddForce(transform.forward * 64f /*32f*/, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            ///

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Taking Damage");
        health -= damage;
        if(health < 0)
        {
            health = 0;
        }
        enemyHealthIndicator.material.color = Color.Lerp(lowHealthColor, fullHealthColor, health / _health);


        if (health <= 0)
        {
            enemyDeathEffect.Play();
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    void DestroyEnemy()
    {
        
        GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().RemoveEnemyToList(this.gameObject);
        Destroy(gameObject);
    }
}
