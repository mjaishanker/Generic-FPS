using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    [SerializeField] GravityAttractor planet;

    Rigidbody rb;

    private void Awake()
    {
        //planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

    }
    private void FixedUpdate()
    {
        if (planet != null)
        {
            if (transform.tag == "Player") // && transform.GetComponent<PlayerMovement>().state != PlayerMovement.State.WallRunning // && transform.GetComponent<PlayerMovement>().state == PlayerMovement.State.Normal
            {
                //Debug.Log("Attracting Player");
                transform.GetComponent<PlayerMovement>().onPlanet = planet.gameObject.transform;
                planet.Attract(transform);
            }
            //Debug.Log("Under Gravity State: " +);
            //if(transform.GetComponent<FirstPersonController>().getState() == State.Normal)
            else if (transform.tag != "Player")
            {
                //Debug.Log("Attracting Others");

                planet.Attract(transform);
            }
        }
    }

    public void setPlanet(GravityAttractor newPlanet)
    {
        planet = newPlanet;
    }
}
