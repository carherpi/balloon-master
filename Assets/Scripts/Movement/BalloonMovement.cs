using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BalloonMovement : MonoBehaviour
{
    // Variables
    [SerializeField] private bool isGrounded; // is the player on the ground?
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;


    public GameAutomaton GameAutomatonScript;
    Vector3 desiredPosition;
    ConstantForce desiredForce;

    

    public float maxSpeed;
    public float maxHeight;

    private float speed;
    private float offset;
    private float n;
    private float slowDownSpeed;
    private float rotDirection;

    private float desiredRot;
    public float rotSpeed = 5;
    public float damping = 10;
    public Quaternion desiredRotQ;

    public enum BalloonStates
    {
        GoingUpFast,
        GoingUpSlow,
        Floating,
        GoingDown,
    }

    private BalloonStates ballonState;


    private void Awake()
    {
        GameAutomatonScript = FindObjectOfType<GameAutomaton>();
    }

    // Start is called before the first frame update
    void Start()
    {

        ballonState = BalloonStates.GoingDown;

        desiredForce = this.gameObject.GetComponent<ConstantForce>();
        desiredForce.enabled = false;     

        desiredPosition = transform.position;
        desiredPosition.y = 0;
        speed = maxSpeed;

        slowDownSpeed = 0.1f;

        // rot
        desiredRot = transform.eulerAngles.z;
        desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, desiredRot);

    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if ((int)GameAutomatonScript.GetGameState() != 3) // if GameRunning
        {
            return;
        }

        transform.Rotate(new Vector3(rotDirection * 0.1f, rotDirection *  0.1f, rotDirection * 0.3f));


        if (ballonState == BalloonStates.GoingUpFast || ballonState == BalloonStates.GoingUpSlow)
        {
            goUp();
            //Shake();
        } 

        if (ballonState == BalloonStates.Floating)
        {
            goFloating();
        }

        if (ballonState == BalloonStates.GoingDown)
        {
            goDown();
        }
        
        // activate force
        //desiredForce.enabled = true;
    }


    void OnCollisionEnter(Collision collision)
    {

        n = Random.Range(-1.0f, 1.0f);
        if (n > 0)
        {
            offset = Random.Range(-1.5f, -0.5f);
            rotDirection = -1.0f;
        }
        else
        {
            offset = Random.Range(0.5f, 1.5f);
            rotDirection = 1.0f;
        }

        if (collision.gameObject.name.Contains("Boy"))
        {
            FastBounce();

        } else // touch floor or anything else
        {

            SlowBounce();            
        }
    }


    public void FastBounce()
    {

        ballonState = BalloonStates.GoingUpFast;

        // update desired position & speed
        desiredPosition.y = maxHeight;

        Debug.Log("Offset!");
        Debug.Log(offset);

        desiredPosition.x = transform.position.x + offset;
        desiredPosition.z = transform.position.z + offset;
        speed = 3.0f;        
    }

    public void SlowBounce()
    {

        ballonState = BalloonStates.GoingUpSlow;

        // update desired position & speed
        desiredPosition.y = 2;
        speed = 0.3f;        
    }

    public void goUp()
    {
        
        // check if it has arrived desired position
        if (transform.position.y < maxHeight & speed >= 0) {
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);
            speed -= slowDownSpeed; // slows down the balloon rapidly
            //slowDownSpeed += 0.1f;
        } else
        {
            ballonState = BalloonStates.Floating;
            speed = 0.0f;
        }
    }

    public void goFloating()
    {
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);
        
        if (n > 0)
        {
            desiredPosition.x += 0.1f;
            desiredPosition.z += 0.1f;
        } else
        {
            desiredPosition.x -= 0.1f;
            desiredPosition.z -= 0.1f;
        }
        
        speed += 0.01f;

        if (speed >= maxSpeed)
        {
            ballonState = BalloonStates.GoingDown;
            desiredPosition.y = 0;

            if (n > 0)
            {
                
                desiredPosition.x = transform.position.x + 1f;
                desiredPosition.z = transform.position.z + 1f;
            }
            else
            {
                desiredPosition.x = transform.position.x - 1f;
                desiredPosition.z = transform.position.z - 1f;
            }
            
        }
    }

    public void goDown()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);        

    }

    public void Shake()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * damping);
    }
}
