using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbit : MonoBehaviour
{

    [SerializeField] GravityAttractor globalPlanet;

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Colliding");
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("Player Colliding");
            other.gameObject.GetComponent<GravityBody>().setPlanet(gameObject.GetComponentInParent<GravityAttractor>());
        }
        else
        { 
            Debug.Log("Others Colliding: " + other.gameObject.name);
            other.gameObject.GetComponent<GravityBody>().setPlanet(gameObject.GetComponentInParent<GravityAttractor>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<GravityBody>().setPlanet(globalPlanet);
        }
        else
        {
            //Debug.Log("Others Exiting");
            other.gameObject.GetComponent<GravityBody>().setPlanet(globalPlanet);
        }
    }
}
