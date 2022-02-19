using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3 hitPoint;
    public GameObject dirt;
    public GameObject blood;

    public int speed;
    public float bulletDamage;

    public float timeToLive = 3f;

    public void Start()
    {
        //this.GetComponent<Rigidbody>().AddForce((hitPoint - this.transform.position).normalized * speed);
        Destroy(gameObject, timeToLive);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy tempEnemy = collision.transform.GetComponent<Enemy>();
            if (tempEnemy != null)
            {
                tempEnemy.takeDamage(bulletDamage);
                Destroy(this.gameObject);
            }

        }
        else
        {
            Destroy(this.gameObject);
        }
        Destroy(this.gameObject);
    }
}
