using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f;

    public bool isHalo;
    public bool isNotSphere;
    public bool isLerping;

    [SerializeField] string rotationAxis;
    RaycastHit hit;

    private void Awake()
    {
        if (gameObject.tag == "Halo")
        {
            isHalo = true;
        }
        else
        {
            isHalo = false;
        }
        if(gameObject.tag == "CubePlanet")
        {
            isNotSphere = true;
        }
    }

    public void Attract(Transform body)
    {
        if (!isHalo && !isNotSphere)
        {
            // Orient the body so the Up axis is looking at the center of the planet and apply a downward force
            Vector3 targetDirection = (body.position - transform.position).normalized;
            Vector3 bodyUp = body.up;

            //Debug.DrawRay(transform.position, targetDirection * 100f, Color.blue);
            //Debug.DrawRay(body.position, body.up * 100f, Color.green);

            body.rotation = Quaternion.FromToRotation(bodyUp, targetDirection) * body.rotation;
            body.GetComponent<Rigidbody>().AddForce(targetDirection * gravity);
        }
        else if (!isHalo && isNotSphere)
        {
            //Debug.Log("Is not Sphere");

            Vector3 bodyUp = body.up;
            Vector3 targetDirection = (body.position - transform.position).normalized;
            if (!Physics.Raycast(body.GetChild(3).position, -bodyUp, out hit))
            {
                isLerping = true;
                //Debug.Log("Hit info: " + hit.transform.name);
                //if(hit.transform.tag != "CubePlanet")
                //{
                //    //Debug.Log("Not Equal");
                //    body.rotation = Quaternion.FromToRotation(bodyUp, targetDirection) * body.rotation;
                //}
            }
            if (isLerping)
            {
                Vector3.Slerp(bodyUp, targetDirection, 0.5f);
                if (bodyUp == targetDirection)
                    isLerping = false;
            }
            body.GetComponent<Rigidbody>().AddForce(-bodyUp * -gravity);


            //Vector3 bodyUp = body.up;
            //Debug.DrawRay(body.position, -body.up * 100f, Color.red);
            //Vector3 targetDirection = Vector3.ProjectOnPlane((body.position - transform.position).normalized, body.up);
            //Vector3 potentialTargetDirection = Vector3.ProjectOnPlane(transform.position - (body.position), -transform.up);
            //Vector3 normalTargetDirection = (body.position - transform.position).normalized;
            //Debug.DrawRay(transform.position, targetDirection * 100f, Color.blue);
            //Debug.DrawRay(transform.position, potentialTargetDirection * 100f, Color.green);
            //Debug.DrawRay(transform.position, normalTargetDirection * 100f, Color.yellow);

            //if(Vector3.Dot(body.up.normalized, targetDirection.normalized) == 0)
            //{
            //    Debug.Log("Perpendicular");
            //}

            //body.rotation = Quaternion.FromToRotation(bodyUp, targetDirection) * body.rotation;
            //Debug.Log(targetDirection);
            //else if (-bodyUp == -transform.up)
            //{
            //    body.rotation = Quaternion.FromToRotation(bodyUp, -transform.up) * body.rotation;
            //}
            //else if (-bodyUp == -transform.right)
            //{
            //    body.rotation = Quaternion.FromToRotation(bodyUp, -transform.right) * body.rotation;
            //}
            //else if (-bodyUp == -transform.forward)
            //{
            //    body.rotation = Quaternion.FromToRotation(bodyUp, -transform.forward) * body.rotation;

            //}
            //body.GetComponent<Rigidbody>().AddForce(targetDirection * -gravity);
            //Vector3 targetDirection = Vector3.ProjectOnPlane((body.position - transform.position).normalized, body.right); //(body.position - transform.position).normalized;
            //Vector3 bodyUp = body.up;

            //Debug.DrawRay(body.position, (body.position - transform.position).normalized * 100f, Color.red);
            //Debug.DrawRay(body.position, targetDirection * 100f, Color.green);
            //Debug.DrawRay(transform.position, targetDirection * 100f, Color.blue);
            //body.rotation = Quaternion.FromToRotation(bodyUp, targetDirection) * body.rotation;
            //body.GetComponent<Rigidbody>().AddForce(-bodyUp * -gravity);

        }
        else
        {
            // Inverse Cylinder
            Vector3 targetDirection = Vector3.ProjectOnPlane(transform.position - (body.position), -transform.up);
            Debug.DrawRay(transform.position, -transform.up * 100f, Color.blue);
            Debug.DrawRay(transform.position, transform.position - (body.position) * 100f, Color.green);
            Debug.DrawRay(transform.position, targetDirection, Color.red);
            Vector3 bodyUp = body.up;
            //Debug.DrawRay(transform.position, -transform.up * 100f, Color.blue);
            //Debug.DrawRay(body.position, body.up * 100f, Color.green);
            //Debug.DrawRay(transform.position, -targetDirection, Color.red);
            body.rotation = Quaternion.FromToRotation(bodyUp, targetDirection) * body.rotation;
            //body.GetComponent<Rigidbody>().AddForce(-targetDirection * -gravity);
            body.GetComponent<Rigidbody>().AddForce(-bodyUp * -gravity);
            // Inverse Cylinder

            // Walk on Cylinder Works -------------------------------
            //body.gameObject.GetComponent<FirstPersonController>().isHalo = isHalo;
            //Vector3 targetDirection = Vector3.ProjectOnPlane(transform.position - body.position, transform.up);
            //Vector3 bodyUp = body.up;
            //Debug.DrawRay(body.position, targetDirection, Color.red);
            //body.rotation = Quaternion.FromToRotation(bodyUp, -targetDirection) * body.rotation;
            //body.GetComponent<Rigidbody>().AddForce(-targetDirection * gravity);
            // Walk on Cylinder Works ---------------------------------
        }

    }
}
