using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerometer : MonoBehaviour
{
    private Rigidbody rbBalloon;
    private float speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        rbBalloon = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Fix for when the child jumps and applies a huge force to the balloon
        if (rbBalloon.velocity.y > 0)
        {
            Debug.Log(rbBalloon.velocity);
            rbBalloon.velocity = new Vector3(0f, 0f, 0f);
        } 
    }

    public Vector3 CalculateDirection()
    {
        Vector3 tilt = Input.acceleration;
        return tilt;
    }
}
