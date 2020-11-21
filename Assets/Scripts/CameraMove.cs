using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3 startPos;
    Quaternion startRot;

    Vector3 destination;
    Quaternion destRot;

    public bool trackingTarget;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDestination(Vector3 dest, Quaternion quat)
    {
        destination = dest;
        destRot = quat;
    }

    public void setDestination(Vector3 dest)
    {
        destination = dest;
    }

    public void setDestination(Quaternion quat)
    {
        destRot = quat;
    }

    public void cameraMoveTo()
    {
        if (trackingTarget)
        {
            transform.position = transform.position + (destination - transform.position) * 0.1f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (destRot.eulerAngles - transform.rotation.eulerAngles) * 0.1f);
        }
    }
}
