using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopeHandler : MonoBehaviour
{
    private Gyroscope gyro;
    private Quaternion initOrient;
    private Quaternion initOrientSubtract;
    private Vector2 inputMovement;
    private readonly float scaleAxis = 0.1f;
    private bool calibrated;

    private float y = -0.25f;
    private float x = 0.10f;

    void Awake()
    {
        this.gyro = Input.gyro;
        this.gyro.enabled = true;
        Debug.Log("Gyro enabled");
    }

    void Update()
    {
       //Debug.Log(this.gyro.attitude.x);
    }

    /** return: true if calibration successful; false if calibration needs to be repeated after a while */
    public bool Calibrate()
    {
        /*
        this.initOrient = this.gyro.attitude;
        this.initOrientSubtract = Quaternion.Inverse(this.initOrient);

        Debug.Log("Initial Orientation: " + this.initOrient);
        this.calibrated = this.initOrient != Quaternion.identity;
        return calibrated;
        */
        return true;
    }

    public Vector2 GetPlayerInputMovement()
    {
        /*
        if (false)
        {
            return Vector2.zero;
        }
        */

        //Quaternion diffVector = this.gyro.attitude * this.initOrientSubtract;
        //Debug.Log(diffVector.eulerAngles);

        //Debug.Log("Enabled: " + this.gyro.enabled);
        //Debug.Log(this.gyro.attitude);
        //Debug.Log("InputMovement: " + diffVector.eulerAngles);

        // adjust values
        /*
        float xMove = AdjustValues(diffVector.eulerAngles.x);
        float yMove = AdjustValues(diffVector.eulerAngles.y);
        */

        

        float xMove = (this.gyro.attitude.x - x) * 5;
        float yMove = (this.gyro.attitude.y - y) * 5;

        Vector2 move = new Vector2(xMove, yMove);
        // limit maximal movement to 1
        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        //Debug.Log(move.y);



        if (Mathf.Abs(move.x) <= 0.1)
        {
            move.x = 0;
        }

        if (Mathf.Abs(move.y) <= 0.1)
        {
            move.y = 0;
        }



        //Debug.Log("Movement x: " + move.x + ", y: " + move.y);
        //Debug.Log("Magnitude: " + move.magnitude);

        /*
        Debug.Log(this.initOrientSubtract);
        Debug.Log(diffVector);
        */
        //Debug.Log(move);

        return move;
    }

    private float AdjustValues(float axisMove)
    {
        // handle overflow
        if (axisMove > 180)
        {
            axisMove -= 360;
        }
        // change direction
        axisMove *= -1;
        // scale movement
        axisMove *= this.scaleAxis;
        return axisMove;
    }
}
