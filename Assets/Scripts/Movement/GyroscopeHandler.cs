using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        this.compass = Input.compass;
        this.compass.enabled = true;
        Debug.Log("Compass enabled");
        this.gyro = Input.gyro;
        this.gyro.enabled = true;
        Debug.Log("Gyro enabled");
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
        return calibrated;
    }

    public Vector2 GetPlayerInputMovement()
    {
        if (!calibrated)
        {
            return Vector2.zero;
        }

        Quaternion diffVector = this.gyro.attitude * this.initOrientSubtract;
        Vector3 diffVectorEuler = this.gyro.attitude.eulerAngles - this.initOrientEuler;
        Vector3 diffVectorGravity = this.gyro.gravity - this.initGravity;
        //Debug.Log("Enabled: " + this.gyro.enabled);
        Debug.Log("GyroAttitude        " + this.gyro.attitude.eulerAngles);
        Debug.Log("RotationRate        " + this.gyro.rotationRate);
        Debug.Log("RotRateUnbia        " + this.gyro.rotationRateUnbiased);
        Debug.Log("GyroGravity         " + this.gyro.gravity);
        Debug.Log("UserAccelera        " + this.gyro.userAcceleration);
        //Debug.Log("InputMovementQuate: " + diffVector.eulerAngles);
        //Debug.Log("InputMovementEuler: " + diffVectorEuler);
        Debug.Log("InputMovementGravity: " + diffVectorGravity);
        //Debug.Log("* (0,0,1,0)         " + this.gyro.attitude * new Quaternion(0, 0, 1, 0));
        //Debug.Log("* (0,0,1,0)         " + (this.gyro.attitude * new Quaternion(0, 0, 1, 0)).eulerAngles);

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

    private static Quaternion GyroToUnity(Quaternion q)
    {
        Debug.Log("Original2: " + q);
        Quaternion gyroQuaternion = new Quaternion(q.x, q.y, -q.z, -q.w);
        Debug.Log("Original3: " + gyroQuaternion);
        gyroQuaternion = Quaternion.Euler(90f, 0f, 0f) * gyroQuaternion;
        Debug.Log("Original4: " + gyroQuaternion);
        return gyroQuaternion;
    }

}