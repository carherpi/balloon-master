using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GyroscopeHandler : MonoBehaviour
{
    private Compass compass;
    private Gyroscope gyro;
    private Quaternion initOrient;
    private Quaternion initOrientSubtract;
    private Vector3 initOrientEuler;
    private Vector3 initGravity;
    private Vector2 inputMovement;
    private readonly float scaleAxis = 0.1f;
    private bool calibrated;

    private PhotonView pV;

    void Awake()
    {
        this.compass = Input.compass;
        this.compass.enabled = true;
        Debug.Log("Compass enabled");
        this.gyro = Input.gyro;
        this.gyro.enabled = true;
        Debug.Log("Gyro enabled");

        pV = GameObject.Find("GameLogic").GetComponent<PhotonView>();
    }

    /** return: true if calibration successful; false if calibration needs to be repeated after a while */
    public bool Calibrate()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Debug.LogError("Your device does not have a gyroscope.");
            // return false;
        }
        if (SystemInfo.supportsAccelerometer)
        {
            Debug.LogError("Your device does not have an accelerometer.");
            // return false;
        }
        Debug.Log("Compass trueHeading" + this.compass.trueHeading);
        Debug.Log("Compass magneticHeading" + this.compass.magneticHeading);
        Debug.Log("Compass rawVector" + this.compass.rawVector);

        this.initOrient = this.gyro.attitude;
        this.initGravity = this.gyro.gravity;
        this.initOrientSubtract = Quaternion.Inverse(this.initOrient);
        this.initOrientEuler = this.initOrient.eulerAngles;
        Debug.Log("Initial Orientation: " + this.initOrient);
        this.calibrated = this.initOrient != Quaternion.identity;

        // Send to opponent you're ready
        pV.RPC("OpponentIsReady", RpcTarget.Others, true); 

        return calibrated;
    }

    public Vector2 GetPlayerInputMovement()
    {
        if (!calibrated)
        {
            return Vector2.zero;
        }

        //Quaternion diffVector = this.gyro.attitude * this.initOrientSubtract;
        //Vector3 diffVectorEuler = this.gyro.attitude.eulerAngles - this.initOrientEuler;
        Vector3 diffVectorGravity = this.gyro.gravity - this.initGravity;
        // adjust values
        //float xMove = AdjustValues(diffVector.eulerAngles.x);
        //float yMove = AdjustValues(diffVector.eulerAngles.y);
        //Vector2 move = new Vector2(xMove, yMove);
        float xMove = AdjustValues(diffVectorGravity.x);
        float yMove = AdjustValues(diffVectorGravity.y);
        Vector2 move = new Vector2(xMove, yMove);

        // limit maximal movement to 1
        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        //Debug.Log("Movement x: " + move.x + ", y: " + move.y);
        //Debug.Log("Magnitude: " + move.magnitude);
        return move;
    }

    private float AdjustValues(float axisMove)
    {
        // handle overflow
        //if (axisMove > 180)
        //{
        //    axisMove -= 360;
        //}
        // change direction
        //axisMove *= -1;
        // scale movement
        axisMove *= 4; // this.scaleAxis;
        return axisMove;
    }
}