using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BalloonMovement : MonoBehaviour
{
    // Variables
    [SerializeField] private bool isGrounded; // is the player on the ground?
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;

    private Vector3 velocity;

    public GameAutomaton GameAutomatonScript;

    Vector3 oldPosition;
    Vector3 desiredPosition;

    Rigidbody rbody;

    bool collision = false;

    public float startSpeed;

    private void Awake()
    {
        GameAutomatonScript = FindObjectOfType<GameAutomaton>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<ConstantForce>().enabled = false;
        rbody = this.gameObject.GetComponent<Rigidbody>();

        desiredPosition = transform.position + new Vector3(0, -transform.position.y, 0);

        oldPosition = transform.position;
    }

    void Update()
    {
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (transform.position != desiredPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * 1);
        }

        Movement();
    }

    private void Movement()
    {
        if ((int)GameAutomatonScript.GetGameState() != 3) // if GameRunning
        {
            return;
            
        }

        // Activate ct force
        this.gameObject.GetComponent<ConstantForce>().enabled = true;

    }

    void OnCollisionEnter()
    {
        /*
        if (other.gameObject.name == "gameobject2")
            rigidbody.AddForce(Vector3.Up * force);
        */
        Debug.Log("Bounce!");

        //this.gameObject.GetComponent<ConstantForce>().force = new Vector3(0f, 1f, 0f);       

        transform.position = transform.position + new Vector3(0, 3, 0);
        rbody.drag = 20;

        //desiredPosition = transform.position + new Vector3(0, 1, 0);


    }
}
