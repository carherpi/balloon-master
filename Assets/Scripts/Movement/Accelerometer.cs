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
        
    }

    public Vector3 CalculateDirection()
    {
        Vector3 tilt = Input.acceleration;
        Debug.Log(tilt);
        //tilt = Quaternion.Euler(90, 0, 0) * tilt;
        //Debug.Log(tilt);
        //rbBalloon.AddForce(tilt * speed);

        return tilt;
    }
}
