using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] 
    Transform cameraPosition = null;

    void Update()
    {
        transform.position = cameraPosition.position;
        //transform.position = new Vector3(cameraPosition.localPosition.x, cameraPosition.localPosition.y, cameraPosition.localPosition.z);
        //Debug.DrawRay(transform.position, transform.up * 100f, Color.green);
        transform.up = cameraPosition.up;
        transform.rotation = cameraPosition.parent.GetChild(2).transform.rotation;
        //transform.Rotate(Vector3.up, Space.Self);
        //transform.forward = -transform.forward;
        //transform.localEulerAngles = cameraPosition.TransformDirection(cameraPosition.forward);
    }
}
