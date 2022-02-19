using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject explosion;
    [SerializeField] LayerMask whatIsEnemies;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] public string sourceTag;

    [Header("Stats")]
    [Range(0f,1f)]
    [SerializeField] float bounciness;
    [SerializeField] bool useGravity;

    [Header("Damage")]
    [SerializeField] int explosionDamage;
    [SerializeField] float explosionRange;

    [Header("Lifetime")]
    [SerializeField] int maxCollisions;
    [SerializeField] float maxLifetime;
    [SerializeField] bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physicMaterial;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        // When to explode
        if (collisions > maxCollisions) ExplodeOnEnemy();

        // Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) ExplodeOnEnemy();

    }

    void ExplodeOnEnemy()
    {
        // Is explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        // Check for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for(int i = 0; i < enemies.Length; i++)
        {
            // Get component of enemy and call take damage
            enemies[i].GetComponent<EnemyAi>().TakeDamage(explosionDamage);
        }

        // Add tiny delay, for debugging
        Invoke("Delay", 0.05f);
    }

    void ExplodeOnPlayer()
    {
        // Is explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        // Check for enemies
        Collider[] players = Physics.OverlapSphere(transform.position, explosionRange, whatIsPlayer);
        for (int i = 0; i < players.Length; i++)
        {
            // Get component of enemy and call take damage
            players[i].GetComponent<PlayerInfo>().PlayerTakeDamage(explosionDamage);
        }

        // Add tiny delay, for debugging
        Invoke("Delay", 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Debugging");
        //if (collision.collider.CompareTag("Bullet")) return;

        // Count up collisions
        collisions++;

        //Bullet Enemy Collision
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            Debug.Log("Player shoots Enemy");
            ExplodeOnEnemy();
        }
        else if (collision.collider.CompareTag("Player") && explodeOnTouch)
        {
            Debug.Log("Enemy shoots Player: " + collision.collider.tag);
            ExplodeOnPlayer();
        }
        //if (sourceTag == "Player")
        //{
        //    Debug.Log("Player shoots Enemy");
        //    if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
        //    {
        //        ExplodeOnEnemy();
        //    }
        //}
        //else if (sourceTag == "Enemy")
        //{
        //    Debug.Log("Enemy shoots Player: " + collision.collider.tag);
        //    if (collision.collider.CompareTag("Player") && explodeOnTouch)
        //    {

        //        ExplodeOnPlayer();
        //    }
        //}
        else Destroy(this.gameObject);
    }
    private void Setup()
    {
        // Create a new Physics material
        physicMaterial = new PhysicMaterial();
        physicMaterial.bounciness = bounciness;
        physicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

        // Assign material to collider
        GetComponent<SphereCollider>().material = physicMaterial;

        // Set Gravity 
        // rb.useGravity = useGravity;


    }
}
