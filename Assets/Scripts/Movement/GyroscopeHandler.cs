using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GyroscopeHandler : MonoBehaviour
{
    private Gyroscope gyro;
    private Quaternion initOrient;
    private Quaternion initOrientSubtract;
    private Vector2 inputMovement;
    private readonly float scaleAxis = 0.1f;
    private bool calibrated;

    private PhotonView pV;

    void Awake()
    {
        this.gyro = Input.gyro;
        this.gyro.enabled = true;
        Debug.Log("Gyro enabled");

        pV = GameObject.Find("GameLogic").GetComponent<PhotonView>();
    }

    /** return: true if calibration successful; false if calibration needs to be repeated after a while */
    public bool Calibrate()
    {
        this.initOrient = this.gyro.attitude;
        this.initOrientSubtract = Quaternion.Inverse(this.initOrient);
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

        Quaternion diffVector = this.gyro.attitude * this.initOrientSubtract;
        //Debug.Log("Enabled: " + this.gyro.enabled);
        //Debug.Log(this.gyro.attitude);
        //Debug.Log("InputMovement: " + diffVector.eulerAngles);

        // adjust values
        float xMove = AdjustValues(diffVector.eulerAngles.x);
        float yMove = AdjustValues(diffVector.eulerAngles.y);
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