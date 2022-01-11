using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerometerHandler : MonoBehaviour
{
    private Vector3 initOrient;
    private Vector3 inputMovement;
    private float minMovement = 0.3f;
    private readonly float scaleAxis = 2;
    private MyVector3Buffer buffer = new MyVector3Buffer(new System.TimeSpan(0, 0, 0, 0, 300)); // save acceleration data from last 0.5 seconds
    private Vector3 accNow = Vector3.zero;
    private Vector3 accOld;

    private void Start()
    {
        this.initOrient = Input.acceleration; // please don't move your phone while setting this
        Debug.Log("Initial Acceleration: " + this.initOrient);
    }
    void Update()
    {
        this.accOld = this.accNow;
        this.accNow = Input.acceleration;
        //Debug.Log("Acce: " + this.accNow);
        buffer.Store(this.accNow);
        // Detect shake
        // Vector3 diffVector = this.accNow - buffer.GetOldestElement();
        Vector3 diffVector = this.accOld - this.accNow;
        //Debug.Log("OldestElem: " + buffer.GetOldestElement());
        //Debug.Log("OldestElem: " + this.accOld);
        //Debug.Log("DiffVector: " + diffVector);
        // adjust values
        float xMove = AdjustValues(diffVector.x);
        float yMove = AdjustValues(diffVector.y);
        Vector2 move = new Vector2(xMove, yMove);
        // is shake severe enough?
        if (move.magnitude > this.minMovement)
        {
            // limit maximal movement to 1
            if (move.magnitude > 1)
            {
                move.Normalize();
            }
            //Debug.Log("Severe enough");
            //buffer.ShowAll();
        }
        //Debug.Log("Hit x: " + move.x + ", y: " + move.y);
        this.inputMovement = new Vector3(move.x, move.y, 0);
        // Detect if shaking phone is severe enough
        //if (().magnitude > this.minMovement)
        //{
        //    //TODO
        //    balloon.Move();
        //    boy.JumpToBalloon();
        //}
    }

    private float AdjustValues(float axisMove)
    {
        // scale movement
        axisMove *= this.scaleAxis;
        // change direction of axis
        axisMove *= -1;
        return axisMove;
    }

    public Vector3 CalculateDirection()
    {
        Vector3 tilt = Input.acceleration;
        //return tilt;
        return this.inputMovement;
    }

}

class MyVector3Buffer
{
    private Queue<KeyValuePair<System.DateTime, Vector3>> queue = new Queue<KeyValuePair<System.DateTime, Vector3>>();
    private System.TimeSpan deltaTime; // after this time the values will be dequeued
    public MyVector3Buffer(System.TimeSpan deltaTime)
    {
        this.deltaTime = deltaTime;
    }

    public void Store(Vector3 element)
    {
        //Debug.Log("Store: " + System.DateTime.UtcNow + ", " + element);
        queue.Enqueue(new KeyValuePair<System.DateTime, Vector3>(System.DateTime.UtcNow, element));
    }

    public Vector3 GetOldestElement()
    {
        System.DateTime nowTime = System.DateTime.UtcNow;
        KeyValuePair<System.DateTime, Vector3> pair;
        bool foundElement = false;
        while (!foundElement)
        {
            pair = queue.Peek();
            //Debug.Log("Peek: " + pair.Key + ", " + pair.Value);
            //Debug.Log("DiffTime: " + (nowTime - pair.Key));
            //Debug.Log("DeltaTime: " + this.deltaTime);

            if (nowTime - pair.Key > this.deltaTime)
            {
                //Debug.Log("Dequeue: " + pair.Key + " - " + nowTime);
                queue.Dequeue();
            }
            else
            {
                foundElement = true;
            }
        }
        return pair.Value;
    }

    public void ShowAll()
    {
        while (queue.Count > 0)
        {
            KeyValuePair<System.DateTime, Vector3> pair = queue.Dequeue();
            Debug.Log("Peek: " + pair.Key + ", " + pair.Value);
        }
    }
}